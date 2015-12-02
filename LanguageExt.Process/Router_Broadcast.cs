using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Process;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Router
    {
        /// <summary>
        /// Spawns a new process with Count worker processes, each message is sent to 
        /// all worker processes
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcast<S, T>(
            ProcessName Name,
            int Count,
            Func<S> Setup,
            Func<S, T, S> Inbox,
            ProcessFlags Flags                    = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize                    = ProcessSetting.DefaultMailboxSize,
            string WorkerName                     = "worker"
            )
        {
            if (Inbox == null) throw new ArgumentNullException(nameof(Inbox));
            if (Setup == null) throw new ArgumentNullException(nameof(Setup));
            if (Count < 1) throw new ArgumentException($"{nameof(Count)} should be greater than 0");

            return spawn<Unit, T>(
                Name,
                () =>
                {
                    spawnMany(Count, WorkerName, Setup, Inbox, Flags);
                    return unit;
                },
                (_, msg) => Children.Iter(pid => fwd(pid)),
                Flags, 
                Strategy, 
                MaxMailboxSize
            );
        }

        /// <summary>
        /// Spawns a new process with that routes each message to the Workers
        /// in a round robin fashion.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Flags">Process flags</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcast<T>(
            ProcessName Name,
            IEnumerable<ProcessId> Workers,
            RouterOption Options = RouterOption.Default,
            ProcessFlags Flags = ProcessFlags.Default,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
        {
            if (Workers == null) throw new ArgumentNullException(nameof(Workers));
            var workers = Set.createRange(Workers);
            if (workers.Count < 1) throw new ArgumentException($"{nameof(Workers)} should have a length of at least 1");

            var router = spawn<T>(
                Name,
                msg => workers.Iter(pid => fwd(pid)),
                Flags,
                DefaultStrategy,
                MaxMailboxSize,
                Terminated: pid => workers = workers.Remove(pid)
            );
            return WatchWorkers(router, workers, Options);
        }

        /// <summary>
        /// Spawns a new process with Count worker processes, each message is sent to 
        /// all worker processes
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Map">Message mapping function</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcastMap<S, T, U>(
            ProcessName Name,
            int Count,
            Func<S> Setup,
            Func<S, U, S> Inbox,
            Func<T, U> Map,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            )
        {
            if (Inbox == null) throw new ArgumentNullException(nameof(Inbox));
            if (Setup == null) throw new ArgumentNullException(nameof(Setup));
            if (Count < 1) throw new ArgumentException($"{nameof(Count)} should be greater than 0");

            return spawn<Unit, T>(
                Name,
                () =>
                {
                    spawnMany(Count, WorkerName, Setup, Inbox, Flags);
                    return unit;
                },
                (_, msg) =>
                {
                    var umsg = Map(msg);
                    return Children.Iter(pid => fwd(pid,umsg));
                },
                Flags,
                Strategy,
                MaxMailboxSize
            );
        }

        /// <summary>
        /// Spawns a new process with that routes each message to all workers
        /// Each message is mapped before being broadcast.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Map">Message mapping function</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcastMap<T, U>(
            ProcessName Name,
            IEnumerable<ProcessId> Workers,
            Func<T,U> Map,
            RouterOption Options = RouterOption.Default,
            ProcessFlags Flags = ProcessFlags.Default,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
        {
            if (Workers == null) throw new ArgumentNullException(nameof(Workers));
            var workers = Set.createRange(Workers);
            if (workers.Count < 1) throw new ArgumentException($"{nameof(Workers)} should have a length of at least 1");
            var router = spawn<T>(
                Name,
                msg =>
                {
                    var umsg = Map(msg);
                    workers.Iter(pid => fwd(pid, umsg));
                },
                Flags,
                DefaultStrategy,
                MaxMailboxSize,
                Terminated: pid => workers = workers.Remove(pid)
            );
            return WatchWorkers(router, workers, Options);
        }

        /// <summary>
        /// Spawns a new process with N worker processes, each message is mapped 
        /// from T to IEnumerable U before each resulting Us are passed all of the 
        /// worker processes.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="map">Maps the message from T to IEnumerable U before each one is passed to the workers</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcastMapMany<S, T, U>(
            ProcessName Name,
            int Count,
            Func<S> Setup,
            Func<S, U, S> Inbox,
            Func<T, IEnumerable<U>> MapMany,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            )
        {
            if (Inbox == null) throw new ArgumentNullException(nameof(Inbox));
            if (WorkerName == null) throw new ArgumentNullException(nameof(WorkerName));
            if (Count < 1) throw new ArgumentException($"{nameof(Count)} should be greater than 0");

            return spawn<Unit, T>(
                Name,
                () =>
                {
                    spawnMany(Count, WorkerName, Setup, Inbox, Flags);
                    return unit;
                },
                (_, msg) =>
                {
                    MapMany(msg).Iter(umsg => Children.Iter(pid => fwd(pid, umsg)));
                    return unit;
                },
                Flags,
                Strategy,
                MaxMailboxSize
            );
        }

        /// <summary>
        /// Spawns a new process with N worker processes, each message is mapped 
        /// from T to IEnumerable U before each resulting Us are passed all of the 
        /// worker processes.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="map">Maps the message from T to IEnumerable U before each one is passed to the workers</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcastMapMany<T, U>(
            ProcessName Name,
            IEnumerable<ProcessId> Workers,
            Func<T, IEnumerable<U>> MapMany,
            RouterOption Options = RouterOption.Default,
            ProcessFlags Flags = ProcessFlags.Default,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
        {
            if (Workers == null) throw new ArgumentNullException(nameof(Workers));
            var workers = Set.createRange(Workers);
            if (workers.Count < 1) throw new ArgumentException($"{nameof(Workers)} should have a length of at least 1");

            var router = spawn<T>(
                Name,
                msg => MapMany(msg).Iter(umsg => workers.Iter(pid => fwd(pid, umsg))),
                Flags,
                DefaultStrategy,
                MaxMailboxSize,
                Terminated: pid => workers = workers.Remove(pid)
            );
            return WatchWorkers(router, workers, Options);
        }

        /// <summary>
        /// Spawns a new process with Count worker processes, each message is sent to one worker
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
        public static ProcessId broadcast<T>(
            ProcessName Name,
            int Count,
            Action<T> Inbox,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            ) =>
            broadcast<Unit, T>(Name, Count, () => unit, (_, msg) => { Inbox(msg); return unit; }, Flags, Strategy, MaxMailboxSize, WorkerName);

        /// <summary>
        /// Spawns a new process with Count worker processes, each message is mapped
        /// and sent to one worker process in a round robin fashion.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcastMap<T, U>(
            ProcessName Name,
            int Count,
            Action<U> Inbox,
            Func<T, U> Map,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            ) =>
            broadcastMap(Name, Count, () => unit, (_, umsg) => { Inbox(umsg); return unit; }, Map, Flags, Strategy, MaxMailboxSize, WorkerName);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is mapped 
        /// from T to IEnumerable U before each resulting U is passed to the worker
        /// processes in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <typeparam name="U">Mapped message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="map">Maps the message from T to IEnumerable U before each one is passed to the workers</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId broadcastMapMany<T, U>(
            ProcessName Name,
            int Count,
            Action<U> Inbox,
            Func<T, IEnumerable<U>> MapMany,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            ) =>
            broadcastMapMany(Name, Count, () => unit, (_, umsg) => { Inbox(umsg); return unit; }, MapMany, Flags, Strategy, MaxMailboxSize, WorkerName);
    }
}
