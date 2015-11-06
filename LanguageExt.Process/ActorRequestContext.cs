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
        public readonly Option<string> SessionId;

        public ActorRequestContext(
            ActorItem self,
            ProcessId sender,
            ActorItem parent,
            object currentMsg,
            ActorRequest currentRequest,
            ProcessFlags processFlags,
            Option<string> sessionId
            )
        {
            Self = self;
            Sender = sender;
            Parent = parent;
            CurrentMsg = currentMsg;
            CurrentRequest = currentRequest;
            ProcessFlags = processFlags;
            SessionId = sessionId;
        }

        public ActorRequestContext SetProcessFlags(ProcessFlags flags) =>
            new ActorRequestContext(
                Self,
                Sender,
                Parent,
                CurrentMsg,
                CurrentRequest,
                flags,
                SessionId
            );

        public ActorRequestContext SetCurrentRequest(ActorRequest currentRequest) =>
            new ActorRequestContext(
                Self,
                Sender,
                Parent,
                CurrentMsg,
                currentRequest,
                ProcessFlags, 
                SessionId
            );

        public ActorRequestContext SetCurrentMessage(object currentMsg) =>
            new ActorRequestContext(
                Self,
                Sender,
                Parent,
                currentMsg,
                CurrentRequest,
                ProcessFlags,
                SessionId
            );

        public ActorRequestContext SetSessionId(Option<string> sessionId) =>
            new ActorRequestContext(
                Self,
                Sender,
                Parent,
                CurrentMsg,
                CurrentRequest,
                ProcessFlags,
                sessionId
            );
    }
}
