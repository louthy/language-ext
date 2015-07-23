using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class AskActor
    {
        public static Tuple<long, Dictionary<long, AskActorReq>> Inbox(Tuple<long, Dictionary<long, AskActorReq>> state, object msg)
        {
            var currentId = state.Item1;
            var requests = state.Item2;

            if (msg is AskActorReq)
            {
                var req = msg as AskActorReq;

                currentId++;
                requests.Add(currentId, req);
                tell(req.To, new ActorRequest(currentId, req.Message, req.To, Self), req.ReplyTo);
            }
            else if (msg is ActorResponse)
            {
                var res = msg as ActorResponse;
                var req = requests[res.RequestId];
                requests.Remove(res.RequestId);
                req.Subject.OnNext(res.Message);
                req.Subject.OnCompleted();
            }
            return Tuple(currentId, requests);
        }

        public static Tuple<long, Dictionary<long, AskActorReq>> Setup()
        {
            return new Tuple<long, Dictionary<long, AskActorReq>>(0, new Dictionary<long, AskActorReq>());
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
