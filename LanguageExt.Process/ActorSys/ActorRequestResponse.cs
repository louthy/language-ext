using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    class ActorRequest : UserControlMessage
    {
        public readonly object Message;
        public readonly ProcessId To;
        public readonly ProcessId ReplyTo;
        public readonly long RequestId;

        public override Type MessageType => Type.User;
        public override TagSpec Tag      => TagSpec.UserAsk;

        [JsonConstructor]
        public ActorRequest(object message, ProcessId to, ProcessId replyTo, long requestId)
        {
            Message = message;
            To = to;
            ReplyTo = replyTo;
            RequestId = requestId;
        }

        public ActorRequest SetSystem(SystemName sys) =>
            new ActorRequest(Message, To.SetSystem(sys), ReplyTo.SetSystem(sys), RequestId);
    }

    internal class ActorResponse : UserControlMessage
    {
        public readonly ProcessId ReplyTo;
        public readonly object Message;
        public readonly ProcessId ReplyFrom;
        public readonly long RequestId;
        public readonly bool IsFaulted;
        public readonly string ResponseMessageType;

        public override Type MessageType => Type.User;
        public override TagSpec Tag      => TagSpec.UserReply;

        [JsonConstructor]
        public ActorResponse(object message, string responseMessageType, ProcessId replyTo, ProcessId replyFrom, long requestId, bool isFaulted = false)
        {
            Message = message;
            ResponseMessageType = responseMessageType;
            ReplyTo = replyTo;
            ReplyFrom = replyFrom;
            RequestId = requestId;
            IsFaulted = isFaulted;
        }

        public ActorResponse SetSystem(SystemName sys) =>
            new ActorResponse(Message, ResponseMessageType, ReplyTo.SetSystem(sys), ReplyFrom.SetSystem(sys), RequestId, IsFaulted);
    }
}
