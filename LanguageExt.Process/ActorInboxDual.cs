using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.FSharp.Control;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using static LanguageExt.Process;
using static LanguageExt.Prelude;
using LanguageExt.Trans;

namespace LanguageExt
{
    /// <summary>
    /// This is both a local and remote inbox in one. 
    /// 
    /// TODO: Lots of cut n paste from the Local and Remote variants, need to factor out the 
    ///       common elements.
    /// </summary>
    internal class ActorInboxDual<S, T> : IActorInbox, ILocalActorInbox
    {
        ProcessId supervisor;
        ICluster cluster;
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        Actor<S, T> actor;
        int version;
        object sync = new object();

        string actorPath;

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

            CheckRemoteInbox(ClusterUserInboxKey, this.cluster, actor.Id, sysInbox, userInbox);
            CheckRemoteInbox(ClusterSystemInboxKey, this.cluster, actor.Id, sysInbox, userInbox);

            this.cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey, 
                msg =>
                {
                    if (userInbox.CurrentQueueLength == 0)
                    {
                        CheckRemoteInbox(ClusterUserInboxKey, this.cluster, actor.Id, sysInbox, userInbox);
                    }
                });

            this.cluster.SubscribeToChannel<string>(ClusterSystemInboxNotifyKey, 
                msg =>
                {
                    if (sysInbox.CurrentQueueLength == 0)
                    {
                        CheckRemoteInbox(ClusterSystemInboxKey, this.cluster, actor.Id, sysInbox, userInbox);
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

        public Unit Ask(object message, ProcessId sender)
        {
            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message).IfSome(msg => userInbox.Post(msg));
            }
            return unit;
        }

        public Unit Tell(object message, ProcessId sender)
        {
            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message).IfSome(msg => userInbox.Post(msg));
            }
            return unit;
        }

        public Unit TellSystem(SystemMessage message)
        {
            if (sysInbox != null)
            {
                if (message == null) throw new ArgumentNullException(nameof(message));
                sysInbox.Post(message);
            }
            return unit;
        }

        public Unit TellUserControl(UserControlMessage message)
        {
            if (userInbox != null)
            {
                if (message == null) throw new ArgumentNullException(nameof(message));
                userInbox.Post(message);
            }
            return unit;
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

        public void CheckRemoteInbox(string key, ICluster cluster, ProcessId self, FSharpMailboxProcessor<SystemMessage> sysInbox, FSharpMailboxProcessor<UserControlMessage> userInbox)
        {
            try
            {
                int count = cluster.QueueLength(key);

                while(count > 0)
                {
                    Option<Tuple<RemoteMessageDTO, Message>> pair;
                    lock (sync)
                    {
                        pair = ActorInboxCommon.GetNextMessage(cluster, self, key);
                        pair.IfSome(x => cluster.Dequeue<RemoteMessageDTO>(key));
                    }

                    pair.IfSome(x => map(x, (dto, msg) => {
                        switch (msg.MessageType)
                        {
                            case Message.Type.ActorSystem:  ActorContext.LocalRoot.Tell(msg, dto.Sender); break;
                            case Message.Type.System:       sysInbox.Post((SystemMessage)msg); break;
                            case Message.Type.User:         userInbox.Post((UserControlMessage)msg); break;
                            case Message.Type.UserControl:  userInbox.Post((UserControlMessage)msg); break;
                        }
                    }));
                    count--;
                }
            }
            catch (Exception e)
            {
                logSysErr("CheckRemoteInbox failed for " + self, e);
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
                    tell(ActorContext.DeadLetters, DeadLetter.create(ActorContext.Sender, actor.Id, e, "Remote message inbox.", msg));
                    logSysErr(e);
                }
                finally
                {
                    // Remove from queue, then see if there are any more to process.
                    CheckRemoteInbox(key, cluster, actor.Id, sysInbox, userInbox);
                }
            });
    }
}
