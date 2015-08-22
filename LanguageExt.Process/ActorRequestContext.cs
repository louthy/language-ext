using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal class ActorRequestContext
    {
        public readonly ActorItem Self;
        public readonly ProcessId Sender;
        public readonly ActorItem Parent;
        public readonly object CurrentMsg;
        public readonly ActorRequest CurrentRequest;
        public readonly ProcessFlags ProcessFlags;

        public ActorRequestContext(
            ActorItem self,
            ProcessId sender,
            ActorItem parent,
            object currentMsg,
            ActorRequest currentRequest,
            ProcessFlags processFlags
            )
        {
            Self = self;
            Sender = sender;
            Parent = parent;
            CurrentMsg = currentMsg;
            CurrentRequest = currentRequest;
            ProcessFlags = processFlags;
        }

        public ActorRequestContext SetProcessFlags(ProcessFlags flags)
        {
            return new ActorRequestContext(
                Self,
                Sender,
                Parent,
                CurrentMsg,
                CurrentRequest,
                flags
            );
        }

        public ActorRequestContext SetCurrentRequest(ActorRequest currentRequest)
        {
            return new ActorRequestContext(
                Self,
                Sender,
                Parent,
                CurrentMsg,
                currentRequest,
                ProcessFlags
            );
        }

        public ActorRequestContext SetCurrentMessage(object currentMsg)
        {
            return new ActorRequestContext(
                Self,
                Sender,
                Parent,
                currentMsg,
                CurrentRequest,
                ProcessFlags
            );
        }
    }
}
