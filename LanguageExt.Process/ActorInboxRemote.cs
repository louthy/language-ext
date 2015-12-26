using System;
using System.Threading;
using LanguageExt.Trans;
using static LanguageExt.Process;
using static LanguageExt.Prelude;
using Microsoft.FSharp.Control;
using Newtonsoft.Json;

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

            actorPath = actor.Id.ToString();

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
            // System inbox is just listening to the notifications, that means that system
            // messages don't persist.
            cluster.UnsubscribeChannel(ClusterSystemInboxNotifyKey);
            cluster.SubscribeToChannel<RemoteMessageDTO>(ClusterSystemInboxNotifyKey).Subscribe(SysInbox);
        }

        void SubscribeToUserInboxChannel()
        {
            cluster.UnsubscribeChannel(ClusterUserInboxNotifyKey);
            cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey).Subscribe(msg => userNotify.Post(msg));
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

        /// <summary>
        /// TODO: This is a combination of code in ActorCommon.GetNextMessage and
        ///       CheckRemoteInbox.  Some factoring is needed.
        /// </summary>
        void SysInbox(RemoteMessageDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    // Failed to deserialise properly
                    return;
                }
                if (dto.Tag == 0 && dto.Type == 0)
                {
                    // Message is bad
                    tell(ActorContext.DeadLetters, DeadLetter.create(dto.Sender, actor.Id, null, "Failed to deserialise message: ", dto));
                    return;
                }
                var msg = MessageSerialiser.DeserialiseMsg(dto, actor.Id);

                try
                {
                    lock(sync)
                    {
                        ActorInboxCommon.SystemMessageInbox(actor, this, (SystemMessage)msg, parent);
                    }
                }
                catch (Exception e)
                {
                    ActorContext.WithContext(new ActorItem(actor, this, actor.Flags), parent, dto.Sender, msg as ActorRequest, msg, msg.SessionId, () => replyErrorIfAsked(e));
                    tell(ActorContext.DeadLetters, DeadLetter.create(dto.Sender, actor.Id, e, "Remote message inbox.", msg));
                    logSysErr(e);
                }
            }
            catch (Exception e)
            {
                logSysErr(e);
            }
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
                                case Message.Type.User:        directive = ActorInboxCommon.UserMessageInbox(actor, inbox, (UserControlMessage)msg, parent); break;
                                case Message.Type.UserControl: directive = ActorInboxCommon.UserMessageInbox(actor, inbox, (UserControlMessage)msg, parent); break;
                            }
                        }
                        catch (Exception e)
                        {
                            ActorContext.WithContext(new ActorItem(actor, inbox, actor.Flags), parent, dto.Sender, msg as ActorRequest, msg, msg.SessionId, () => replyErrorIfAsked(e));
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
