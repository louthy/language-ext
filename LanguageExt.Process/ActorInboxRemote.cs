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
        int version = 0;
        string actorPath;
        object sync = new object();

        public Unit Startup(IActor process, ActorItem parent, Option<ICluster> cluster, int version = 0)
        {
            if (cluster.IsNone) throw new Exception("Remote inboxes not supported when there's no cluster");
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.cluster = cluster.LiftUnsafe();
            this.version = version;
            this.parent = parent;

            // Registered process remote address hack
            actorPath = actor.Id.Path.StartsWith(Registered.Path)
                ? actor.Id.Skip(1).ToString()
                : actor.Id.ToString();

            // Preparing for message versioning support
            //actorPath += "-" + version;

            userNotify = ActorInboxCommon.StartNotifyMailbox(tokenSource.Token, msgId => CheckRemoteInbox(ClusterUserInboxKey));
            sysNotify = ActorInboxCommon.StartNotifyMailbox(tokenSource.Token, msgId => CheckRemoteInbox(ClusterSystemInboxKey));

            this.cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey, msg => userNotify.Post(msg));
            this.cluster.SubscribeToChannel<string>(ClusterSystemInboxNotifyKey, msg => sysNotify.Post(msg));

            // We want the check done asyncronously, in case the setup function creates child processes that
            // won't exist if we invoke directly.
            this.cluster.PublishToChannel(ClusterUserInboxNotifyKey, Guid.NewGuid().ToString());
            this.cluster.PublishToChannel(ClusterSystemInboxNotifyKey, Guid.NewGuid().ToString());

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

        private void CheckRemoteInbox(string key)
        {
            var inbox = this;
            var count = cluster.QueueLength(key);

            while (count > 0)
            {
                ActorInboxCommon.GetNextMessage(cluster, actor.Id, key).IfSome(
                    x => iter(x, (dto, msg) =>
                    {
                        try
                        {
                            switch (msg.MessageType)
                            {
                                case Message.Type.System: ActorInboxCommon.SystemMessageInbox(actor, inbox, (SystemMessage)msg, parent); break;
                                case Message.Type.User: ActorInboxCommon.UserMessageInbox(actor, inbox, (UserControlMessage)msg, parent); break;
                                case Message.Type.UserControl: ActorInboxCommon.UserMessageInbox(actor, inbox, (UserControlMessage)msg, parent); break;
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
                            cluster.Dequeue<RemoteMessageDTO>(key);
                        }
                    }));
                count--;
            }
        }

        public Unit Shutdown()
        {
            Dispose();
            return unit;
        }

        public void Dispose()
        {
            var ts = tokenSource;
            ts?.Cancel();
            ts?.Dispose();
            ts = null;

            var cs = cluster;
            cs?.UnsubscribeChannel(ClusterUserInboxNotifyKey);
            cs?.UnsubscribeChannel(ClusterSystemInboxNotifyKey);
            cluster = null;
        }
    }
}
