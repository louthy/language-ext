using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal class ActorRequest : UserControlMessage
    {
        public readonly long RequestId;
        public readonly object Message;
        public readonly ProcessId To;
        public readonly ProcessId ReplyTo;

        public override Type MessageType => Type.User;
        public override UserControlMessageTag Tag => UserControlMessageTag.UserAsk;

        public ActorRequest(long requestId, object msg, ProcessId to, ProcessId replyTo)
        {
            RequestId = requestId;
            Message = msg;
            To = to;
            ReplyTo = replyTo;
        }
    }

    internal class ActorResponse : UserControlMessage
    {
        public readonly long RequestId;
        public readonly object Message;

        public override Type MessageType => Type.User;
        public override UserControlMessageTag Tag => UserControlMessageTag.UserReply;

        public ActorResponse(long requestId, object msg)
        {
            RequestId = requestId;
            Message = msg;
        }
    }
}
