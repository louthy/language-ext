using System;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Microsoft.FSharp.Control;
using System.Threading;
using static LanguageExt.Process;
using static LanguageExt.Prelude;
using LanguageExt.Trans;
using System.Threading.Tasks;

namespace LanguageExt
{
    class ActorInboxRemote<S,T> : IActorInbox
    {
        ProcessId supervisor;
        ICluster cluster;
        CancellationTokenSource tokenSource;
        Actor<S, T> actor;
        int version = 0;
        string actorPath;
        object sync = new object();

        public Unit Startup(IActor process, ProcessId supervisor, Option<ICluster> cluster, int version = 0)
        {
            if (cluster.IsNone) throw new Exception("Remote inboxes not supported when there's no cluster");
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.supervisor = supervisor;
            this.cluster = cluster.LiftUnsafe();
            this.version = version;

            // Registered process remote address hack
            actorPath = actor.Id.Path.StartsWith(Registered.Path)
                ? actor.Id.Skip(1).ToString()
                : actor.Id.ToString();

            // Preparing for message versioning support
            //actorPath += "-" + version;

            this.cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey,
                msg =>
                {
                    CheckRemoteInbox(ClusterUserInboxKey, this.cluster, actor.Id, ActorInboxCommon.UserMessageInbox, ActorInboxCommon.SystemMessageInbox);
                });

            this.cluster.SubscribeToChannel<string>(ClusterSystemInboxNotifyKey,
                msg =>
                {
                    CheckRemoteInbox(ClusterSystemInboxKey, this.cluster, actor.Id, ActorInboxCommon.UserMessageInbox, ActorInboxCommon.SystemMessageInbox);
                });

            // We want the check done asyncronously, in case the setup function creates child processes that
            // won't exist if we invoke directly.
            this.cluster.PublishToChannel(ClusterUserInboxNotifyKey, "Startup");
            this.cluster.PublishToChannel(ClusterSystemInboxNotifyKey, "Startup");

            return unit;
        }

        string ClusterKey =>
            actorPath;

        string ClusterUserInboxKey =>
            ClusterKey + "-user-inbox";

        string ClusterSystemInboxKey =>
            ClusterKey + "-system-inbox";

        string ClusterUserInboxNotifyKey =>
            ClusterUserInboxKey + "-notify";

        string ClusterSystemInboxNotifyKey =>
            ClusterSystemInboxKey + "-notify";

        private void CheckRemoteInbox(string key, ICluster cluster, ProcessId self, Action<Actor<S, T>, UserControlMessage> userInbox, Action<Actor<S, T>, SystemMessage> sysInbox)
        {
            lock(sync)
            {
                var count = cluster.QueueLength(key);

                while (count > 0)
                {
                    ActorInboxCommon.GetNextMessage(cluster, self, key).IfSome(
                        x => map(x, (dto, msg) =>
                        {
                            try
                            {
                                switch (msg.MessageType)
                                {
                                    case Message.Type.ActorSystem:  ActorContext.LocalRoot.Tell(msg, dto.Sender); break;
                                    case Message.Type.System:       sysInbox(actor, (SystemMessage)msg); break;
                                    case Message.Type.User:         userInbox(actor, (UserControlMessage)msg); break;
                                    case Message.Type.UserControl:  userInbox(actor, (UserControlMessage)msg); break;
                                }
                            }
                            catch (Exception e)
                            {
                                ActorContext.WithContext(actor, dto.Sender, msg as ActorRequest, msg, () => replyErrorIfAsked(e));
                                tell(ActorContext.DeadLetters, DeadLetter.create(dto.Sender, self, e, "Remote message inbox.", msg));
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
        }

        public Unit Shutdown()
        {
            Dispose();
            return unit;
        }

        public void Dispose()
        {
            var ts = tokenSource;
            if (ts != null) ts.Dispose();
            this.cluster.UnsubscribeChannel(ClusterUserInboxNotifyKey);
            this.cluster.UnsubscribeChannel(ClusterSystemInboxNotifyKey);
        }
    }
}
