using LanguageExt.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    class ActorRequestContext
    {
        public readonly ActorItem Self;
        public readonly ProcessId Sender;
        public readonly ActorItem Parent;
        public readonly ActorSystem System;

        public ProcessOpTransaction Ops;
        public object CurrentMsg;
        public ActorRequest CurrentRequest;
        public ProcessFlags ProcessFlags;

        public ActorRequestContext(
            ActorSystem system,
            ActorItem self,
            ProcessId sender,
            ActorItem parent,
            object currentMsg,
            ActorRequest currentRequest,
            ProcessFlags processFlags,
            ProcessOpTransaction ops
            )
        {
            Self = self;
            Sender = sender;
            Parent = parent;
            CurrentMsg = currentMsg;
            CurrentRequest = currentRequest;
            ProcessFlags = processFlags;
            Ops = ops;
            System = system;
        }

        public ActorRequestContext SetProcessFlags(ProcessFlags flags) =>
            new ActorRequestContext(
                System,
                Self,
                Sender,
                Parent,
                CurrentMsg,
                CurrentRequest,
                flags,
                Ops
            );

        public ActorRequestContext SetCurrentRequest(ActorRequest currentRequest) =>
            new ActorRequestContext(
                System,
                Self,
                Sender,
                Parent,
                CurrentMsg,
                currentRequest,
                ProcessFlags,
                Ops
            );

        public ActorRequestContext SetCurrentMessage(object currentMsg) =>
            new ActorRequestContext(
                System,
                Self,
                Sender,
                Parent,
                currentMsg,
                CurrentRequest,
                ProcessFlags,
                Ops
            );

        public void SetOps(ProcessOpTransaction ops)
        {
            Ops = ops;
        }


        /// <summary>
        /// Run the operations that affect the settings and sending of tells
        /// in the order which they occured in the actor
        /// </summary>
        public Unit RunOps()
        {
            if (Ops != null)
            {
                while (Ops.Ops.Count > 0)
                {
                    var ops = Ops;
                    Ops = ProcessOpTransaction.Start(Ops.ProcessId);
                    ops.Run();
                }
            }
            return unit;
        }

        public Map<string, ProcessId> Children =>
            Self.Actor.Children.Map(c => c.Actor.Id);
    }
}
