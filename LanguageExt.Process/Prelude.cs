using LanguageExt.UnitsOfMeasure;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// <para>
    ///     Process
    /// </para>
    /// <para>
    ///     The Language Ext process system uses the actor model as seen in Erlang 
    ///     processes.  Actors are famed for their ability to support massive concurrency
    ///     through messaging and no shared memory.
    /// </para>
    /// <para>
    ///     https://en.wikipedia.org/wiki/Actor_model
    /// </para>
    /// <para>
    ///     Each process has an 'inbox' and a state.  The state is the property of the
    ///     process and no other.  The messages in the inbox are passed to the process
    ///     one at a time.  When the process has finished processing a message it returns
    ///     its current state.  This state is then passed back in with the next message.
    /// </para>
    /// <para>
    ///     You can think of it as a fold over a stream of messages.
    /// </para>
    /// <para>
    ///     A process must finish dealing with a message before another will be given.  
    ///     Therefore they are blocking.  But they block themselves only. The messages 
    ///     will build up whilst they are processing.
    /// </para>
    /// <para>
    ///     Because of this, processes are also in a 'supervision hierarchy'.  Essentially
    ///     each process can spawn child-processes and the parent process 'owns' the child.  
    /// </para>
    /// <para>
    ///     Processes have a default failure strategy where the process just restarts with
    ///     its original state.  The inbox always survives a crash and the failed message 
    ///     is sent to a 'dead letters' process.  You can monitor this. You can also provide
    ///     bespoke strategies for different types of failure behaviours (See Strategy folder)
    /// </para>
    /// <para>
    ///     So post crash the process restarts and continues processing the next message.
    /// </para>
    /// <para>
    ///     By creating child processes it's possible for a parent process to 'offload'
    ///     work.  It could create 10 child processes, and simply route the messages it
    ///     gets to its children for a very simple load balancer. Processes are very 
    ///     lightweight and should not be seen as Threads or Tasks.  You can create 
    ///     10s of 1000s of them and it will 'just work'.
    /// </para>
    /// <para>
    ///     Scheduled tasks become very simple also.  You can send a process to a message
    ///     with a delay.  So a background process that needs to run every 30 minutes 
    ///     can just send itself a message with a delay on it at the end of its message
    ///     handler:
    /// </para>
    /// <para>
    ///         tellSelf(unit, TimeSpan.FromMinutes(30));
    /// </para>
    /// <para>
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Triggers when the Process system shuts down
        /// Either subscribe to the OnNext or OnCompleted
        /// </summary>
        public static IObservable<ShutdownCancellationToken> PreShutdown =>
            preShutdownSubj;

        /// <summary>
        /// Triggers when the Process system shuts down
        /// Either subscribe to the OnNext or OnCompleted
        /// </summary>
        public static IObservable<SystemName> Shutdown =>
            shutdownSubj;

        /// <summary>
        /// Log of everything that's going on in the Languge Ext process system
        /// </summary>
        public static IObservable<ProcessLogItem> ProcessSystemLog => 
            log.ObserveOn(TaskPoolScheduler.Default);

        /// <summary>
        /// Current process ID
        /// </summary>
        /// <remarks>
        /// This should be used from within a process message loop only
        /// </remarks>
        public static ProcessId Self =>
            ActorContext.Self;

        /// <summary>
        /// Parent process ID
        /// </summary>
        /// <remarks>
        /// This should be used from within a process message loop only
        /// </remarks>
        public static ProcessId Parent =>
            InMessageLoop
                ? ActorContext.Request.Parent.Actor.Id
                : raiseUseInMsgLoopOnlyException<ProcessId>(nameof(Parent));

        /// <summary>
        /// Root process ID
        /// The Root process is the parent of all processes
        /// </summary>
        public static ProcessId Root(SystemName system = default(SystemName)) =>
            ActorContext.System(system).Root;

        /// <summary>
        /// User process ID
        /// The User process is the default entry process, your first process spawned
        /// will be a child of this process.
        /// </summary>
        public static ProcessId User(SystemName system = default(SystemName)) =>
            ActorContext.System(system).User;

        /// <summary>
        /// Dead letters process
        /// Subscribe to it to monitor the failed messages (<see cref="subscribe(ProcessId)"/>)
        /// </summary>
        public static ProcessId DeadLetters(SystemName system = default(SystemName)) =>
            ActorContext.System(system).DeadLetters;

        /// <summary>
        /// Errors process
        /// Subscribe to it to monitor the errors thrown 
        /// </summary>
        public static ProcessId Errors(SystemName system = default(SystemName)) =>
            ActorContext.System(system).Errors;

        /// <summary>
        /// Sender process ID
        /// Always valid even if there's not a sender (the 'NoSender' process ID will
        /// be provided).
        /// </summary>
        public static ProcessId Sender =>
            ActorContext.Request.Sender;

        /// <summary>
        /// Get the child processes of the running process
        /// </summary>
        /// <remarks>
        /// This should be used from within a process message loop only
        /// </remarks>
        public static Map<string, ProcessId> Children =>
            InMessageLoop
                ? ActorContext.Request.Children
                : raiseUseInMsgLoopOnlyException<Map<string,ProcessId>>(nameof(Children));

        /// <summary>
        /// Get the child processes of the process ID provided
        /// </summary>
        public static Map<string, ProcessId> children(ProcessId pid) =>
            ActorContext.System(pid).GetChildren(pid);

        /// <summary>
        /// Get the child processes by name
        /// </summary>
        public static ProcessId child(ProcessName name) =>
            InMessageLoop
                ? Self[name]
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
                ? ActorContext.Request.Children.Count == 0
                    ? raise<ProcessId>(new NoChildProcessesException())
                    : ActorContext.Request
                                  .Children
                                  .Skip(index % ActorContext.Request.Children.Count)
                                  .Map( kv => kv.Value )
                                  .Head()
                : raiseUseInMsgLoopOnlyException<ProcessId>(nameof(child));

        /// <summary>
        /// Immediately kills the Process that is running from within its message
        /// loop.  It does this by throwing a ProcessKillException which is caught
        /// and triggers the shutdown of the Process.  Any Process that has a 
        /// persistent inbox or state will also have its persistent data wiped.  
        /// If the Process is registered it will have its registration revoked.
        /// If you wish for the data to be maintained for future spawns then call 
        /// Process.shutdown()
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit kill() =>
            InMessageLoop
                ? raise<Unit>(new ProcessKillException())
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(kill));

        /// <summary>
        /// Shutdown the currently running process.  The shutdown message jumps 
        /// ahead of any messages already in the process's queue but doesn't exit
        /// immediately like kill().  Any Process that has a persistent inbox or 
        /// state will have its state maintained for future spawns.  If you wish 
        /// for the data to be dropped then call Process.kill()
        /// </summary>
        public static Unit shutdown() =>
            shutdown(Self);

        /// <summary>
        /// Kill a specified running process.
        /// Forces the specified Process to shutdown.  The kill message jumps 
        /// ahead of any messages already in the process's queue.  Any Process
        /// that has a persistent inbox or state will also have its persistent
        /// data wiped.  If the Process is registered it will also its 
        /// registration revoked.
        /// If you wish for the data to be maintained for future
        /// spawns then call Process.shutdown(pid);
        /// </summary>
        public static Unit kill(ProcessId pid) =>
            ActorContext.System(pid).Kill(pid, false);

        /// <summary>
        /// Shutdown a specified running process.
        /// Forces the specified Process to shutdown.  The shutdown message jumps 
        /// ahead of any messages already in the process's queue.  Any Process
        /// that has a persistent inbox or state will have its state maintained
        /// for future spawns.  If you wish for the data to be dropped then call
        /// Process.kill(pid)
        /// </summary>
        public static Unit shutdown(ProcessId pid) =>
            ActorContext.System(pid).Kill(pid, true);

        /// <summary>
        /// Shutdown all processes on the specified process-system
        /// </summary>
        public static Unit shutdownSystem(SystemName system) =>
            ActorContext.StopSystem(system);

        /// <summary>
        /// Shutdown all processes on all process-systems
        /// </summary>
        public static Unit shutdownAll() =>
            ActorContext.StopAllSystems();

        /// <summary>
        /// Forces a running process to restart.  This will reset its state and drop
        /// any subscribers, or any of its subscriptions.
        /// </summary>
        public static Unit restart(ProcessId pid) =>
            ActorContext.System(pid).TellSystem(pid, SystemMessage.Restart);

        /// <summary>
        /// Pauses a running process.  Messages will still be accepted into the Process'
        /// inbox (unless the inbox is full); but they won't be processed until the
        /// Process is unpaused: <see cref="unpause(ProcessId)"/>
        /// </summary>
        /// <param name="pid">Process to pause</param>
        public static Unit pause(ProcessId pid) =>
            ActorContext.System(pid).TellSystem(pid, SystemMessage.Pause);

        /// <summary>
        /// Pauses a running process.  Messages will still be accepted into the Process'
        /// inbox (unless the inbox is full); but they won't be processed until the
        /// Process is unpaused: <see cref="unpause(ProcessId)"/> manually, or until the 
        /// delay expires.
        /// </summary>
        /// <param name="pid">Process to pause</param>
        public static IDisposable pauseFor(ProcessId pid, Time delay)
        {
            pause(pid);
            return (IDisposable)Task.Delay((int)delay.Milliseconds).ContinueWith(_ => unpause(pid));
        }

        /// <summary>
        /// Un-pauses a paused process.  Messages that have built-up in the inbox whilst
        /// the Process was paused will be Processed immediately.
        /// </summary>
        /// <param name="pid">Process to un-pause</param>
        public static Unit unpause(ProcessId pid) =>
            ActorContext.System(pid).TellSystem(pid, SystemMessage.Unpause);

        /// <summary>
        /// Find out if a process exists
        /// 
        ///     Rules:
        ///         * Local processes   - the process must actually be alive and in-memory
        ///         * Remote processes  - the process must have an inbox to receive messages 
        ///                               and may be active, but it's not required.
        ///         * Dispatchers/roles - at least one process in the collection must exist(pid)
        ///         * JS processes      - not current supported
        /// </summary>
        /// <param name="pid">Process ID to check</param>
        /// <returns>True if exists</returns>
        public static bool exists(ProcessId pid) =>
            ActorContext.System(pid).GetDispatcher(pid).Exists;

        /// <summary>
        /// Watch another Process in case it terminates
        /// </summary>
        /// <param name="pid">Process to watch</param>
        public static Unit watch(ProcessId pid) =>
            InMessageLoop
                ? ActorContext.Request.Self.Actor.DispatchWatch(pid)
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(watch));

        /// <summary>
        /// Un-watch another Process that this Process has been watching
        /// </summary>
        /// <param name="pid">Process to watch</param>
        public static Unit unwatch(ProcessId pid) =>
            InMessageLoop
                ? ActorContext.Request.Self.Actor.DispatchUnWatch(pid)
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(unwatch));

        /// <summary>
        /// Watch for the death of the watching process and tell the watcher
        /// process when that happens.
        /// </summary>
        /// <param name="watcher">Watcher</param>
        /// <param name="watching">Watched</param>
        public static Unit watch(ProcessId watcher, ProcessId watching) =>
            ActorContext.System(watcher).GetDispatcher(watcher).DispatchWatch(watching);

        /// <summary>
        /// Stop watching for the death of the watching process
        /// </summary>
        /// <param name="watcher">Watcher</param>
        /// <param name="watching">Watched</param>
        public static Unit unwatch(ProcessId watcher, ProcessId watching) =>
            ActorContext.System(watcher).GetDispatcher(watcher).DispatchUnWatch(watching);

        /// <summary>
        /// Find the number of items in the Process inbox
        /// </summary>
        /// <param name="pid">Process</param>
        /// <returns>Number of items in the Process inbox</returns>
        public static int inboxCount(ProcessId pid) =>
            ActorContext.System(pid).GetDispatcher(pid).GetInboxCount();

        /// <summary>
        /// Return True if the message sent is a Tell and not an Ask
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static bool isTell =>
            InMessageLoop
                ? ActorContext.Request.CurrentRequest == null
                : raiseUseInMsgLoopOnlyException<bool>(nameof(isTell));

        /// <summary>
        /// Get a list of cluster nodes that are online
        /// </summary>
        public static Map<ProcessName, ClusterNode> ClusterNodes(SystemName system = default(SystemName)) =>
            ActorContext.System(system).ClusterState == null
                ? Map.empty<ProcessName, ClusterNode>()
                : ActorContext.System(system).ClusterState.Members;

        /// <summary>
        /// List of system names running on this node
        /// </summary>
        public static Lst<SystemName> Systems =>
            ActorContext.Systems;

        /// <summary>
        /// Return True if the message sent is an Ask and not a Tell
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static bool isAsk => !isTell;

        /// <summary>
        /// Resolves a ProcessId into the absolute ProcessIds that it represents
        /// This allows live resolution of role-based ProcessIds to their real node
        /// ProcessIds.  
        /// </summary>
        /// <remarks>Mostly useful for debugging, but could be useful for layering
        /// additional logic to any message dispatch.
        /// </remarks>
        /// <param name="pid"></param>
        /// <returns>Enumerable of resolved ProcessIds - could be zero length</returns>
        public static IEnumerable<ProcessId> resolve(ProcessId pid) =>
            ActorContext.System(pid).ResolveProcessIdSelection(pid);
    }
}
