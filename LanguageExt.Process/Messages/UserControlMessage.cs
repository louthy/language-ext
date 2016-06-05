using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    public abstract class UserControlMessage : Message
    {
        public override Type MessageType => Type.UserControl;
        public readonly static UserControlMessage GetChildren = new GetChildrenMessage();
        public readonly static UserControlMessage Null = new UserControlNullMessage();
    }

    class UserControlNullMessage : UserControlMessage
    {
        public override TagSpec Tag => TagSpec.Null;
    }

    class GetChildrenMessage : UserControlMessage
    {
        public override TagSpec Tag => TagSpec.GetChildren;
        public override string ToString() => "GetChildren";
    }

    public class UserMessage : UserControlMessage
    {
        public override Type MessageType => Type.User;
        public override TagSpec Tag      => TagSpec.User;

        public UserMessage(object message, ProcessId sender, ProcessId replyTo)
        {
            Content = message;
            Sender = sender;
            ReplyTo = replyTo;
        }

        public ProcessId Sender { get; }
        public ProcessId ReplyTo { get; }
        public object Content { get; }

        public UserMessage SetSystem(SystemName sys) =>
            new UserMessage(Content, Sender.SetSystem(sys), ReplyTo.SetSystem(sys));
    }

    public class TerminatedMessage : UserControlMessage
    {
        public override Type MessageType => Type.UserControl;
        public override TagSpec Tag      => TagSpec.UserTerminated;

        public ProcessId Id;

        [JsonConstructor]
        public TerminatedMessage(ProcessId id)
        {
            Id = id;
        }

        public TerminatedMessage SetSystem(SystemName sys) =>
            new TerminatedMessage(Id.SetSystem(sys));
    }

    public class RemoteMessageDTO
    {
        public int Type;
        public int Tag;
        public string Exception;
        public string Child;
        public string To;
        public string Sender;
        public string ReplyTo;
        public long RequestId;
        public string ContentType;
        public string Content;
        public Guid MessageId;

        internal static RemoteMessageDTO Create(object message, ProcessId to, ProcessId sender, Message.Type type, Message.TagSpec tag) =>
            map(message as ActorRequest, req =>
                req == null
                    ? map(message as ActorResponse, res =>
                        res == null
                            ? CreateMessage(message, to, sender, type, tag)
                            : CreateResponse(res, to, sender))
                    : CreateRequest(req, to, sender));

        internal static RemoteMessageDTO CreateMessage(object message, ProcessId to, ProcessId sender, Message.Type type, Message.TagSpec tag) =>
            new RemoteMessageDTO
            {
                Type        = (int)type,
                Tag         = (int)tag,
                To          = to.ToString(),
                RequestId   = -1,
                MessageId   = Guid.NewGuid(),
                Sender      = sender.ToString(),
                ReplyTo     = sender.ToString(),
                ContentType = message == null
                                ? null
                                : message.GetType().AssemblyQualifiedName,
                Content     = message == null
                                ? null
                                : JsonConvert.SerializeObject(message, ActorSystemConfig.Default.JsonSerializerSettings)
            };

        internal static RemoteMessageDTO CreateRequest(ActorRequest req, ProcessId to, ProcessId sender) =>
            new RemoteMessageDTO
            {
                Type        = (int)Message.Type.User,
                Tag         = (int)Message.TagSpec.UserAsk,
                Child       = null,
                Exception   = null,
                To          = to.ToString(),
                RequestId   = req.RequestId,
                MessageId   = Guid.NewGuid(),
                Sender      = sender.ToString(),
                ReplyTo     = req.ReplyTo.ToString(),
                ContentType = req.Message.GetType().AssemblyQualifiedName,
                Content     = JsonConvert.SerializeObject(req.Message, ActorSystemConfig.Default.JsonSerializerSettings)
            };

        internal static RemoteMessageDTO CreateResponse(ActorResponse res, ProcessId to, ProcessId sender) =>
            new RemoteMessageDTO
            {
                Type        = (int)Message.Type.User,
                Tag         = (int)Message.TagSpec.UserReply,
                Child       = null,
                Exception   = res.IsFaulted
                                ? "RESPERR"
                                : null,
                To          = to.ToString(),
                RequestId   = res.RequestId,
                MessageId   = Guid.NewGuid(),
                Sender      = res.ReplyFrom.ToString(),
                ReplyTo     = res.ReplyTo.ToString(),
                ContentType = res.Message.GetType().AssemblyQualifiedName,
                Content     = JsonConvert.SerializeObject(res.Message, ActorSystemConfig.Default.JsonSerializerSettings)
            };
    }
}
