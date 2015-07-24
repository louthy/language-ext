using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Control;
using System.Threading;
using Microsoft.FSharp.Core;
using Newtonsoft.Json;

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

        public Unit Shutdown()
        {
            return unit;
        }

        string ClusterKey =>
            actor.Id.Path;

        string ClusterUserInboxKey =>
            ClusterKey + "-user-inbox";

        string ClusterSystemInboxKey =>
            ClusterKey + "-system-inbox";

        string ClusterUserInboxNotifyKey => 
            ClusterUserInboxKey + "-notify";

        string ClusterSystemInboxNotifyKey => 
            ClusterSystemInboxKey + "-notify";

        public Unit Startup(IProcess process, ProcessId supervisor, Option<ICluster> cluster)
        {
            if (cluster.IsNone) throw new Exception("Remote inboxes not supported when there's no cluster");
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.supervisor = supervisor;
            this.cluster = cluster.LiftUnsafe();

            userInbox = StartMailbox<UserControlMessage>(actor, ClusterUserInboxKey, tokenSource.Token, ActorInboxCommon.UserMessageInbox);
            sysInbox = StartMailbox<SystemMessage>(actor, ClusterSystemInboxKey, tokenSource.Token, ActorInboxCommon.SystemMessageInbox);

            CheckRemoteInbox(ClusterUserInboxKey);
            CheckRemoteInbox(ClusterSystemInboxKey);

            userSub = this.cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey)
                        .Subscribe( msg =>
                        {
                            if (userInbox.CurrentQueueLength == 0)
                            {
                                CheckRemoteInbox(ClusterUserInboxKey);
                            }
                        });

            sysSub = this.cluster.SubscribeToChannel<string>(ClusterSystemInboxNotifyKey)
                        .Subscribe(msg =>
                        {
                            if (sysInbox.CurrentQueueLength == 0)
                            {
                                CheckRemoteInbox(ClusterSystemInboxKey);
                            }
                        });

            return unit;
        }

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

        private void CheckRemoteInbox(string key)
        {
            if (cluster.QueueLength(key) > 0)
            {
                var dto = cluster.Peek<RemoteMessageDTO>(key);
                var obj = JsonConvert.DeserializeObject(dto.Content);

                var sender = String.IsNullOrEmpty(dto.Sender) ? ProcessId.NoSender : new ProcessId(dto.Sender);
                var replyTo = String.IsNullOrEmpty(dto.ReplyTo) ? ProcessId.NoSender : new ProcessId(dto.ReplyTo);

                switch ((Message.Type)dto.Type)
                {
                    case Message.Type.System:
                        switch ((SystemMessageTag)dto.Tag)
                        {
                            case SystemMessageTag.ChildIsFaulted:
                                sysInbox.Post(new SystemChildIsFaultedMessage(dto.Child, new Exception(dto.Exception)));
                                break;

                            case SystemMessageTag.Restart:
                                sysInbox.Post(new SystemRestartMessage());
                                break;

                            case SystemMessageTag.LinkChild:
                                sysInbox.Post(new SystemLinkChildMessage(dto.Child));
                                break;

                            case SystemMessageTag.UnLinkChild:
                                sysInbox.Post(new SystemUnLinkChildMessage(dto.Child));
                                break;
                        }
                        break;

                    case Message.Type.User:
                        switch ((UserControlMessageTag)dto.Tag)
                        {
                            case UserControlMessageTag.UserAsk:
                                userInbox.Post(new ActorRequest(obj, actor.Id, replyTo, dto.RequestId));
                                break;

                            case UserControlMessageTag.User:
                                userInbox.Post(new UserMessage(obj, sender, replyTo));
                                break;
                        }
                        break;

                    case Message.Type.UserControl:
                        switch ((UserControlMessageTag)dto.Tag)
                        {
                            case UserControlMessageTag.Shutdown:
                                userInbox.Post(new UserControlShutdownMessage());
                                break;
                        }
                        break;
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
                    tell(ActorContext.DeadLetters, msg, actor.Id);
                    logSysErr(e);
                }
                finally
                {
                    // Remove from queue, then see if there are any more to process.
                    cluster.Dequeue<RemoteMessageDTO>(key);
                    CheckRemoteInbox(key);
                }
            });
    }
}
