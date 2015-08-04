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
    internal class ActorInboxLocal<S,T> : IActorInbox, ILocalActorInbox
    {
        ProcessId supervisor;
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        Actor<S, T> actor;
        Option<ICluster> cluster;

        public Unit Startup(IActor process, ProcessId supervisor, Option<ICluster> cluster)
        {
            if (Active)
            {
                Shutdown();
            }
            this.cluster = cluster;
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.supervisor = supervisor;
            userInbox = StartMailbox<UserControlMessage>(actor, tokenSource.Token, ActorInboxCommon.UserMessageInbox);
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
                supervisor = ProcessId.None;
            }
            return unit;
        }

        string ClusterKey => actor.Id.Path + "-inbox";
        string ClusterNotifyKey => actor.Id.Path + "-inbox-notify";

        public bool Active => 
            tokenSource != null;

        private ProcessId GetSender(ProcessId sender) =>
            sender = sender.IsValid
                ? sender
                : ActorContext.Self.IsValid
                    ? ActorContext.Self
                    : ProcessId.NoSender;

        public Unit Ask(object message, ProcessId sender)
        {
            if (userInbox != null)
            {
                if (message == null)
                {
                    return Process.tell(ActorContext.DeadLetters, DeadLetter.create(sender, actor.Id, "Message is null for ask (expected " + typeof(T) + ")", message));
                }
                if (!typeof(ActorRequest).IsAssignableFrom(message.GetType()))
                {
                    return Process.tell(ActorContext.DeadLetters, DeadLetter.create(sender, actor.Id, "Invalid message type for ask (expected ActorRequest)", message));
                }

                var req = (ActorRequest)message;
                if (!typeof(T).IsAssignableFrom(req.Message.GetType()))
                {
                    return Process.tell(ActorContext.DeadLetters, DeadLetter.create(sender, actor.Id, "Invalid message type for ask (expected "+typeof(T)+")", message));
                }
                userInbox.Post((UserControlMessage)message);
            }
            return unit;
        }

        public Unit Tell(object message, ProcessId sender)
        {
            if (userInbox != null)
            {
                if (message is ActorRequest)
                {
                    return Ask(message, sender);
                }
                if (message == null)
                {
                    return Process.tell(ActorContext.DeadLetters, DeadLetter.create(sender, actor.Id, "Message is null for tell (expected " + typeof(T) + ")", message));
                }
                if (!typeof(T).IsAssignableFrom(message.GetType()))
                {
                    return Process.tell(ActorContext.DeadLetters, DeadLetter.create(sender, actor.Id, "Invalid message type for tell (expected " + typeof(T) + ")", message));
                }
                var enqItem = new UserMessage(message, sender, sender);
                userInbox.Post(enqItem);
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

        public Unit TellSystem(SystemMessage message)
        {
            if (sysInbox != null)
            {
                if (message == null) throw new ArgumentNullException(nameof(message));
                sysInbox.Post(message);
            }
            return unit;
        }

        private FSharpMailboxProcessor<TMsg> StartMailbox<TMsg>(Actor<S, T> actor, CancellationToken cancelToken, Action<Actor<S, T>, TMsg> handler) where TMsg : Message =>
            ActorInboxCommon.Mailbox<S, T, TMsg>(cluster, actor.Flags, cancelToken, msg =>
                  handler(actor,msg));

        public void Dispose()
        {
            var ts = tokenSource;
            if (ts != null) ts.Dispose();
        }
    }
}
