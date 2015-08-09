using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class AskActor
    {
        const int responseActors = 20;

        public static Tuple<long,Dictionary<long, AskActorReq>> Inbox(Tuple<long, Dictionary<long, AskActorReq>> state, object msg)
        {
            logInfo("AskActor.Inbox start");

            var reqId = state.Item1;
            var dict = state.Item2;

            if (msg is AskActorReq)
            {
                reqId++;

                var req = (AskActorReq)msg;

                logInfo("About to send ask request - reqId: " + reqId);
                tell(req.To, new ActorRequest(req.Message, req.To, Self, reqId));
                logInfo("Sent ask request - reqId: " + reqId);
                dict.Add(reqId, req);
            }
            else
            {
                var res = (ActorResponse)msg;
                if (dict.ContainsKey(res.RequestId))
                {
                    logInfo("Ask response has returned - reqId: " + reqId);
                    var req = dict[res.RequestId];
                    try
                    {
                        req.Complete(new AskActorRes(res.Message));
                    }
                    catch (Exception e)
                    {
                        req.Complete(new AskActorRes(e));
                        logSysErr(e);
                    }
                    finally
                    {
                        dict.Remove(res.RequestId);
                    }
                }
                else
                {
                    logWarn("Request ID doesn't exist: " + res.RequestId);
                }
            }

            logInfo("AskActor.Inbox done");

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
        public readonly Action<AskActorRes> Complete;
        public readonly ProcessId To;
        public readonly ProcessId ReplyTo;

        public AskActorReq(object msg, Action<AskActorRes> complete, ProcessId to, ProcessId replyTo)
        {
            Complete = complete;
            Message = msg;
            To = to;
            ReplyTo = replyTo;
        }

        public override string ToString() =>
            "Ask request from: " + ReplyTo + " to: " + To + " msg: " + Message;
    }

    internal class AskActorRes
    {
        public bool IsFaulted => Exception != null;
        public readonly Exception Exception;
        public readonly object Response;

        public AskActorRes(Exception exception)
        {
            Exception = exception;
        }
        public AskActorRes(object response)
        {
            Response = response;
        }
    }
}
