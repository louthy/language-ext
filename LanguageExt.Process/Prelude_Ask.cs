using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Ask functions
    /// 
    ///     'ask' is a request/response system for processes.  You can ask a process a question (a message) and it
    ///     can reply using the 'Process.reply' function.  It doesn't have to and 'ask' will timeout after 
    ///     ActorConfig.Default.Timeout seconds. 
    /// 
    ///     'ask' is blocking, because mostly it will be called from within a process and processes shouldn't 
    ///     perform asynchronous operations.
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Ask a process for a reply
        /// </summary>
        /// <param name="pid">Process to ask</param>
        /// <param name="message">Message to send</param>
        /// <returns>The response to the request</returns>
        public static T ask<T>(ProcessId pid, object message) =>
            ActorContext.Ask<T>(pid, message).Wait();

        /// <summary>
        /// Ask children the same message
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        public static IEnumerable<T> askChildren<T>(object message, int take = Int32.MaxValue) =>
            Observable.Merge<T>(Children.Values.Map(child => ActorContext.Ask<T>(child, message)))
                      .Take(Math.Max(take, Children.Count))
                      .ToEnumerable();

        /// <summary>
        /// Ask parent process for a reply
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>The response to the request</returns>
        public static T askParent<T>(object message) =>
            ask<T>(Parent, message);
    }
}
