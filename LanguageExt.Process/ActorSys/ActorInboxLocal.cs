﻿using Microsoft.FSharp.Control;
using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using LanguageExt.Trans;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using Newtonsoft.Json;

namespace LanguageExt
{
    class ActorInboxLocal<S,T> : IActorInbox, ILocalActorInbox
    {
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        Actor<S, T> actor;
        ActorItem parent;
        Option<ICluster> cluster;
        Que<UserControlMessage> userQueue = Que<UserControlMessage>.Empty;
        int maxMailboxSize;

        public Unit Startup(IActor process, ActorItem parent, Option<ICluster> cluster, int maxMailboxSize)
        {
            if (Active)
            {
                Shutdown();
            }
            this.cluster = cluster;
            this.parent = parent;
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;

            this.maxMailboxSize = maxMailboxSize;

            userInbox = StartMailbox<UserControlMessage>(actor, tokenSource.Token, StatefulUserInbox);
            sysInbox = StartMailbox<SystemMessage>(actor, tokenSource.Token, ActorInboxCommon.SystemMessageInbox);
            return unit;
        }

        public Unit Shutdown()
        {
            var ts = tokenSource;
            if (ts != null)
            {
                ts.Cancel();
                ts.Dispose();

                tokenSource = null;
                userInbox = null;
                sysInbox = null;
            }
            return unit;
        }

        string ClusterKey => actor.Id.Path + "-inbox";
        string ClusterNotifyKey => actor.Id.Path + "-inbox-notify";

        public bool Active => 
            tokenSource != null;

        ProcessId GetSender(ProcessId sender) =>
            sender = sender.IsValid
                ? sender
                : Self.IsValid
                    ? Self
                    : ProcessId.NoSender;

        public Unit Ask(object message, ProcessId sender)
        {
            if (Count >= MailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message).IfSome(msg => userInbox.Post(msg));
            }
            return unit;
        }

        public Unit Tell(object message, ProcessId sender)
        {
            if (Count >= MailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message).IfSome(msg => userInbox.Post(msg));
            }
            return unit;
        }

        public Unit TellUserControl(UserControlMessage message)
        {
            if (Count >= MailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, MailboxSize, "user");
            }

            if (userInbox != null)
            {
                if (message == null) throw new ArgumentNullException(nameof(message));
                userInbox.Post(message);
            }
            return unit;
        }

        public Unit TellSystem(SystemMessage message)
        {
            if (sysInbox != null)
            {
                if (sysInbox.CurrentQueueLength >= MailboxSize)
                {
                    throw new ProcessInboxFullException(actor.Id, MailboxSize, "system");
                }

                if (message == null) throw new ArgumentNullException(nameof(message));
                sysInbox.Post(message);
            }
            return unit;
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
            IsPaused = true;
            return unit;
        }

        public Unit Unpause()
        {
            IsPaused = false;

            // Wake up the user inbox to process any messages that have
            // been waiting patiently.
            TellUserControl(UserControlMessage.Null);

            return unit;
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

        FSharpMailboxProcessor<TMsg> StartMailbox<TMsg>(Actor<S, T> actor, CancellationToken cancelToken, Func<Actor<S, T>, IActorInbox, TMsg, ActorItem, InboxDirective> handler) where TMsg : Message =>
            ActorInboxCommon.Mailbox<TMsg>(cancelToken, msg =>
                  handler(actor, this, msg, parent));

        public void Dispose()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = null;
        }

        /// <summary>
        /// Number of unprocessed items
        /// </summary>
        public int Count =>
            ((userInbox?.CurrentQueueLength) + (userQueue?.Count)).GetValueOrDefault();

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
                throw new ProcessException($"{err } for Process ({actor.Id}).", actor.Id.Path, sender.Path, null);
            });
            return res.LiftUnsafe();
        }
    }
}
