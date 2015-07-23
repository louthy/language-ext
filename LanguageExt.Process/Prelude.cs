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
    ///     The Language Ext process system uses the actor model as seen in Erlang 
    ///     processes.  Actors are famed for their ability to support massive concurrency
    ///     through messaging and no shared memory.
    /// 
    ///     https://en.wikipedia.org/wiki/Actor_model
    /// 
    ///     Each process has an 'inbox' and a state.  The state is the property of the
    ///     process and no other.  The messages in the inbox are passed to the process
    ///     one at a time.  When the process has finished processing a message it returns
    ///     its current state.  This state is then passed back in with the next message.
    /// 
    ///     You can think of it as a fold over a stream of messages.
    /// 
    ///     A process must finish dealing with a message before another will be given.  
    ///     Therefore they are blocking.  But they block themselves only. The messages 
    ///     will build up whilst they are processing.
    /// 
    ///     Because of this, processes are also in a 'supervision hierarchy'.  Essentially
    ///     each process can spawn child-processes and the parent process 'owns' the child.  
    /// 
    ///     Although not currently implemented in Language Ext processes (it will be 
    ///     soon), it is usually possible to have strategies for what happens when a child 
    ///     dies.  Currently the process just restarts with its original state.  The inbox 
    ///     always survives a crash and the failed message is sent to a 'dead letters' 
    ///     process.  You can monitor this. 
    /// 
    ///     So post crash the process restarts and continues processing the next message.
    /// 
    ///     By creating child processes it's possible for a parent process to 'offload'
    ///     work.  It could create 10 child processes, and simply route the messages it
    ///     gets to its children for a very simple load balancer. Processes are very 
    ///     lightweight and should not be seen as Threads or Tasks.  You can create 
    ///     10s of 1000s of them and it will 'just work'.
    /// 
    ///     Scheduled tasks become very simple also.  You can send a process to a message
    ///     with a delay.  So a background process that needs to run every 30 minutes 
    ///     can just send itself a message with a delay on it at the end of its message
    ///     handler:
    /// 
    ///         tellSelf(unit, TimeSpan.FromMinutes(30));
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Log of everything that's going on in the Languge Ext process system
        /// </summary>
        public static readonly IObservable<ProcessLogItem> ProcessLog = 
            log.SubscribeOn(TaskPoolScheduler.Default);

        /// <summary>
        /// Current process ID
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static ProcessId Self =>
            InMessageLoop
                ? ActorContext.Self
                : failWithMessageLoopEx<ProcessId>();

        /// <summary>
        /// Parent process ID
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static ProcessId Parent =>
            InMessageLoop
                ? ActorContext.Parent
                : failWithMessageLoopEx<ProcessId>();

        /// <summary>
        /// User process ID
        /// The User process is the default entry process
        /// </summary>
        public static ProcessId User =>
            ActorContext.User;

        /// <summary>
        /// Sender process ID
        /// Always valid even if there's not a sender (the 'NoSender' process ID will
        /// be provided).
        /// </summary>
        public static ProcessId Sender =>
            ActorContext.Sender;

        /// <summary>
        /// Get the child processes of the running process
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Map<string, ProcessId> Children =>
            InMessageLoop
                ? ActorContext.Children
                : failWithMessageLoopEx<Map<string,ProcessId>>();

        /// <summary>
        /// Find a registered process by name
        /// </summary>
        /// <param name="name">Process name</param>
        /// <returns>ProcessId or ProcessId.None</returns>
        public static ProcessId find(ProcessName name) =>
            ActorContext.Registered.MakeChildId(name);

        /// <summary>
        /// Register self as a named process
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <param name="name">Name to register under</param>
        public static ProcessId reg(ProcessName name) =>
            InMessageLoop
                ? ActorContext.Register(name, Self)
                : failWithMessageLoopEx<ProcessId>();

        /// <summary>
        /// Register the name with the process
        /// </summary>
        /// <param name="name">Name to register under</param>
        /// <param name="process">Process to be registered</param>
        public static ProcessId reg(ProcessName name, ProcessId process) =>
            ActorContext.Register(name, process);

        /// <summary>
        /// Deregister the process associated with the name
        /// </summary>
        /// <param name="name">Name of the process to deregister</param>
        public static Unit dereg(ProcessName name) =>
            ActorContext.Deregister(name);

        /// <summary>
        /// Forces the current running process to shutdown.  The kill message 
        /// jumps ahead of any messages already in the queue.
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit kill() =>
            InMessageLoop
                ? raise<Unit>(new SystemKillActorException())
                : failWithMessageLoopEx<Unit>();

        /// <summary>
        /// Shutdown the currently running process.
        /// This differs from kill() in that the shutdown message just joins
        /// the back of the queue like all other messages allowing any backlog
        /// to be processed first.
        /// </summary>
        public static Unit shutdown() =>
            kill(Self);

        /// <summary>
        /// Kill a specified running process.
        /// Forces the specified process to shutdown.  The kill message 
        /// jumps ahead of any messages already in the process's queue.
        /// </summary>
        public static Unit kill(ProcessId pid) =>
            ActorContext.RootInbox.Tell(ActorSystemMessage.ShutdownProcess(pid), ActorContext.Self);

        /// <summary>
        /// Reply to an ask
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit reply(object message) =>
            InMessageLoop
                ? ActorContext.CurrentRequestId == -1
                    ? ActorContext.Sender.IsValid
                        ? tell(ActorContext.Sender, message, ActorContext.Self)
                        : failwith<Unit>(
                            "You can't reply to this message.  It was sent from outside of a process and it wasn't an 'ask'.  " +
                            "Therefore we have no return address or observable stream to send the reply to."
                            )
                    : ActorContext.RootInbox.Tell(ActorSystemMessage.Reply(ActorContext.CurrentRequestId, message), ActorContext.Self)
                : failWithMessageLoopEx<Unit>();

        /// <summary>
        /// Publish a message for any listening subscribers
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <param name="message">Message to publish</param>
        public static Unit publish(object message) =>
            InMessageLoop
                ? ActorContext.RootInbox.Tell(ActorSystemMessage.Publish(ActorContext.Self, message), ActorContext.Self)
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
                ? delay(() => publish(message),delayFor).Subscribe()
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
        /// Get the child processes of the process provided
        /// </summary>
        public static Map<string, ProcessId> children(ProcessId pid) =>
            ask<Map<string, ProcessId>>(ActorContext.Root, ActorSystemMessage.GetChildren(pid)).Wait();

        /// <summary>
        /// Shutdown all processes and restart
        /// </summary>
        public static IObservable<Unit> shutdownAll() =>
            ask<Unit>(ActorContext.Root,ActorSystemMessage.ShutdownAll);

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
            from x in ask<IObservable<object>>(ActorContext.Root, ActorSystemMessage.ObservePub(pid))
                        .Timeout(ActorConfig.Default.Timeout)
                        .Wait()
            where x is T
            select (T)x;

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
                        .Timeout(ActorConfig.Default.Timeout)
                        .Wait()
            where x is T
            select (T)x;

        /// <summary>
        /// Not in message loop exception
        /// </summary>
        internal static T failWithMessageLoopEx<T>() =>
            failwith<T>("This should be used from within a process' message loop only");

        /// <summary>
        /// Returns true if in a message loop
        /// </summary>
        internal static bool InMessageLoop =>
            ActorContext.Self.IsValid && ActorContext.Self.Value != ActorContext.User.Value;
    }
}
