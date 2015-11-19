using System;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Map;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process:  Forward functions
    /// 
    ///     'fwd' is used to forward a message onto another process whilst maintaining the original 
    ///     sender context (for 'ask' responses to go back to the right place).
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Forward a message
        /// </summary>
        /// <param name="pid">Process ID to send to</param>
        /// <param name="message">Message to send</param>
        public static Unit fwd<T>(ProcessId pid, T message) =>
            ActorContext.CurrentRequest == null
                ? tell(pid, message, Sender)
                : tell(pid, 
                    new ActorRequest(
                        message,
                        pid,
                        ActorContext.CurrentRequest.ReplyTo,
                        ActorContext.CurrentRequest.RequestId),
                    ActorContext.AskId);

        /// <summary>
        /// Forward a message
        /// </summary>
        /// <param name="pid">Process ID to send to</param>
        public static Unit fwd(ProcessId pid) =>
            ActorContext.CurrentRequest == null
                ? tell(pid, ActorContext.CurrentMsg, Sender)
                : tell(pid, ActorContext.CurrentRequest, ActorContext.AskId);

        /// <summary>
        /// Forward a message to a named child process
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="name">Name of the child process</param>
        public static Unit fwdChild<T>(ProcessName name, T message) =>
            fwd(Self.Child(name), message);

        /// <summary>
        /// Forward a message to a child process (found by index)
        /// </summary>
        /// <remarks>
        /// Because of the potential changeable nature of child nodes, this will
        /// take the index and mod it by the number of children.  We expect this 
        /// call will mostly be used for load balancing, and round-robin type 
        /// behaviour, so feel that's acceptable.  
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="index">Index of the child process (see remarks)</param>
        public static Unit fwdChild<T>(int index, T message) =>
            fwd(child(index), message);

        /// <summary>
        /// Forward a message to a named child process
        /// </summary>
        /// <param name="name">Name of the child process</param>
        public static Unit fwdChild<T>(ProcessName name) =>
            fwd(Self.Child(name));

        /// <summary>
        /// Forward a message to a child process (found by index)
        /// </summary>
        /// <remarks>
        /// Because of the potential changeable nature of child nodes, this will
        /// take the index and mod it by the number of children.  We expect this 
        /// call will mostly be used for load balancing, and round-robin type 
        /// behaviour, so feel that's acceptable.  
        /// </remarks>
        /// <param name="index">Index of the child process (see remarks)</param>
        public static Unit fwdChild<T>(int index) =>
            fwd(child(index));
    }
}
