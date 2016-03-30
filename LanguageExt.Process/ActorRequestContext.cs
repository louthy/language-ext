using LanguageExt.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    class ActorRequestContext
    {
        public readonly ActorItem Self;
        public readonly ProcessId Sender;
        public readonly ActorItem Parent;
        public readonly object CurrentMsg;
        public readonly ActorRequest CurrentRequest;
        public readonly ProcessFlags ProcessFlags;
        public readonly ProcessOpTransaction Ops;

        public ActorRequestContext(
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
        }

        public ActorRequestContext SetProcessFlags(ProcessFlags flags) =>
            new ActorRequestContext(
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
                Self,
                Sender,
                Parent,
                currentMsg,
                CurrentRequest,
                ProcessFlags,
                Ops
            );

        public ActorRequestContext SetOps(ProcessOpTransaction ops) =>
            new ActorRequestContext(
                Self,
                Sender,
                Parent,
                CurrentMsg,
                CurrentRequest,
                ProcessFlags,
                ops
            );

    }
}
