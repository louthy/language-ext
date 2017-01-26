using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// <para>
    ///     Process: Ask functions
    /// </para>
    /// <para>
    ///     'ask' is a request/response system for processes.  You can ask a process a question (a message) and it
    ///     can reply using the 'Process.reply' function.  It doesn't have to and 'ask' will timeout after 
    ///     ActorConfig.Default.Timeout seconds. 
    /// </para>
    /// <para>
    ///     'ask' is blocking, because mostly it will be called from within a process and processes shouldn't 
    ///     perform asynchronous operations.
    /// </para>
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Asynchronous ask - must be used outside of a Process
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="pid">Process to ask</param>
        /// <param name="message">Message to send</param>
        /// <returns>A promise to return a response to the request</returns>
        public static Task<R> askAsync<R>(ProcessId pid, object message) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<Task<R>>(nameof(observeState))
                : Task.Run(() => ask<R>(pid, message));

        /// <summary>
        /// Ask a process for a reply
        /// </summary>
        /// <param name="pid">Process to ask</param>
        /// <param name="message">Message to send</param>
        /// <returns>The response to the request</returns>
        public static T ask<T>(ProcessId pid, object message) =>
            ActorContext.System(pid).Ask<T>(pid, message);

        /// <summary>
        /// Ask children the same message
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        public static IEnumerable<T> askChildren<T>(object message, int take = Int32.MaxValue) =>
            ActorContext.System(default(SystemName)).AskMany<T>(Children.Values, message, take);

        /// <summary>
        /// Ask parent process for a reply
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>The response to the request</returns>
        public static T askParent<T>(object message) =>
            ask<T>(Parent, message);

        /// <summary>
        /// Ask a named child process for a reply
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="name">Name of the child process</param>
        public static T askChild<T>(ProcessName name, object message) =>
            ask<T>(Self.Child(name), message);

        /// <summary>
        /// Ask a child process (found by index) for a reply
        /// </summary>
        /// <remarks>
        /// Because of the potential changeable nature of child nodes, this will
        /// take the index and mod it by the number of children.  We expect this 
        /// call will mostly be used for load balancing, and round-robin type 
        /// behaviour, so feel that's acceptable.  
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="index">Index of the child process (see remarks)</param>
        public static T askChild<T>(int index, object message) =>
            ask<T>(child(index), message);
    }
}
