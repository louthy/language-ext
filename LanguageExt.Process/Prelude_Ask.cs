using System;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Ask functions
    /// 
    ///     'Ask' is a request/response system for processes.  You can ask a process a question (a message) and it
    ///     can reply using the 'Process.reply' function.  It doesn't have to, and the returned IObservable has
    ///     a default timeout set of 10 seconds.  You must subscribe to the IObservable for the message to be sent.
    /// 
    ///     If possible you should avoid 'ask', as it's not as efficient as 'tell'.  But sometimes it's is necessary
    ///     to stop a ton of hoop jumping...  so it exists.  Internally this is what happens:
    /// 
    ///         An Rx Subject is packaged with your message and a unique request ID
    ///         That package is sent as a message via a tell to the internal 'ask-process'
    ///         It recieves the message, puts it in it's dictionary of requests
    ///         It then sends the message as a tell to the process you want a response from
    ///         The process receives it, and calls reply (this is another 'tell').  
    ///         The reply arrives back at the internal 'ask-process'
    ///         It matches up the request ID in the reply with your request, gets the Rx Subject and calls OnNext 
    ///         You receive the response in the IObservable<T> subscription.
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Ask a process for a reply
        /// </summary>
        /// <param name="pid">Process to ask</param>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>The response to the request</returns>
        public static IObservable<T> ask<T>(ProcessId pid, object message, ProcessId sender = default(ProcessId)) =>
            ActorContext.Ask<T>(pid, message, sender);

        /// <summary>
        /// Ask a process for a reply at a specified time in the future
        /// </summary>
        /// <remarks>NOTE: The 'ask' is triggered at the time specified.  It doesn't fire immediately and then return the held result later.</remarks>
        /// <param name="pid">Process to ask</param>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>The response to the request</returns>
        public static IObservable<T> ask<T>(ProcessId pid, object message, TimeSpan delayFor, ProcessId sender = default(ProcessId)) =>
            from x in delay(() => ActorContext.Ask<T>(pid, message, sender), delayFor)
            from y in x
            select y;

        /// <summary>
        /// Ask a process for a reply at a specified time in the future
        /// </summary>
        /// <remarks>NOTE: The 'ask' is triggered at the time specified.  It doesn't fire immediately and then return the held result later.</remarks>
        /// <param name="pid">Process to ask</param>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>The response to the request</returns>
        public static IObservable<T> ask<T>(ProcessId pid, object message, DateTime delayUntil, ProcessId sender = default(ProcessId)) =>
            from x in delay(() => ActorContext.Ask<T>(pid, message, sender), delayUntil)
            from y in x
            select y;

        /// <summary>
        /// Ask children the same message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IObservable<T> askChildren<T>(object message) =>
            Observable.Merge(Children.Values.Map(child => ask<T>(child, message))).Take(Children.Count);

        /// <summary>
        /// Ask children the same message at a specified time in the future
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <returns>The N responses</returns>
        public static IObservable<T> askChildren<T>(object message, TimeSpan delayFor) =>
            from x in delay(() => Observable.Merge(Children.Values.Map(child => ask<T>(child, message))).Take(Children.Count), delayFor)
            from y in x
            select y;

        /// <summary>
        /// Ask children the same message at a specified time in the future
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <returns>The N responses</returns>
        public static IObservable<T> askChildren<T>(object message, DateTime delayUntil) =>
            from x in delay(() => Observable.Merge(Children.Values.Map(child => ask<T>(child, message))).Take(Children.Count), delayUntil)
            from y in x
            select y;

        /// <summary>
        /// Ask parent process for a reply
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>The response to the request</returns>
        public static IObservable<T> askParent<T>(object message, ProcessId sender = default(ProcessId)) =>
            ask<T>(ActorContext.Parent, message, sender);

        /// <summary>
        /// Ask parent process for a reply at a specified time in the future
        /// </summary>
        /// <remarks>NOTE: The 'ask' is triggered at the time specified.  It doesn't fire immediately and then return the held result later.</remarks>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>The response to the request</returns>
        public static IObservable<T> askParent<T>(object message, TimeSpan delayFor, ProcessId sender = default(ProcessId)) =>
            ask<T>(ActorContext.Parent, message, delayFor, sender);

        /// <summary>
        /// Ask parent process for a reply at a specified time in the future
        /// </summary>
        /// <remarks>NOTE: The 'ask' is triggered at the time specified.  It doesn't fire immediately and then return the held result later.</remarks>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>The response to the request</returns>
        public static IObservable<T> askParent<T>(ProcessId pid, object message, DateTime delayUntil, ProcessId sender = default(ProcessId)) =>
            ask<T>(ActorContext.Parent, message, delayUntil, sender);
    }
}
