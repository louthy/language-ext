using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;
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
            var reqId = state.Item1;
            var dict = state.Item2;

            if (msg is AskActorReq)
            {
                reqId++;

                var req = (AskActorReq)msg;
                ActorContext.System(req.To).Ask(req.To, new ActorRequest(req.Message, req.To, Self, reqId), Self);
                dict.Add(reqId, req);
            }
            else
            {
                var res = (ActorResponse)msg;
                if (dict.ContainsKey(res.RequestId))
                {
                    var req = dict[res.RequestId];
                    try
                    {
                        if (res.IsFaulted)
                        {
                            Exception ex = null;

                            // Let's be reeeally safe here and do everything we can to get some valid information out of
                            // the response to report to the process doing the 'ask'.  
                            try
                            {
                                var msgtype = Type.GetType(res.ResponseMessageType);
                                if (msgtype == res.Message.GetType() && typeof(Exception).GetTypeInfo().IsAssignableFrom(msgtype.GetTypeInfo()))
                                {
                                    // Type is fine, just return it as an error
                                    ex = (Exception)res.Message;
                                }
                                else
                                {
                                    if (res.Message is string)
                                    {
                                        ex = (Exception)JsonConvert.DeserializeObject(res.Message.ToString(), msgtype);
                                    }
                                    else
                                    {
                                        ex = (Exception)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(res.Message), msgtype);
                                    }
                                }
                            }
                            catch
                            {
                                ex = new Exception(res.Message == null ? $"An unknown error was thrown by {req.To}" : res.Message.ToString());
                            }

                            req.Complete(new AskActorRes(new ProcessException($"Process issue: {ex.Message}", req.To.Path, req.ReplyTo.Path, ex)));
                        }
                        else
                        {
                            req.Complete(new AskActorRes(res.Message));
                        }
                    }
                    catch (Exception e)
                    {
                        req.Complete(new AskActorRes(new ProcessException($"Process issue: {e.Message}", req.To.Path, req.ReplyTo.Path, e)));
                        logSysErr(e);
                    }
                    finally
                    {
                        dict.Remove(res.RequestId);
                    }
                }
                else
                {
                    logWarn($"Request ID doesn't exist: {res.RequestId}");
                }
            }

            return new Tuple<long, Dictionary<long, AskActorReq>>(reqId, dict);
        }

        public static Tuple<long, Dictionary<long, AskActorReq>> Setup()
        {
            return Tuple(1L, new Dictionary<long, AskActorReq>());
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
            $"Ask request from: {ReplyTo} to: {To} msg: {Message}";
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
