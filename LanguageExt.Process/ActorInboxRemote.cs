using System;
using System.Threading;
using LanguageExt.Trans;
using static LanguageExt.Process;
using static LanguageExt.Prelude;
using Microsoft.FSharp.Control;

namespace LanguageExt
{
    class ActorInboxRemote<S,T> : IActorInbox
    {
        ICluster cluster;
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<string> userNotify;
        FSharpMailboxProcessor<string> sysNotify;
        Actor<S, T> actor;
        ActorItem parent;
        int maxMailboxSize;
        string actorPath;
        object sync = new object();

        public Unit Startup(IActor process, ActorItem parent, Option<ICluster> cluster, int maxMailboxSize)
        {
            if (cluster.IsNone) throw new Exception("Remote inboxes not supported when there's no cluster");
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.cluster = cluster.LiftUnsafe();
            this.maxMailboxSize = maxMailboxSize < 0
                ? ActorConfig.Default.MaxMailboxSize
                : maxMailboxSize;
            this.parent = parent;

            // Registered process remote address hack
            actorPath = actor.Id.Path.StartsWith(Registered.Path, StringComparison.Ordinal)
                ? actor.Id.Skip(1).ToString()
                : actor.Id.ToString();

            userNotify = ActorInboxCommon.StartNotifyMailbox(tokenSource.Token, msgId => CheckRemoteInbox(ClusterUserInboxKey, true));
            sysNotify = ActorInboxCommon.StartNotifyMailbox(tokenSource.Token, msgId => CheckRemoteInbox(ClusterSystemInboxKey, false));

            SubscribeToSysInboxChannel();
            SubscribeToUserInboxChannel();

            return unit;
        }

        string ClusterKey =>
            ActorInboxCommon.ClusterKey(actorPath);

        string ClusterUserInboxKey =>
            ActorInboxCommon.ClusterUserInboxKey(actorPath);

        string ClusterSystemInboxKey =>
            ActorInboxCommon.ClusterSystemInboxKey(actorPath);

        string ClusterUserInboxNotifyKey =>
            ActorInboxCommon.ClusterUserInboxNotifyKey(actorPath);

        string ClusterSystemInboxNotifyKey =>
            ActorInboxCommon.ClusterSystemInboxNotifyKey(actorPath);

        void SubscribeToSysInboxChannel()
        {
            cluster.SubscribeToChannel<string>(ClusterSystemInboxNotifyKey, msg => sysNotify.Post(msg));
            // We want the check done asyncronously, in case the setup function creates child processes that
            // won't exist if we invoke directly.
            cluster.PublishToChannel(ClusterSystemInboxNotifyKey, Guid.NewGuid().ToString());
        }

        void SubscribeToUserInboxChannel()
        {
            cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey, msg => userNotify.Post(msg));
            // We want the check done asyncronously, in case the setup function creates child processes that
            // won't exist if we invoke directly.
            cluster.PublishToChannel(ClusterUserInboxNotifyKey, Guid.NewGuid().ToString());
        }

        public bool IsPaused
        {
            get;
            private set;
        }

        public Unit Pause()
        {
            lock (sync)
            {
                if (!IsPaused)
                {
                    IsPaused = true;
                    cluster.UnsubscribeChannel(ClusterUserInboxNotifyKey);
                }
            }
            return unit;
        }

        public Unit Unpause()
        {
            lock (sync)
            {
                if (IsPaused)
                {
                    IsPaused = false;
                    SubscribeToUserInboxChannel();
                }
            }
            return unit;
        }

        void CheckRemoteInbox(string key, bool pausable)
        {
            var inbox = this;
            var count = cluster.QueueLength(key);

            while (count > 0 && (!pausable || !IsPaused))
            {
                var directive = InboxDirective.Default;

                ActorInboxCommon.GetNextMessage(cluster, actor.Id, key).IfSome(
                    x => iter(x, (dto, msg) =>
                    {
                        try
                        {
                            switch (msg.MessageType)
                            {
                                case Message.Type.System:      directive = ActorInboxCommon.SystemMessageInbox(actor, inbox, (SystemMessage)msg, parent); break;
                                case Message.Type.User:        directive = ActorInboxCommon.UserMessageInbox(actor, inbox, (UserControlMessage)msg, parent); break;
                                case Message.Type.UserControl: directive = ActorInboxCommon.UserMessageInbox(actor, inbox, (UserControlMessage)msg, parent); break;
                            }
                        }
                        catch (Exception e)
                        {
                            ActorContext.WithContext(new ActorItem(actor, inbox, actor.Flags), parent, dto.Sender, msg as ActorRequest, msg, () => replyErrorIfAsked(e));
                            tell(ActorContext.DeadLetters, DeadLetter.create(dto.Sender, actor.Id, e, "Remote message inbox.", msg));
                            logSysErr(e);
                        }
                        finally
                        {
                            if (directive == InboxDirective.Default)
                            {
                                cluster.Dequeue<RemoteMessageDTO>(key);
                            }
                        }
                    }));

                if (directive == InboxDirective.Default)
                {
                    count--;
                }
            }
        }

        public Unit Shutdown()
        {
            Dispose();
            return unit;
        }

        public void Dispose()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = null;

            cluster?.UnsubscribeChannel(ClusterUserInboxNotifyKey);
            cluster?.UnsubscribeChannel(ClusterSystemInboxNotifyKey);
            cluster = null;
        }
    }
}
