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
        ForwardToDeadLetters,
        ForwardToSelf,
        ForwardToParent,
        ForwardToProcess,
        StayInQueue
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
            new ForwardToDeadLetters();

        /// <summary>
        /// Forward the failed message back to the Process that failed.
        /// It will join the back of the queue.
        /// </summary>
        public static MessageDirective ForwardToSelf =>
            new ForwardToSelf();

        /// <summary>
        /// Forward the failed message to the supervisor (the parent) of the
        /// Process that failed.  It will join the back of the queue.
        /// </summary>
        public static readonly MessageDirective ForwardToParent = 
            new ForwardToParent();

        /// <summary>
        /// Forward the failed message to the Process specified.
        /// It will join the back of the queue.
        /// </summary>
        public static MessageDirective ForwardTo(ProcessId pid) =>
            new ForwardToProcess(pid);

        /// <summary>
        /// Forward the failed message back to the Process that failed.
        /// It will join the back of the queue.
        /// </summary>
        public static MessageDirective StayInQueue =>
            new StayInQueue();

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

    class ForwardToDeadLetters : MessageDirective
    {
        public ForwardToDeadLetters()
            :
            base(MessageDirectiveType.ForwardToDeadLetters)
        {
        }
    }

    class ForwardToSelf : MessageDirective
    {
        public ForwardToSelf()
            :
            base(MessageDirectiveType.ForwardToSelf)
        {
        }
    }

    class ForwardToParent : MessageDirective
    {
        public ForwardToParent()
            :
            base(MessageDirectiveType.ForwardToParent)
        {
        }
    }

    class ForwardToProcess : MessageDirective
    {
        public readonly ProcessId ProcessId;

        public ForwardToProcess(ProcessId pid)
            :
            base(MessageDirectiveType.ForwardToProcess)
        {
            ProcessId = pid;
        }
    }

    class StayInQueue : MessageDirective
    {
        public StayInQueue()
            :
            base(MessageDirectiveType.StayInQueue)
        {
        }
    }
}
