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
        public readonly Subject<object> Subject;

        public override Type MessageType => Type.User;
        public override UserControlMessageTag Tag => UserControlMessageTag.UserAsk;

        public ActorRequest(object msg, ProcessId to, ProcessId replyTo, Subject<object> subject)
        {
            Message = msg;
            To = to;
            ReplyTo = replyTo;
            Subject = subject;
        }
    }

    internal class ActorResponse
    {
        public readonly object Message;
        public readonly Subject<object> Subject;
        public readonly ProcessId ReplyTo;
        public readonly ProcessId ReplyFrom;

        public ActorResponse(ProcessId replyToId, object msg, Subject<object> subject, ProcessId replyFromId)
        {
            Message = msg;
            Subject = subject;
            ReplyTo = replyToId;
            ReplyFrom = replyFromId;
        }
    }
}
