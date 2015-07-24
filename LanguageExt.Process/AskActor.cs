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

        public static Tuple<long,Dictionary<long, AskActorReq>> Inbox(Tuple<long, Dictionary<long, AskActorReq>> state, object msg)
        {
            var reqId = state.Item1;
            var dict = state.Item2;

            if (msg is AskActorReq)
            {
                reqId++;

                var req = (AskActorReq)msg;
                tell(req.To, new ActorRequest(req.Message, req.To, Self, reqId));
                dict.Add(reqId, req);
            }
            else
            {
                var res = (ActorResponse)msg;
                var req = dict[res.RequestId];
                try
                {
                    req.Subject.OnNext(res.Message);
                    req.Subject.OnCompleted();
                }
                catch (Exception e)
                {
                    logSysErr(e);
                }
                dict.Remove(res.RequestId);
            }
            return new Tuple<long, Dictionary<long, AskActorReq>>(reqId, dict);
        }

        public static Tuple<long, Dictionary<long, AskActorReq>> Setup()
        {
            return Tuple(0L, new Dictionary<long, AskActorReq>());
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
