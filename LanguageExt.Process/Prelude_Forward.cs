using System;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Map;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process:  Tell functions
    /// 
    ///     'Tell' is used to send a message from one process to another (or from outside a process to a process).
    ///     The messages are sent to the process asynchronously and join the process' inbox.  The process will 
    ///     deal with one message from its inbox at a time.  It cannot start the next message until it's finished
    ///     with a previous message.
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
                : tell(pid, ActorContext.CurrentRequest, ActorContext.AskId);
    }
}
