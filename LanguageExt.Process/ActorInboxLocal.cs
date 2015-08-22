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
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        Actor<S, T> actor;
        ActorItem parent;
        Option<ICluster> cluster;
        int version;

        public Unit Startup(IActor process, ActorItem parent, Option<ICluster> cluster, int version)
        {
            if (Active)
            {
                Shutdown();
            }
            this.cluster = cluster;
            this.parent = parent;
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.version = version;
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

        private FSharpMailboxProcessor<TMsg> StartMailbox<TMsg>(Actor<S, T> actor, CancellationToken cancelToken, Action<Actor<S, T>, IActorInbox, TMsg, ActorItem> handler) where TMsg : Message =>
            ActorInboxCommon.Mailbox<S, T, TMsg>(cluster, actor.Flags, cancelToken, msg =>
                  handler(actor, this, msg, parent));

        public void Dispose()
        {
            var ts = tokenSource;
            if (ts != null) ts.Dispose();
        }
    }
}
