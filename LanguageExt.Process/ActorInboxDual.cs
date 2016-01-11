using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using Microsoft.FSharp.Control;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using static LanguageExt.Process;
using static LanguageExt.Prelude;
using LanguageExt.Trans;
using Newtonsoft.Json;

namespace LanguageExt
{
    /// <summary>
    /// This is both a local and remote inbox in one. 
    /// 
    /// TODO: Lots of cut n paste from the Local and Remote variants, need to factor out the 
    ///       common elements.
    /// </summary>
    class ActorInboxDual<S, T> : IActorInbox, ILocalActorInbox
    {
        ICluster cluster;
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        Actor<S, T> actor;
        ActorItem parent;
        int maxMailboxSize;
        object sync = new object();

        string actorPath;

        public Unit Startup(IActor process, ActorItem parent, Option<ICluster> cluster, int maxMailboxSize)
        {
            if (cluster.IsNone) throw new Exception("Remote inboxes not supported when there's no cluster");
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.parent = parent;
            this.cluster = cluster.LiftUnsafe();
            this.maxMailboxSize = maxMailboxSize < 0
                ? ActorConfig.Default.MaxMailboxSize
                : maxMailboxSize;

            actorPath = actor.Id.ToString();

            userInbox = StartMailbox<UserControlMessage>(actor, ClusterUserInboxKey, tokenSource.Token, ActorInboxCommon.UserMessageInbox, true);
            sysInbox = StartMailbox<SystemMessage>(actor, ClusterSystemInboxKey, tokenSource.Token, ActorInboxCommon.SystemMessageInbox, false);

            SubscribeToSysInboxChannel();
            SubscribeToUserInboxChannel();

            return unit;
        }

        void SubscribeToSysInboxChannel()
        {
            cluster.UnsubscribeChannel(ClusterSystemInboxNotifyKey);
            cluster.SubscribeToChannel<string>(ClusterSystemInboxNotifyKey).Subscribe(
                msg =>
                {
                    if (sysInbox.CurrentQueueLength == 0)
                    {
                        CheckRemoteInbox(ClusterSystemInboxKey, cluster, actor.Id, sysInbox, userInbox, false);
                    }
                });
            cluster.PublishToChannel(ClusterSystemInboxNotifyKey, Guid.NewGuid().ToString());
        }

        void SubscribeToUserInboxChannel()
        {
            cluster.UnsubscribeChannel(ClusterUserInboxNotifyKey);
            cluster.SubscribeToChannel<string>(ClusterUserInboxNotifyKey).Subscribe(
                msg =>
                {
                    if (userInbox.CurrentQueueLength == 0)
                    {
                        CheckRemoteInbox(ClusterUserInboxKey, cluster, actor.Id, sysInbox, userInbox, true);
                    }
                });
            cluster.PublishToChannel(ClusterUserInboxNotifyKey, Guid.NewGuid().ToString());
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

        public bool IsPaused
        {
            get;
            private set;
        }

        public Unit Pause()
        {
            lock(sync)
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

                    // Wake up the user inbox to process any messages that have
                    // been waiting patiently.
                    SubscribeToUserInboxChannel();
                }
            }
            return unit;
        }

        public Unit Ask(object message, ProcessId sender)
        {
            if (userInbox.CurrentQueueLength >= maxMailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, maxMailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message)
                                .IfSome(msg =>
                                {
                                    if (IsPaused)
                                    {
                                        new ActorDispatchRemote(actor.Id, cluster).Ask(message, sender);
                                    }
                                    else
                                    {
                                        userInbox.Post(msg);
                                    }
                                });
            }
            return unit;
        }

        public Unit Tell(object message, ProcessId sender)
        {
            if (userInbox.CurrentQueueLength >= maxMailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, maxMailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message)
                                .IfSome(msg =>
                                {
                                    if (IsPaused)
                                    {
                                        new ActorDispatchRemote(actor.Id, cluster).Tell(message, sender, Message.TagSpec.User);
                                    }
                                    else
                                    {
                                        userInbox.Post(msg);
                                    }
                                });
            }
            return unit;
        }

        public Unit TellSystem(SystemMessage message)
        {
            if (sysInbox.CurrentQueueLength >= maxMailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, maxMailboxSize, "system");
            }

            if (sysInbox != null)
            {
                if (message == null) throw new ArgumentNullException(nameof(message));
                sysInbox.Post(message);
            }
            return unit;
        }

        public Unit TellUserControl(UserControlMessage msg)
        {
            if (userInbox.CurrentQueueLength >= maxMailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, maxMailboxSize, "user");
            }

            if (userInbox != null)
            {
                if (msg == null) throw new ArgumentNullException(nameof(msg));

                if (IsPaused)
                {
                    new ActorDispatchRemote(actor.Id, cluster).TellUserControl(msg, ProcessId.None);
                }
                else
                {
                    userInbox.Post(msg);
                }
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
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = null;

            cluster?.UnsubscribeChannel(ClusterUserInboxNotifyKey);
            cluster?.UnsubscribeChannel(ClusterSystemInboxNotifyKey);
            cluster = null;
        }

        public void CheckRemoteInbox(string key, ICluster cluster, ProcessId self, FSharpMailboxProcessor<SystemMessage> sysInbox, FSharpMailboxProcessor<UserControlMessage> userInbox, bool pausable)
        {
            try
            {
                int count = cluster.QueueLength(key);

                while (count > 0 && (!pausable || !IsPaused))
                {
                    Option<Tuple<RemoteMessageDTO, Message>> pair;
                    lock (sync)
                    {
                        pair = ActorInboxCommon.GetNextMessage(cluster, self, key);
                        pair.IfSome(x => cluster.Dequeue<RemoteMessageDTO>(key));
                    }

                    pair.IfSome(x => iter(x, (dto, msg) =>
                    {
                        switch (msg.MessageType)
                        {
                            case Message.Type.System: sysInbox.Post((SystemMessage)msg); break;
                            case Message.Type.User: userInbox.Post((UserControlMessage)msg); break;
                            case Message.Type.UserControl: userInbox.Post((UserControlMessage)msg); break;
                        }
                    }));
                    count--;
                }
            }
            catch (Exception e)
            {
                logSysErr($"CheckRemoteInbox failed for {self}", e);
            }
        }

        FSharpMailboxProcessor<TMsg> StartMailbox<TMsg>(Actor<S, T> actor, string key, CancellationToken cancelToken, Func<Actor<S, T>, IActorInbox, TMsg, ActorItem, InboxDirective> handler, bool pausable) 
            where TMsg : Message =>
            ActorInboxCommon.Mailbox<TMsg>(cancelToken, msg =>
            {
                try
                {
                    var directive = handler(actor, this, msg, parent);
                    // TODO: Push msg to front of queue if directive requests it
                }
                catch (Exception e)
                {
                    replyErrorIfAsked(e);
                    tell(ActorContext.DeadLetters, DeadLetter.create(ActorContext.Sender, actor.Id, e, "Remote message inbox.", msg));
                    logSysErr(e);
                }
                finally
                {
                    // Remove from queue, then see if there are any more to process.
                    CheckRemoteInbox(key, cluster, actor.Id, sysInbox, userInbox, pausable);
                }
            });

        /// <summary>
        /// Number of unprocessed items
        /// </summary>
        public int Count =>
            (userInbox?.CurrentQueueLength).GetValueOrDefault();

        public bool HasStateTypeOf<TState>() =>
            typeof(TState).GetTypeInfo().IsAssignableFrom(typeof(S).GetTypeInfo());

        public bool CanAcceptMessageType<TMsg>()
        {
            if (typeof(TMsg) == typeof(TerminatedMessage) || typeof(TMsg) == typeof(UserControlMessage) || typeof(TMsg) == typeof(SystemMessage))
            {
                return true;
            }

            if (typeof(T).GetTypeInfo().IsAssignableFrom(typeof(TMsg).GetTypeInfo()))
            {
                return false;
            }

            return true;
        }

        public object ValidateMessageType(object message, ProcessId sender)
        {
            if (message == null)
            {
                throw new ProcessException($"Invalid message.  Null is not allowed for Process ({actor.Id}).", actor.Id.Path, sender.Path, null);
            }
            if (message is T || message is TerminatedMessage || message is UserControlMessage || message is SystemMessage)
            {
                return message;
            }

            if (message is string)
            {
                return JsonConvert.DeserializeObject<T>((string)message);
            }

            throw new ProcessException($"Invalid message-type ({message.GetType().Name}) for Process ({actor.Id}).  The Process accepts: ({typeof(T)})", actor.Id.Path, sender.Path, null);
        }
    }
}
