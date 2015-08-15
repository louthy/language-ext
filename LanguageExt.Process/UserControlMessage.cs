using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public abstract class UserControlMessage : Message
    {
        public override Type MessageType => Type.UserControl;

        public static UserControlMessage Shutdown => new UserControlShutdownMessage();
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
    }

    public class UserControlShutdownMessage : UserControlMessage
    {
        public override Type MessageType => Type.UserControl;
        public override TagSpec Tag      => TagSpec.Shutdown;
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
    }
}
