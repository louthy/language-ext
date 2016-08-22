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
using Newtonsoft.Json;
using LanguageExt.ActorSys;

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
        BlockingQueue<UserControlMessage> userInbox;
        BlockingQueue<SystemMessage> sysInbox;

        ICluster cluster;
        Actor<S, T> actor;
        ActorItem parent;
        int maxMailboxSize;
        object sync = new object();

        public Unit Startup(IActor process, ActorItem parent, Option<ICluster> cluster, int maxMailboxSize)
        {
            if (cluster.IsNone) throw new Exception("Remote inboxes not supported when there's no cluster");

            this.actor = (Actor<S, T>)process;
            this.parent = parent;
            this.cluster = cluster.IfNoneUnsafe(() => null);
            this.maxMailboxSize = maxMailboxSize == -1 
                ? ActorContext.System(actor.Id).Settings.GetProcessMailboxSize(actor.Id) 
                : maxMailboxSize;

            userInbox = new BlockingQueue<UserControlMessage>(this.maxMailboxSize);
            sysInbox = new BlockingQueue<SystemMessage>(this.maxMailboxSize);

            var obj = new ThreadObj { Actor = actor, Inbox = this, Parent = parent };
            userInbox.ReceiveAsync(obj, (state, msg) => ActorInboxCommon.UserMessageInbox(state.Actor, state.Inbox, msg, state.Parent));
            sysInbox.ReceiveAsync(obj, (state, msg) => ActorInboxCommon.SystemMessageInbox(state.Actor, state.Inbox, msg, state.Parent));


            SubscribeToSysInboxChannel();
            SubscribeToUserInboxChannel();

            return unit;
        }

        class ThreadObj
        {
            public Actor<S, T> Actor;
            public ActorInboxDual<S, T> Inbox;
            public ActorItem Parent;
        }


        void SubscribeToSysInboxChannel()
        {
            cluster.UnsubscribeChannel(ActorInboxCommon.ClusterSystemInboxNotifyKey(actor.Id));
            cluster.SubscribeToChannel<string>(ActorInboxCommon.ClusterSystemInboxNotifyKey(actor.Id)).Subscribe(
                msg =>
                {
                    if (sysInbox.Count == 0)
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
                    if (userInbox.Count == 0)
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
            get
            {
                return userInbox.IsPaused;
            }
            private set
            {
                if (value)
                {
                    userInbox.Pause();
                    cluster.UnsubscribeChannel(ActorInboxCommon.ClusterUserInboxNotifyKey(actor.Id));
                }
                else
                {
                    // Wake up the user inbox to process any messages that have
                    // been waiting patiently.
                    SubscribeToUserInboxChannel();
                    userInbox.UnPause();
                }
            }
        }

        public Unit Pause()
        {
            lock(sync)
            {
                if (!userInbox.IsPaused)
                {
                    IsPaused = true;
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
                }
            }
            return unit;
        }

        public Unit Ask(object message, ProcessId sender)
        {
            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message)
                                .IfSome(msg =>
                                {
                                    if (IsPaused)
                                    {
                                        new ActorDispatchRemote(actor.Id, cluster, ActorContext.SessionId, false).Ask(message, sender);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            userInbox.Post(msg);
                                        }
                                        catch (QueueFullException)
                                        {
                                            throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
                                        }
                                    }
                                });
            }
            return unit;
        }

        public Unit Tell(object message, ProcessId sender)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message)
                                .IfSome(msg =>
                                {
                                    if (IsPaused)
                                    {
                                        new ActorDispatchRemote(actor.Id, cluster, ActorContext.SessionId, false).Tell(message, sender, Message.TagSpec.User);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            userInbox.Post(msg);
                                        }
                                        catch (QueueFullException)
                                        {
                                            throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
                                        }
                                    }
                                });
            }
            return unit;
        }

        public Unit TellSystem(SystemMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (sysInbox != null)
            {
                try
                {
                    sysInbox.Post(message);
                }
                catch (QueueFullException)
                {
                    throw new ProcessInboxFullException(actor.Id, MailboxSize, "system");
                }
            }
            return unit;
        }

        public Unit TellUserControl(UserControlMessage msg)
        {
            if (msg == null) throw new ArgumentNullException(nameof(msg));

            if (userInbox != null)
            {
                if (IsPaused)
                {
                    new ActorDispatchRemote(actor.Id, cluster, ActorContext.SessionId, false).TellUserControl(msg, ProcessId.None);
                }
                else
                {
                    try
                    {
                        userInbox.Post(msg);
                    }
                    catch(QueueFullException)
                    {
                        throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
                    }
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
            userInbox?.Cancel();
            sysInbox?.Cancel();
            cluster?.UnsubscribeChannel(ActorInboxCommon.ClusterUserInboxNotifyKey(actor.Id));
            cluster?.UnsubscribeChannel(ActorInboxCommon.ClusterSystemInboxNotifyKey(actor.Id));
            userInbox = null;
            sysInbox = null;
            cluster = null;
        }

        public void CheckRemoteInbox(string key, ICluster cluster, ProcessId self, BlockingQueue<SystemMessage> sysInbox, BlockingQueue<UserControlMessage> userInbox, bool pausable)
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

        /// <summary>
        /// Number of unprocessed items
        /// </summary>
        public int Count =>
            userInbox?.Count ?? 0;

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
            return res.IfLeft(() => null);
        }
    }
}
