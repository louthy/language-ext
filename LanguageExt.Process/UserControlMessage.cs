using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal enum UserControlMessageTag
    {
        User,
        UserAsk,
        UserReply,
        Shutdown
    }

    internal abstract class UserControlMessage : Message
    {
        public override Message.Type MessageType => Message.Type.UserControl;
        public abstract UserControlMessageTag Tag { get; }

        public static UserControlMessage Shutdown => new UserControlShutdownMessage();
    }

    internal class UserMessage : UserControlMessage
    {
        public override Message.Type MessageType => Message.Type.User;
        public override UserControlMessageTag Tag => UserControlMessageTag.User;

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

    internal class UserControlShutdownMessage : UserControlMessage
    {
        public override Message.Type MessageType => Message.Type.UserControl;
        public override UserControlMessageTag Tag => UserControlMessageTag.Shutdown;
    }
}
