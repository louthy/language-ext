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
        Que<UserControlMessage> userQueue = Que<UserControlMessage>.Empty;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        Actor<S, T> actor;
        ActorItem parent;
        int maxMailboxSize;
        object sync = new object();

        public Unit Startup(IActor process, ActorItem parent, Option<ICluster> cluster, int maxMailboxSize)
        {
            if (cluster.IsNone) throw new Exception("Remote inboxes not supported when there's no cluster");
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.parent = parent;
            this.cluster = cluster.LiftUnsafe();

            this.maxMailboxSize = maxMailboxSize;

            userInbox = StartMailbox<UserControlMessage>(actor, ActorInboxCommon.ClusterUserInboxKey(actor.Id), tokenSource.Token, StatefulUserInbox, true);
            sysInbox = StartMailbox<SystemMessage>(actor, ActorInboxCommon.ClusterSystemInboxKey(actor.Id), tokenSource.Token, ActorInboxCommon.SystemMessageInbox, false);

            SubscribeToSysInboxChannel();
            SubscribeToUserInboxChannel();

            return unit;
        }

        void SubscribeToSysInboxChannel()
        {
            cluster.UnsubscribeChannel(ActorInboxCommon.ClusterSystemInboxNotifyKey(actor.Id));
            cluster.SubscribeToChannel<string>(ActorInboxCommon.ClusterSystemInboxNotifyKey(actor.Id)).Subscribe(
                msg =>
                {
                    if (sysInbox.CurrentQueueLength == 0)
                    {
                        CheckRemoteInbox(ActorInboxCommon.ClusterSystemInboxKey(actor.Id), cluster, actor.Id, sysInbox, userInbox, false);
                    }
                });
            cluster.PublishToChannel(ActorInboxCommon.ClusterSystemInboxNotifyKey(actor.Id), Guid.NewGuid().ToString());
        }

        void SubscribeToUserInboxChannel()
        {
            cluster.UnsubscribeChannel(ActorInboxCommon.ClusterUserInboxNotifyKey(actor.Id));
            cluster.SubscribeToChannel<string>(ActorInboxCommon.ClusterUserInboxNotifyKey(actor.Id)).Subscribe(
                msg =>
                {
                    if (userInbox.CurrentQueueLength == 0)
                    {
                        CheckRemoteInbox(ActorInboxCommon.ClusterUserInboxKey(actor.Id), cluster, actor.Id, sysInbox, userInbox, true);
                    }
                });
            cluster.PublishToChannel(ActorInboxCommon.ClusterUserInboxNotifyKey(actor.Id), Guid.NewGuid().ToString());
        }

        int MailboxSize =>
            maxMailboxSize < 0
                ? ActorContext.System(actor.Id).Settings.GetProcessMailboxSize(actor.Id)
                : maxMailboxSize;

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
                    cluster.UnsubscribeChannel(ActorInboxCommon.ClusterUserInboxNotifyKey(actor.Id));
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
            if (userInbox.CurrentQueueLength >= MailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message)
                                .IfSome(msg =>
                                {
                                    if (IsPaused)
                                    {
                                        new ActorDispatchRemote(actor.Id, cluster, ActorContext.SessionId).Ask(message, sender);
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
            if (userInbox.CurrentQueueLength >= MailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message)
                                .IfSome(msg =>
                                {
                                    if (IsPaused)
                                    {
                                        new ActorDispatchRemote(actor.Id, cluster, ActorContext.SessionId).Tell(message, sender, Message.TagSpec.User);
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
            if (sysInbox.CurrentQueueLength >= MailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, MailboxSize, "system");
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
            if (userInbox.CurrentQueueLength >= MailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
            }

            if (userInbox != null)
            {
                if (msg == null) throw new ArgumentNullException(nameof(msg));

                if (IsPaused)
                {
                    new ActorDispatchRemote(actor.Id, cluster, ActorContext.SessionId).TellUserControl(msg, ProcessId.None);
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

            cluster?.UnsubscribeChannel(ActorInboxCommon.ClusterUserInboxNotifyKey(actor.Id));
            cluster?.UnsubscribeChannel(ActorInboxCommon.ClusterSystemInboxNotifyKey(actor.Id));
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

        InboxDirective StatefulUserInbox(Actor<S, T> actor, IActorInbox inbox, UserControlMessage msg, ActorItem parent)
        {
            if (IsPaused)
            {
                userQueue = userQueue.Enqueue(msg);
            }
            else
            {
                while (userQueue.Count > 0)
                {
                    // Don't process messages if we've been paused
                    if (IsPaused) return InboxDirective.Pause;

                    var qmsg = userQueue.Peek();
                    userQueue = userQueue.Dequeue();
                    ProcessInboxDirective(ActorInboxCommon.UserMessageInbox(actor, inbox, qmsg, parent), qmsg);
                }

                if (IsPaused)
                {
                    // Don't process the message if we've been paused
                    userQueue = userQueue.Enqueue(msg);
                    return InboxDirective.Pause;
                }

                return ProcessInboxDirective(ActorInboxCommon.UserMessageInbox(actor, inbox, msg, parent), msg);
            }
            return InboxDirective.Default;
        }

        InboxDirective ProcessInboxDirective(InboxDirective directive, UserControlMessage msg)
        {
            IsPaused = (directive & InboxDirective.Pause) != 0;

            if ((directive & InboxDirective.PushToFrontOfQueue) != 0)
            {
                var newQueue = Que<UserControlMessage>.Empty.Enqueue(msg);

                while (userQueue.Count > 0)
                {
                    newQueue = newQueue.Enqueue(userQueue.Peek());
                    userQueue = userQueue.Dequeue();
                }

                userQueue = newQueue;
            }
            return directive;
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
                    tell(ActorContext.System(actor.Id).DeadLetters, DeadLetter.create(ActorContext.Request.Sender, actor.Id, e, "Remote message inbox.", msg));
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

        public Either<string, bool> HasStateTypeOf<TState>() =>
            TypeHelper.HasStateTypeOf(
                typeof(TState),
                typeof(S).Cons(
                    typeof(S).GetTypeInfo().ImplementedInterfaces
                    ).ToArray()
            );

        public Either<string, bool> CanAcceptMessageType<TMsg>() =>
            TypeHelper.IsMessageValidForProcess(typeof(TMsg), new[] { typeof(T) });

        public object ValidateMessageType(object message, ProcessId sender)
        {
            var res = TypeHelper.IsMessageValidForProcess(message, new[] { typeof(T) });
            res.IfLeft(err =>
            {
                throw new ProcessException($"{err} for Process ({actor.Id}).", actor.Id.Path, sender.Path, null);
            });
            return res.LiftUnsafe();
        }
    }
}
