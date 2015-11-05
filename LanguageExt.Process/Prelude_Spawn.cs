using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Spawn functions
    /// 
    ///     The spawn functions create a new process.  Processes are either simple message receivers that
    ///     don't manage state and therefore only need a messageHandler.  Or they manage state over time.
    /// 
    ///     If they manage state then you should also provide a 'setup' function that generates the initial
    ///     state.  This allows the process system to recover if a process crashes.  It can re-call the 
    ///     setup function to reset the state and continue processing the messages.
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Create a new process by name.  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<T>(ProcessName name, Action<T> messageHandler, ProcessFlags flags = ProcessFlags.Default, State<StrategyContext, Unit> strategy = null) =>
            spawn<Unit, T>(name, () => unit, (state, msg) => { messageHandler(msg); return state; }, flags, strategy);

        /// <summary>
        /// Create a new process by name (accepts Unit as a return value instead of void).  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawnU<T>(ProcessName name, Func<T,Unit> messageHandler, ProcessFlags flags = ProcessFlags.Default, State<StrategyContext, Unit> strategy = null) =>
            spawn<Unit, T>(name, () => unit, (state, msg) => { messageHandler(msg); return state; }, flags, strategy);

        /// <summary>
        /// Create a new process by name.  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="setup">Startup and restart function</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<S, T>(
            ProcessName name,
            Func<S> setup,
            Func<S, T, S> messageHandler,
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            ActorContext.ActorCreate(ActorContext.SelfProcess, name, messageHandler, setup, strategy, flags, maxMailboxSize);

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="count">Number of processes to spawn</param>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<T>(
            int count, 
            ProcessName name, 
            Action<T> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            List.map(Range(0, count), n => spawn($"{name}-{n}", messageHandler, flags, strategy)).ToList();

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="count">Number of processes to spawn</param>
        /// <param name="setup">Startup and restart function</param>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<S, T>(
            int count, 
            ProcessName name, 
            Func<S> setup, 
            Func<S, T, S> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            List.map(Range(0, count), n => ActorContext.ActorCreate(ActorContext.SelfProcess, $"{name}-{n}", messageHandler, setup, strategy, flags, maxMailboxSize)).ToList();

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="spec">Map of IDs and State for generating child workers</param>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<S, T>(
            ProcessName name, 
            Map<int, Func<S>> spec, 
            Func<S, T, S> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            Map.map(spec, (id,state) => ActorContext.ActorCreate(ActorContext.SelfProcess, $"{name}-{id}", messageHandler, state, strategy, flags, maxMailboxSize)).Values.ToList();

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="name">Delegator process name</param>
        /// <param name="count">Number of worker processes</param>
        /// <param name="messageHandler">Worker message handler</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobin<T>(
            ProcessName name, 
            int count, 
            Action<T> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(name, () => ignore(spawnN(count, "worker", messageHandler, flags)), (_, msg) => HandleNoChild(() => fwdNextChild(msg)), flags, strategy);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="name">Delegator process name</param>
        /// <param name="count">Number of worker processes</param>
        /// <param name="messageHandler">Worker message handler</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobin<S, T>(
            ProcessName name, 
            int count, 
            Func<S> setup, 
            Func<S, T, S> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(name, () => ignore(spawnN(count, "worker", setup, messageHandler, flags)), (_, msg) => HandleNoChild(() => fwdNextChild(msg)), flags, strategy);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="name">Delegator process name</param>
        /// <param name="spec">Map of IDs and State for generating child workers</param>
        /// <param name="messageHandler">Worker message handler</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobin<S, T>(
            ProcessName name, 
            Map<int, Func<S>> spec, 
            Func<S, T, S> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(name, () => ignore(Map.map(spec, (id, state) => ActorContext.ActorCreate(ActorContext.SelfProcess, $"worker-{id}", messageHandler, state, DefaultStrategy, flags, maxMailboxSize))), (_, msg) => HandleNoChild(() => fwdNextChild(msg)), flags, strategy, maxMailboxSize);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="name">Delegator process name</param>
        /// <param name="count">Number of worker processes</param>
        /// <param name="map">Maps the message frop T to U before being passed to a worker</param>
        /// <param name="messageHandler">Worker message handler</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobinMap<T,U>(
            ProcessName name, 
            int count, 
            Func<T,U> map, 
            Action<U> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null, 
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(name, () => ignore(spawnN(count, "worker", messageHandler, flags, maxMailboxSize: maxMailboxSize)), (_, msg) => HandleNoChild(() => fwdNextChild(map(msg))), flags, strategy, maxMailboxSize);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="name">Delegator process name</param>
        /// <param name="count">Number of worker processes</param>
        /// <param name="map">Maps the message frop T to IEnumerable U before each one is passed to the workers</param>
        /// <param name="messageHandler">Worker message handler</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobinMapMany<T, U>(
            ProcessName name, 
            int count, 
            Func<T, IEnumerable<U>> map, Action<U> messageHandler, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(name, () => ignore(spawnN(count, "worker", messageHandler, flags, maxMailboxSize: maxMailboxSize)), (_, msg) => map(msg).Iter( m => HandleNoChild(() => fwdNextChild(m))), flags, strategy, maxMailboxSize);

        /// <summary>
        /// Spawn by type
        /// </summary>
        /// <typeparam name="TProcess">Process type</typeparam>
        /// <typeparam name="TMsg">Message type</typeparam>
        /// <param name="name">Name of process to spawn</param>
        /// <param name="flags">Process flags</param>
        /// <param name="strategy">Failure supervision strategy</param>
        /// <returns>ProcessId</returns>
        public static ProcessId spawn<TProcess,TMsg>(
            ProcessName name, 
            ProcessFlags flags = ProcessFlags.Default,
            State<StrategyContext, Unit> strategy = null,
            int maxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
            where TProcess : IProcess<TMsg>, new()
        {
            return spawn<IProcess<TMsg>, TMsg>(name, () => new TProcess(),
              (process, msg) => {
                  process.OnMessage(msg);
                  return process;
              },
              flags,
              strategy,
              maxMailboxSize
            );
        }

        static T HandleNoChild<T>(Func<T> act)
        {
            int max = 200;
            while (max > 0)
            {
                try
                {
                    return act();
                }
                catch (NoChildProcessesException)
                {
                    // TODO
                    System.Threading.Thread.Sleep(10);
                    max--;
                }
            }
            throw new NoChildProcessesException();
        }
    }
}
