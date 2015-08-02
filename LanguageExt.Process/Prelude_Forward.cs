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
        public static Unit fwd<T>(ProcessId pid) =>
            ActorContext.CurrentRequest == null
                ? tell(pid, ActorContext.CurrentMsg, Sender)
                : tell(pid, ActorContext.CurrentRequest, ActorContext.AskId);
    }
}
