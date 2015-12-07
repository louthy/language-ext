using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        /// Triggers when the Process system shuts down
        /// Either subscribe to the OnNext or OnCompleted
        /// </summary>
        public static readonly IObservable<CancelShutdown> PreShutdown =
            preShutdownSubj;

        /// <summary>
        /// Triggers when the Process system shuts down
        /// Either subscribe to the OnNext or OnCompleted
        /// </summary>
        public static readonly IObservable<Unit> Shutdown =
            shutdownSubj;

        /// <summary>
        /// Log of everything that's going on in the Languge Ext process system
        /// </summary>
        public static readonly IObservable<ProcessLogItem> ProcessSystemLog = 
            log.ObserveOn(TaskPoolScheduler.Default);

        /// <summary>
        /// Current process ID
        /// </summary>
        /// <remarks>
        /// This should be used from within a process message loop only
        /// </remarks>
        public static ProcessId Self =>
            InMessageLoop
                ? ActorContext.Self
                : ActorContext.User;

        /// <summary>
        /// Parent process ID
        /// </summary>
        /// <remarks>
        /// This should be used from within a process message loop only
        /// </remarks>
        public static ProcessId Parent =>
            InMessageLoop
                ? ActorContext.Parent
                : raiseUseInMsgLoopOnlyException<ProcessId>(nameof(Parent));

        /// <summary>
        /// Root process ID
        /// The Root process is the parent of all processes
        /// </summary>
        public static ProcessId Root =>
            ActorContext.Root;

        /// <summary>
        /// User process ID
        /// The User process is the default entry process, your first process spawned
        /// will be a child of this process.
        /// </summary>
        public static ProcessId User =>
            ActorContext.User;

        /// <summary>
        /// Dead letters process
        /// Subscribe to it to monitor the failed messages (<see cref="subscribe(ProcessId)"/>)
        /// </summary>
        public static ProcessId DeadLetters =>
            ActorContext.DeadLetters;

        /// <summary>
        /// Errors process
        /// Subscribe to it to monitor the errors thrown 
        /// </summary>
        public static ProcessId Errors =>
            ActorContext.Errors;

        /// <summary>
        /// Registered process root
        /// It allows local and distributed processes to be found by name 
        /// </summary>
        public static ProcessId Registered =>
            ActorContext.Registered;

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
        /// This should be used from within a process message loop only
        /// </remarks>
        public static Map<string, ProcessId> Children =>
            InMessageLoop
                ? ActorContext.Children
                : raiseUseInMsgLoopOnlyException<Map<string,ProcessId>>(nameof(Children));

        /// <summary>
        /// Get the child processes of the process ID provided
        /// </summary>
        public static Map<string, ProcessId> children(ProcessId pid) =>
            ActorContext.GetChildren(pid);

        /// <summary>
        /// Get the child processes by name
        /// </summary>
        public static ProcessId child(ProcessName name) =>
            InMessageLoop
                ? Self.Child(name)
                : raiseUseInMsgLoopOnlyException<ProcessId>(nameof(child));

        /// <summary>
        /// Get the child processes by index.
        /// </summary>
        /// <remarks>
        /// Because of the potential changeable nature of child nodes, this will
        /// take the index and mod it by the number of children.  We expect this 
        /// call will mostly be used for load balancing, and round-robin type 
        /// behaviour, so feel that's acceptable.  
        /// </remarks>
        public static ProcessId child(int index) =>
            InMessageLoop
                ? ActorContext.SelfProcess.Actor.Children.Count == 0
                    ? raise<ProcessId>(new NoChildProcessesException())
                    : ActorContext.SelfProcess
                                  .Actor
                                  .Children
                                  .Skip(index % ActorContext.SelfProcess.Actor.Children.Count)
                                  .Map( kv => kv.Value.Actor.Id )
                                  .Head()
                : raiseUseInMsgLoopOnlyException<ProcessId>(nameof(child));

        /// <summary>
        /// Find a registered process by name
        /// </summary>
        /// <param name="name">Process name</param>
        /// <returns>ProcessId or ProcessId.None</returns>
        public static ProcessId find(ProcessName name) =>
            ActorContext.Registered.Child(name);

        /// <summary>
        /// Register self as a named process
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        /// <typeparam name="T">The message type of the actor to register</typeparam>
        /// <param name="name">Name to register under</param>
        public static ProcessId register<T>(ProcessName name, ProcessFlags flags = ProcessFlags.Default, int maxMailboxSize = ProcessSetting.DefaultMailboxSize) =>
            InMessageLoop
                ? ActorContext.Register<T>(name, Self, flags, maxMailboxSize)
                : raiseUseInMsgLoopOnlyException<ProcessId>(nameof(name));

        /// <summary>
        /// Register the name with the process
        /// </summary>
        /// <typeparam name="T">The message type of the actor to register</typeparam>
        /// <param name="name">Name to register under</param>
        /// <param name="process">Process to be registered</param>
        public static ProcessId register<T>(ProcessName name, ProcessId process, ProcessFlags flags = ProcessFlags.Default, int maxMailboxSize = ProcessSetting.DefaultMailboxSize) =>
            ActorContext.Register<T>(name, process, flags, maxMailboxSize);

        /// <summary>
        /// Deregister the process associated with the name
        /// </summary>
        /// <param name="name">Name of the process to deregister</param>
        public static Unit deregister(ProcessName name) =>
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
                ? raise<Unit>(new ProcessKillException())
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(kill));

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
            ActorContext.Kill(pid);

        /// <summary>
        /// Shutdown all processes and restart
        /// </summary>
        public static Unit shutdownAll() =>
            ActorContext.Shutdown();

        /// <summary>
        /// Forces a running process to restart.  This will reset its state and drop
        /// any subscribers, or any of its subscriptions.
        /// </summary>
        public static Unit restart(ProcessId pid) =>
            ActorContext.TellSystem(pid, SystemMessage.Restart);

        /// <summary>
        /// Pauses a running process.  Messages will still be accepted into the Process'
        /// inbox (unless the inbox is full); but they won't be processes until the
        /// Process is unpaused: <see cref="unpause(ProcessId)"/>
        /// </summary>
        /// <param name="pid">Process to pause</param>
        public static Unit pause(ProcessId pid) =>
            ActorContext.TellSystem(pid, SystemMessage.Pause);

        /// <summary>
        /// Un-pauses a paused process.  Messages that have built-up in the inbox whilst
        /// the Process was paused will be Processed immediately.
        /// </summary>
        /// <param name="pid">Process to un-pause</param>
        public static Unit unpause(ProcessId pid) =>
            ActorContext.TellSystem(pid, SystemMessage.Unpause);

        /// <summary>
        /// Watch another Process in case it terminates
        /// </summary>
        /// <param name="pid">Process to watch</param>
        public static Unit watch(ProcessId pid) =>
            InMessageLoop
                ? ActorContext.SelfProcess.Actor.DispatchWatch(pid)
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(watch));

        /// <summary>
        /// Un-watch another Process that this Process has been watching
        /// </summary>
        /// <param name="pid">Process to watch</param>
        public static Unit unwatch(ProcessId pid) =>
            InMessageLoop
                ? ActorContext.SelfProcess.Actor.DispatchUnWatch(pid)
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(unwatch));

        /// <summary>
        /// Watch for the death of the watching process and tell the watcher
        /// process when that happens.
        /// </summary>
        /// <param name="watcher">Watcher</param>
        /// <param name="watching">Watched</param>
        public static Unit watch(ProcessId watcher, ProcessId watching) =>
            ActorContext.GetDispatcher(watcher).DispatchWatch(watching);

        /// <summary>
        /// Stop watching for the death of the watching process
        /// </summary>
        /// <param name="watcher">Watcher</param>
        /// <param name="watching">Watched</param>
        public static Unit unwatch(ProcessId watcher, ProcessId watching) =>
            ActorContext.GetDispatcher(watcher).DispatchUnWatch(watching);

        /// <summary>
        /// Find the number of items in the Process inbox
        /// </summary>
        /// <param name="pid">Process</param>
        /// <returns>Number of items in the Process inbox</returns>
        public static int inboxCount(ProcessId pid) =>
            ActorContext.GetDispatcher(pid).GetInboxCount();

        /// <summary>
        /// Return True if the message sent is a Tell and not an Ask
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static bool isTell =>
            InMessageLoop
                ? ActorContext.CurrentRequest == null
                : raiseUseInMsgLoopOnlyException<bool>(nameof(isTell));

        /// <summary>
        /// Get a list of cluster nodes that are online
        /// </summary>
        public static Map<ProcessName, ClusterNode> ClusterNodes =>
            ActorContext.ClusterState == null
                ? Map.empty<ProcessName, ClusterNode>()
                : ActorContext.ClusterState.Members;

        /// <summary>
        /// Return True if the message sent is an Ask and not a Tell
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static bool isAsk => !isTell;
    }
}
