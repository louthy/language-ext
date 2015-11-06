using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

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
            this.maxMailboxSize = maxMailboxSize < 0
                ? ActorConfig.Default.MaxMailboxSize
                : maxMailboxSize;

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
                : ActorContext.Self.IsValid
                    ? ActorContext.Self
                    : ProcessId.NoSender;

        public Unit Ask(object message, ProcessId sender)
        {
            if (Count >= maxMailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, maxMailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message).IfSome(msg => userInbox.Post(msg));
            }
            return unit;
        }

        public Unit Tell(object message, ProcessId sender)
        {
            if (Count >= maxMailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, maxMailboxSize, "user");
            }

            if (userInbox != null)
            {
                ActorInboxCommon.PreProcessMessage<T>(sender, actor.Id, message).IfSome(msg => userInbox.Post(msg));
            }
            return unit;
        }

        public Unit TellUserControl(UserControlMessage message)
        {
            if (Count >= maxMailboxSize)
            {
                throw new ProcessInboxFullException(actor.Id, maxMailboxSize, "user");
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
                    var qmsg = userQueue.Peek();
                    userQueue = userQueue.Dequeue();
                    ActorInboxCommon.UserMessageInbox(actor, inbox, qmsg, parent);
                }
                var directive = ActorInboxCommon.UserMessageInbox(actor, inbox, msg, parent);

                if (directive == InboxDirective.PushToFrontOfQueue)
                {
                    var newQueue = Que<UserControlMessage>.Empty;

                    while (userQueue.Count > 0)
                    {
                        newQueue = newQueue.Enqueue(userQueue.Peek());
                        userQueue.Dequeue();
                    }

                    userQueue = newQueue;
                }
            }
            return InboxDirective.Default;
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
            userInbox.CurrentQueueLength + userQueue.Count;
    }
}
