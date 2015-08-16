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
        public static Unit publish<T>(T message) =>
            InMessageLoop
                ? ActorContext.Publish(message)
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(publish));

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
        public static IDisposable publish<T>(T message, TimeSpan delayFor) =>
            InMessageLoop
                ? delay(() => ActorContext.Publish(message), delayFor).Subscribe()
                : raiseUseInMsgLoopOnlyException<IDisposable>(nameof(publish));

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
        public static IDisposable publish<T>(T message, DateTime delayUntil) =>
            InMessageLoop
                ? delay(() => ActorContext.Publish(message), delayUntil).Subscribe()
                : raiseUseInMsgLoopOnlyException<IDisposable>(nameof(publish));

        /// <summary>
        /// Subscribes our inbox to another process publish stream.  When it calls 'publish' it will
        /// arrive in our inbox.
        /// </summary>
        /// <param name="pid">Process to subscribe to</param>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <returns>IDisposable, call IDispose to end the subscription</returns>
        public static Unit subscribe(ProcessId pid) =>
            InMessageLoop
                ? ActorContext.SelfProcess.AddSubscription(pid, ActorContext.Observe<object>(pid).Subscribe(x => tell(Self, x, pid)))
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(subscribe));

        /// <summary>
        /// Unsubscribe from a process's publications
        /// </summary>
        /// <param name="pid">Process to unsub from</param>
        public static Unit unsubscribe(ProcessId pid) =>
            InMessageLoop
                ? ActorContext.SelfProcess.RemoveSubscription(pid)
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(unsubscribe));

        /// <summary>
        /// Subscribe to the process publish stream.  When a process calls 'publish' it emits
        /// messages that can be consumed using this method.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// Because this call is asychronous it could allow access to the message loop, therefore
        /// you can't call it from within a process message loop.
        /// </remarks>
        /// <returns>IDisposable, call IDispose to end the subscription</returns>
        public static IDisposable subscribe<T>(ProcessId pid, IObserver<T> observer) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<IDisposable>(nameof(subscribe))
                : observe<T>(pid).Subscribe(observer);

        /// <summary>
        /// Subscribe to the process publish stream.  When a process calls 'publish' it emits
        /// messages that can be consumed using this method.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// Because this call is asychronous it could allow access to the message loop, therefore
        /// you can't call it from within a process message loop.
        /// </remarks>
        /// <returns>IDisposable, call IDispose to end the subscription</returns>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError, Action onComplete) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<IDisposable>(nameof(subscribe))
                : observe<T>(pid).Subscribe(onNext, onError, onComplete);

        /// <summary>
        /// Subscribe to the process publish stream.  When a process calls 'publish' it emits
        /// messages that can be consumed using this method.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// Because this call is asychronous it could allow access to the message loop, therefore
        /// you can't call it from within a process message loop.
        /// </remarks>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<IDisposable>(nameof(subscribe))
                : observe<T>(pid).Subscribe(onNext, onError, () => { });

        /// <summary>
        /// Subscribe to the process publish stream.  When a process calls 'publish' it emits
        /// messages that can be consumed using this method.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// Because this call is asychronous it could allow access to the message loop, therefore
        /// you can't call it from within a process message loop.
        /// </remarks>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<IDisposable>(nameof(subscribe))
                : observe<T>(pid).Subscribe(onNext, ex => { }, () => { });

        /// <summary>
        /// Subscribe to the process publish stream.  When a process calls 'publish' it emits
        /// messages that can be consumed using this method.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// Because this call is asychronous it could allow access to the message loop, therefore
        /// you can't call it from within a process message loop.
        /// </remarks>
        /// <returns>IDisposable, call IDispose to end the subscription</returns>
        public static IDisposable subscribe<T>(ProcessId pid, Action<T> onNext, Action onComplete) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<IDisposable>(nameof(subscribe))
                : observe<T>(pid).Subscribe(onNext, ex => { }, onComplete);

        /// <summary>
        /// Get an IObservable for a process publish stream.  When a process calls 'publish' it emits
        /// messages on the observable returned by this method.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// Because this call is asychronous it could allow access to the message loop, therefore
        /// you can't call it from within a process message loop.
        /// </remarks>
        /// <returns>IObservable T</returns>
        public static IObservable<T> observe<T>(ProcessId pid) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<IObservable<T>>(nameof(observe))
                : ActorContext.Observe<T>(pid);

        /// <summary>
        /// Get an IObservable for a process's state stream.  When a process state updates at the end of its
        /// message loop it announces it on the stream returned from this method.  You should use this for 
        /// notification only.  Never modify the state object belonging to a process.  Best practice is to make
        /// the state type immutable.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// 
        /// Because this call is asychronous it could allow access to the message loop, therefore
        /// you can't call it from within a process message loop.
        /// </remarks>
        /// <returns>IObservable T</returns>
        public static IObservable<T> observeState<T>(ProcessId pid) =>
            InMessageLoop
                ? raiseDontUseInMessageLoopException<IObservable<T>>(nameof(observeState))
                : from x in ask<IObservable<object>>(ActorContext.Root, ActorSystemMessage.ObserveState(pid))
                  where x is T
                  select (T)x;


        /// <summary>
        /// Subscribes our inbox to another process state publish stream.  
        /// When a process state updates at the end of its message loop it announces it arrives in our inbox.
        /// You should use this for notification only.  Never modify the state object belonging to a process.  
        /// Best practice is to make the state type immutable.
        /// </summary>
        /// <remarks>
        /// The process can publish any number of types, any published messages not of type T will be ignored.
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <returns>IObservable T</returns>
        public static Unit subscribeState<T>(ProcessId pid) =>
            InMessageLoop
                ? ActorContext.SelfProcess.AddSubscription(
                      pid,
                     (from x in ask<IObservable<object>>(ActorContext.Root, ActorSystemMessage.ObserveState(pid))
                      where x is T
                      select (T)x).Subscribe(x => tell(Self, x, pid)))
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(subscribeState));
    }
}
