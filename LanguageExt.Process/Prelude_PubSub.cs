using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process 
    /// 
    ///     Publish / Subscribe / Observe
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Publish a message for any listening subscribers
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <param name="message">Message to publish</param>
        public static Unit publish(object message) =>
            InMessageLoop
                ? ActorContext.Publish(message)
                : failWithMessageLoopEx<Unit>();

        /// <summary>
        /// Publish a message for any listening subscribers, delayed.
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <param name="message">Message to publish</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable publish(object message, TimeSpan delayFor) =>
            InMessageLoop
                ? delay(() => publish(message), delayFor).Subscribe()
                : failWithMessageLoopEx<IDisposable>();

        /// <summary>
        /// Publish a message for any listening subscribers, delayed.
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="message">Message to publish</param>
        /// <param name="delayUntil">When to send</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable publish(object message, DateTime delayUntil) =>
            InMessageLoop
                ? delay(() => publish(message), delayUntil).Subscribe()
                : failWithMessageLoopEx<IDisposable>();

        /// <summary>
        /// Subscribes our inbox to another process's publications
        /// </summary>
        /// <param name="pid">Process to subscribe to</param>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <returns>IDisposable, call IDispose to end the subscription</returns>
        public static IDisposable subscribe(ProcessId pid) =>
            observe<object>(pid).Subscribe(x => tell(Self, x, pid));

        /// <summary>
        /// Subscribe to the process's publish stream.  When a process calls 'pub' it emits
        /// messages that can be consumed using this method.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subscribe<T>(ProcessId pid, IObserver<T> observer) =>
            observe<T>(pid).Subscribe(observer);

        /// <summary>
        /// Subscribe to the process's publish stream.  When a process calls 'pub' it emits
        /// messages that can be consumed using this method.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError, Action onComplete) =>
            observe<T>(pid).Subscribe(onNext, onError, onComplete);

        /// <summary>
        /// Subscribe to the process's publish stream.  When a process calls 'pub' it emits
        /// messages that can be consumed using this method.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError) =>
            observe<T>(pid).Subscribe(onNext, onError, () => { });

        /// <summary>
        /// Subscribe to the process's publish stream.  When a process calls 'pub' it emits
        /// messages that can be consumed using this method.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext) =>
            observe<T>(pid).Subscribe(onNext, ex => { }, () => { });

        /// <summary>
        /// Subscribe to the process's publish stream.  When a process calls 'pub' it emits
        /// messages that can be consumed using this method.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext, Action onComplete) =>
            observe<T>(pid).Subscribe(onNext, ex => { }, onComplete);

        /// <summary>
        /// Get an IObservable for a process's publish stream.  When a process calls 'pub' it emits
        /// messages on the observable returned by this method.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IObservable<T> observe<T>(ProcessId pid) =>
            ActorContext.Observe<T>(pid);

        /// <summary>
        /// Get an IObservable for a process's state stream.  When a process's state updates it
        /// announces it on the stream returned from this method.  You should use this for notification
        /// only.  Never modify the state object belonging to a process.  Best practice is to make
        /// the state type immutable.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IObservable<T> observeState<T>(ProcessId pid) =>
            from x in ask<IObservable<object>>(ActorContext.Root, ActorSystemMessage.ObserveState(pid))
            where x is T
            select (T)x;
    }
}
