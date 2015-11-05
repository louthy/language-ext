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
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<T>(
            ProcessName Name, 
            Action<T> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default, 
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(Name, () => unit, (state, msg) => { Inbox(msg); return state; }, Flags, Strategy, MaxMailboxSize);

        /// <summary>
        /// Create a new process by name (accepts Unit as a return value instead of void).  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawnU<T>(
            ProcessName Name, 
            Func<T,Unit> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default, 
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(Name, () => unit, (state, msg) => { Inbox(msg); return state; }, Flags, Strategy, MaxMailboxSize);

        /// <summary>
        /// Create a new process by name.  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Setup">Startup and restart function</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<S, T>(
            ProcessName Name,
            Func<S> Setup,
            Func<S, T, S> Inbox,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            ActorContext.ActorCreate(ActorContext.SelfProcess, Name, Inbox, Setup, Strategy, Flags, MaxMailboxSize);

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Count">Number of processes to spawn</param>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<T>(
            int Count, 
            ProcessName Name, 
            Action<T> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            List.map(Range(0, Count), n => spawn($"{Name}-{n}", Inbox, Flags, Strategy)).ToList();

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Count">Number of processes to spawn</param>
        /// <param name="Setup">Startup and restart function</param>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<S, T>(
            int Count, 
            ProcessName Name, 
            Func<S> Setup, 
            Func<S, T, S> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            List.map(Range(0, Count), n => ActorContext.ActorCreate(ActorContext.SelfProcess, $"{Name}-{n}", Inbox, Setup, Strategy, Flags, MaxMailboxSize)).ToList();

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
        /// <param name="Spec">Map of IDs and State for generating child workers</param>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<S, T>(
            ProcessName Name, 
            Map<int, Func<S>> Spec, 
            Func<S, T, S> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            Map.map(Spec, (id,state) => ActorContext.ActorCreate(ActorContext.SelfProcess, $"{Name}-{id}", Inbox, state, Strategy, Flags, MaxMailboxSize)).Values.ToList();

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobin<T>(
            ProcessName Name, 
            int Count, 
            Action<T> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(Name, () => ignore(spawnN(Count, "worker", Inbox, Flags)), (_, msg) => HandleNoChild(() => fwdNextChild(msg)), Flags, Strategy);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobin<S, T>(
            ProcessName Name, 
            int Count, 
            Func<S> Setup, 
            Func<S, T, S> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(Name, () => ignore(spawnN(Count, "worker", Setup, Inbox, Flags)), (_, msg) => HandleNoChild(() => fwdNextChild(msg)), Flags, Strategy);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Spec">Map of IDs and State for generating child workers</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobin<S, T>(
            ProcessName Name, 
            Map<int, Func<S>> Spec, 
            Func<S, T, S> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(Name, () => ignore(Map.map(Spec, (id, state) => ActorContext.ActorCreate(ActorContext.SelfProcess, $"worker-{id}", Inbox, state, DefaultStrategy, Flags, MaxMailboxSize))), (_, msg) => HandleNoChild(() => fwdNextChild(msg)), Flags, Strategy, MaxMailboxSize);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Map">Maps the message frop T to U before being passed to a worker</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobinMap<T,U>(
            ProcessName Name, 
            int Count, 
            Func<T,U> Map, 
            Action<U> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null, 
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(Name, () => ignore(spawnN(Count, "worker", Inbox, Flags, MaxMailboxSize: MaxMailboxSize)), (_, msg) => HandleNoChild(() => fwdNextChild(Map(msg))), Flags, Strategy, MaxMailboxSize);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="map">Maps the message frop T to IEnumerable U before each one is passed to the workers</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId spawnRoundRobinMapMany<T, U>(
            ProcessName Name, 
            int Count, 
            Func<T, IEnumerable<U>> map, Action<U> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            spawn<Unit, T>(Name, () => ignore(spawnN(Count, "worker", Inbox, Flags, MaxMailboxSize: MaxMailboxSize)), (_, msg) => map(msg).Iter( m => HandleNoChild(() => fwdNextChild(m))), Flags, Strategy, MaxMailboxSize);

        /// <summary>
        /// Spawn by type
        /// </summary>
        /// <typeparam name="TProcess">Process type</typeparam>
        /// <typeparam name="TMsg">Message type</typeparam>
        /// <param name="Name">Name of process to spawn</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>ProcessId</returns>
        public static ProcessId spawn<TProcess,TMsg>(
            ProcessName Name, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
            where TProcess : IProcess<TMsg>, new()
        {
            return spawn<IProcess<TMsg>, TMsg>(Name, () => new TProcess(),
              (process, msg) => {
                  process.OnMessage(msg);
                  return process;
              },
              Flags,
              Strategy,
              MaxMailboxSize
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
