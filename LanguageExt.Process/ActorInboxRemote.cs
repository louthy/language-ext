using System;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Microsoft.FSharp.Control;
using System.Threading;
using static LanguageExt.Process;
using static LanguageExt.Prelude;
using LanguageExt.Trans;

namespace LanguageExt
{
    class ActorInboxRemote<S,T> : IActorInbox
    {
        ProcessId supervisor;
        ICluster cluster;
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        IDisposable userSub;
        IDisposable sysSub;
        Actor<S, T> actor;
        int version = 0;
        string actorPath;

        public Unit Shutdown()
        {
            return unit;
        }

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

            userInbox = StartMailbox<UserControlMessage>(actor, ClusterUserInboxKey, tokenSource.Token, ActorInboxCommon.UserMessageInbox);
            sysInbox = StartMailbox<SystemMessage>(actor, ClusterSystemInboxKey, tokenSource.Token, ActorInboxCommon.SystemMessageInbox);

            CheckRemoteInbox(ClusterUserInboxKey);
            CheckRemoteInbox(ClusterSystemInboxKey);

            userSub = this.cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey)
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Subscribe( msg =>
                        {
                            if (userInbox.CurrentQueueLength == 0)
                            {
                                CheckRemoteInbox(ClusterUserInboxKey);
                            }
                        });

            sysSub = this.cluster.SubscribeToChannel<string>(ClusterSystemInboxNotifyKey)
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Subscribe(msg =>
                        {
                            if (sysInbox.CurrentQueueLength == 0)
                            {
                                CheckRemoteInbox(ClusterSystemInboxKey);
                            }
                        });

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

        private RemoteMessageDTO GetNextMessage(string key)
        {
            RemoteMessageDTO dto = null;

            do
            {
                dto = cluster.Peek<RemoteMessageDTO>(key);
                if (dto == null || (dto.Tag == 0 && dto.Type == 0))
                {
                    cluster.Dequeue<RemoteMessageDTO>(key);
                    if (cluster.QueueLength(key) == 0) return null;
                }
            }
            while (dto.Tag == 0 && dto.Type == 0);

            return dto;
        }

        private void CheckRemoteInbox(string key)
        {
            if (cluster.QueueLength(key) > 0)
            {
                var dto = GetNextMessage(key);
                if (dto != null)
                {
                    var msg = MessageSerialiser.DeserialiseMsg(dto, actor.Id);

                    switch (msg.MessageType)
                    {
                        case Message.Type.ActorSystem: ActorContext.LocalRoot.Tell(msg, dto.Sender); break;
                        case Message.Type.System: sysInbox.Post((SystemMessage)msg); break;
                        case Message.Type.User: userInbox.Post((UserControlMessage)msg); break;
                        case Message.Type.UserControl: userInbox.Post((UserControlMessage)msg); break;
                    }
                }
            }
        }

        private FSharpMailboxProcessor<TMsg> StartMailbox<TMsg>(Actor<S, T> actor, string key, CancellationToken cancelToken, Action<Actor<S, T>, TMsg> handler) where TMsg : Message =>
            ActorInboxCommon.Mailbox<S, T, TMsg>(Some(cluster), actor.Flags, cancelToken, msg =>
            {
                try
                {
                    handler(actor, msg);
                }
                catch (Exception e)
                {
                    Process.tell(ActorContext.DeadLetters, DeadLetter.create(ActorContext.Sender, actor.Id, e, "Remote message inbox.", msg));
                    logSysErr(e);
                }
                finally
                {
                    // Remove from queue, then see if there are any more to process.
                    cluster.Dequeue<RemoteMessageDTO>(key);
                    CheckRemoteInbox(key);
                }
            });

        public void Dispose()
        {
            var ts = tokenSource;
            if (ts != null) ts.Dispose();

            if (userSub != null)
            {
                var sub = userSub;
                sub.Dispose();
                userSub = null;
            }

            if (sysSub != null)
            {
                var sub = sysSub;
                sub.Dispose();
                sysSub = null;
            }
        }
    }
}
