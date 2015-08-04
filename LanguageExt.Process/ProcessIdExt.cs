using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

/// <summary>
/// Extensions methods for ProcessId
/// </summary>
public static class __ProcessIdExt
{
    //
    // Prelude
    //

    /// <summary>
    /// Get the child processes of this process
    /// </summary>
    public static Map<string, ProcessId> GetChildren(this ProcessId self) =>
        Process.children(self);

    /// <summary>
    /// Get the child processes by name
    /// </summary>
    public static ProcessId GetChild(this ProcessId self, ProcessName name) =>
        self.MakeChildId(name);

    /// <summary>
    /// Get the child processes by index.
    /// </summary>
    /// <remarks>
    /// Because of the potential changeable nature of child nodes, this will
    /// take the index and mod it by the number of children.  We expect this 
    /// call will mostly be used for load balancing, and round-robin type 
    /// behaviour, so feel that's acceptable.  
    /// </remarks>
    public static ProcessId GetChild(this ProcessId self, int index) =>
        GetChildren(self)
            .Skip(index % ActorContext.SelfProcess.Children.Count)
            .Map(kv => kv.Value)
            .Head();

    /// <summary>
    /// Register as a named process
    /// </summary>
    public static ProcessId Register<T>(this ProcessId self, ProcessFlags flags = ProcessFlags.Default) =>
        ActorContext.Register<T>(self.GetName(), self, flags);

    /// <summary>
    /// Register as a named process
    /// </summary>
    public static ProcessId Register<T>(this ProcessId self, ProcessName name, ProcessFlags flags = ProcessFlags.Default) =>
        ActorContext.Register<T>(name, self, flags);

    /// <summary>
    /// Kill the process.
    /// Forces the process to shutdown.  The kill message 
    /// jumps ahead of any messages already in the process's queue.
    /// </summary>
    public static Unit Kill(this ProcessId self) =>
        ActorContext.LocalRoot.Tell(ActorSystemMessage.ShutdownProcess(self), ActorContext.Self);

    //
    // Ask
    //

    /// <summary>
    /// Ask a process for a reply
    /// </summary>
    /// <param name="pid">Process to ask</param>
    /// <param name="message">Message to send</param>
    /// <returns>The response to the request</returns>
    public static T Ask<T>(this ProcessId pid, object message) =>
        ask<T>(pid, message);


    //
    // Pub Sub
    //
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
    public static Unit Subscribe(this ProcessId pid) =>
        subscribe(pid);

    /// <summary>
    /// Unsubscribe from a process's publications
    /// </summary>
    /// <param name="pid">Process to unsub from</param>
    public static Unit Unsubscribe(this ProcessId pid) =>
        unsubscribe(pid);

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
    public static IDisposable Subscribe<T>(this ProcessId pid, IObserver<T> observer) =>
        subscribe(pid, observer);

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
    public static IDisposable Subscribe<T>(this ProcessId pid, Action<T> onNext, Action<Exception> onError, Action onComplete) =>
        subscribe(pid, onNext, onError, onComplete);

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
    public static IDisposable Subscribe<T>(this ProcessId pid, Action<T> onNext, Action<Exception> onError) =>
        subscribe(pid, onNext, onError);

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
    public static IDisposable Subscribe<T>(this ProcessId pid, Action<T> onNext) =>
        subscribe(pid, onNext);

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
    public static IDisposable Subscribe<T>(this ProcessId pid, Action<T> onNext, Action onComplete) =>
        subscribe(pid, onNext, onComplete);

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
    public static IObservable<T> Observe<T>(this ProcessId pid) =>
        observe<T>(pid);

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
    public static IObservable<T> ObserveState<T>(this ProcessId pid) =>
        observeState<T>(pid);

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
    public static Unit SubscribeState<T>(this ProcessId pid) =>
        subscribeState<T>(pid);


    //
    // Tell
    //

    /// <summary>
    /// Send a message to a process
    /// </summary>
    /// <param name="pid">Process ID to send to</param>
    /// <param name="message">Message to send</param>
    /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
    public static Unit Tell<T>(this ProcessId pid, T message, ProcessId sender = default(ProcessId)) =>
        tell(pid, message, sender);

    /// <summary>
    /// Send a message at a specified time in the future
    /// </summary>
    /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
    /// for any other reason.</returns>
    /// <param name="pid">Process ID to send to</param>
    /// <param name="message">Message to send</param>
    /// <param name="delayFor">How long to delay sending for</param>
    /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
    public static IDisposable Tell<T>(this ProcessId pid, T message, TimeSpan delayFor, ProcessId sender = default(ProcessId)) =>
        tell(pid, message, delayFor, sender);

    /// <summary>
    /// Send a message at a specified time in the future
    /// </summary>
    /// <remarks>
    /// This will fail to be accurate across a Daylight Saving Time boundary
    /// </remarks>
    /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
    /// for any other reason.</returns>
    /// <param name="pid">Process ID to send to</param>
    /// <param name="message">Message to send</param>
    /// <param name="delayUntil">Date and time to send</param>
    /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
    public static IDisposable Tell<T>(this ProcessId pid, T message, DateTime delayUntil, ProcessId sender = default(ProcessId)) =>
        tell(pid, message, delayUntil, sender);
}
