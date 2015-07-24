using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class AskActor
    {
        const int responseActors = 20;

        public static Unit Inbox(AskActorReq req)
        {
            tell(req.To, new ActorRequest(req.Message, req.To, Self, req.Subject));
            return unit;
        }
    }

    internal class AskActorReq
    {
        public readonly object Message;
        public readonly Subject<object> Subject;
        public readonly ProcessId To;
        public readonly ProcessId ReplyTo;

        public AskActorReq(object msg, Subject<object> subject, ProcessId to, ProcessId replyTo)
        {
            Message = msg;
            Subject = subject;
            To = to;
            ReplyTo = replyTo;
        }
    }
}
