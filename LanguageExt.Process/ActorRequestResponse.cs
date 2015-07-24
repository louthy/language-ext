using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal class ActorRequest : UserControlMessage
    {
        public readonly object Message;
        public readonly ProcessId To;
        public readonly ProcessId ReplyTo;
        public readonly long RequestId;

        public override Type MessageType          => Type.User;
        public override UserControlMessageTag Tag => UserControlMessageTag.UserAsk;

        [JsonConstructor]
        public ActorRequest(object message, ProcessId to, ProcessId replyTo, long requestId)
        {
            Message = message;
            To = to;
            ReplyTo = replyTo;
            RequestId = requestId;
        }
    }

    internal class ActorResponse
    {
        public readonly ProcessId ReplyTo;
        public readonly object Message;
        public readonly ProcessId ReplyFrom;
        public readonly long RequestId;

        [JsonConstructor]
        public ActorResponse(ProcessId replyTo, object message, ProcessId replyFrom, long requestId)
        {
            Message = message;
            ReplyTo = replyTo;
            ReplyFrom = replyFrom;
            RequestId = requestId;
        }
    }
}
