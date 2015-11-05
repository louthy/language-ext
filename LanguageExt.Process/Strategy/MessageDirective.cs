using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public enum MessageDirectiveType
    {
        SendToDeadLetters,
        SendToSelf,
        SendToParent,
        Retry
    }

    /// <summary>
    /// Message directives
    /// This class represents a set of core behaviours for the message that
    /// caused a Process to fail.
    /// </summary>
    public abstract class MessageDirective : IEquatable<MessageDirective>
    {
        public readonly MessageDirectiveType Type;

        protected MessageDirective(MessageDirectiveType type)
        {
            Type = type;
        }

        /// <summary>
        /// Forward the failed message onto the Dead Letters process
        /// This is the default behaviour
        /// </summary>
        public static MessageDirective ForwardToDeadLetters =>
            new SendToDeadLetters();

        /// <summary>
        /// Forward the failed message back to the Process that failed.
        /// It will join the back of the queue.
        /// </summary>
        public static MessageDirective ForwardToSelf =>
            new SendToSelf();

        /// <summary>
        /// Forward the failed message to the supervisor (the parent) of the
        /// Process that failed.  It will join the back of the queue.
        /// </summary>
        public static readonly MessageDirective ForwardToParent = 
            new SendToParent();

        /// <summary>
        /// Forward the failed message back to the Process that failed.
        /// It will join the back of the queue.
        /// </summary>
        public static MessageDirective StayInQueue =>
            new RetryMessage();

        public virtual bool Equals(MessageDirective other) =>
            Type == other.Type;

        public override bool Equals(object obj) =>
            obj is MessageDirective
                ? Equals((MessageDirective)obj)
                : false;

        public override int GetHashCode() =>
            Type.GetHashCode();

        public static bool operator ==(MessageDirective lhs, MessageDirective rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(MessageDirective lhs, MessageDirective rhs) =>
            !(lhs == rhs);
    }

    class SendToDeadLetters : MessageDirective
    {
        public SendToDeadLetters()
            :
            base(MessageDirectiveType.SendToDeadLetters)
        {
        }
    }

    class SendToSelf : MessageDirective
    {
        public SendToSelf()
            :
            base(MessageDirectiveType.SendToSelf)
        {
        }
    }

    class SendToParent : MessageDirective
    {
        public SendToParent()
            :
            base(MessageDirectiveType.SendToParent)
        {
        }
    }

    class RetryMessage : MessageDirective
    {
        public RetryMessage()
            :
            base(MessageDirectiveType.Retry)
        {
        }
    }
}
