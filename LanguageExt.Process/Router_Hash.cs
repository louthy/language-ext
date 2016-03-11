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
        /// Default hashing function, used by the hashing routers by default if no
        /// HashFunc parameter is provided.  The function turns a worker-count and 
        /// message into a worker index
        /// </summary>
        /// <typeparam name="TMsg">Message type</typeparam>
        /// <returns>Function that turns a worker-count and message into a worker index</returns>
        public static Func<int, TMsg, int> DefaultHashFunction<TMsg>() =>
            (workers, msg)  => 
                msg.GetHashCode() % workers;

        /// <summary>
        /// Spawns a new process with Count worker processes, each message is sent to a worker
        /// process by calling GetHashCode on the message and modding by the number of workers,
        /// the result is used as the worker index to route to.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId hash<S, T>(
            ProcessName Name,
            int Count,
            Func<S> Setup,
            Func<S, T, S> Inbox,
            Func<int, T, int> HashFunc    = null,
            ProcessFlags Flags                    = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize                    = ProcessSetting.DefaultMailboxSize,
            string WorkerName                     = "worker"
            )
        {
            if (Inbox == null) throw new ArgumentNullException(nameof(Inbox));
            if (Setup == null) throw new ArgumentNullException(nameof(Setup));
            if (Count < 1) throw new ArgumentException($"{nameof(Count)} should be greater than 0");
            HashFunc = HashFunc ?? DefaultHashFunction<T>();

            return spawn<Unit, T>(
                Name,
                () =>
                {
                    spawnMany(Count, WorkerName, Setup, Inbox, Flags);
                    return unit;
                },
                (_, msg) =>
                {
                    var child = Children.Skip(HashFunc(Children.Count, msg)).Take(1).ToArray();
                    if (child.Length == 0)
                    {
                        throw new NoRouterWorkersException();
                    }
                    else
                    {
                        fwd(child[0].Value);
                    }
                    return unit;
                },
                Flags, 
                Strategy, 
                MaxMailboxSize
            );
        }

        /// <summary>
        /// Spawns a new process with that routes each message by calling GetHashCode on the 
        /// message and modding by the number of workers and using that as the worker index
        /// to route to.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId hash<T>(
            ProcessName Name,
            IEnumerable<ProcessId> Workers,
            Func<int, T, int> HashFunc = null,
            RouterOption Options = RouterOption.Default,
            ProcessFlags Flags = ProcessFlags.Default,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
        {
            if (Workers == null) throw new ArgumentNullException(nameof(Workers));
            var workers = Workers.ToArray();
            if (workers.Length < 1) throw new ArgumentException($"{nameof(Workers)} should have a length of at least 1");
            HashFunc = HashFunc ?? DefaultHashFunction<T>();
            var router = spawn<T>(
                Name,
                msg =>
                {
                    fwd(workers[HashFunc(Children.Count, msg)]);
                },
                Flags,
                DefaultStrategy,
                MaxMailboxSize,
                Terminated: pid => { workers = workers.Where(x => x != pid).ToArray(); }
            );
            return WatchWorkers(router, workers, Options);
        }

        /// <summary>
        /// Spawns a new process with Count worker processes, each message is sent to a worker
        /// process by calling GetHashCode on the message and modding by the number of workers
        /// and using that to find the worker index to route to.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId hashMap<S, T, U>(
            ProcessName Name,
            int Count,
            Func<S> Setup,
            Func<S, U, S> Inbox,
            Func<T, U> Map,
            Func<int, T, int> HashFunc = null,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            )
        {
            if (Inbox == null) throw new ArgumentNullException(nameof(Inbox));
            if (Setup == null) throw new ArgumentNullException(nameof(Setup));
            if (Count < 1) throw new ArgumentException($"{nameof(Count)} should be greater than 0");
            HashFunc = HashFunc ?? DefaultHashFunction<T>();

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
                    var child = Children.Skip(HashFunc(Children.Count, msg)).Take(1).ToArray();
                    if (child.Length == 0)
                    {
                        throw new NoRouterWorkersException();
                    }
                    else
                    {
                        fwd(child[0].Value, umsg);
                    }
                    return unit;
                },
                Flags,
                Strategy,
                MaxMailboxSize
            );
        }

        /// <summary>
        /// Spawns a new process where each message is sent to a worker
        /// process by calling GetHashCode on the mapped message and 
        /// modding it by the number of workers to find the worker index 
        /// to route to.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId hashMap<T, U>(
            ProcessName Name,
            IEnumerable<ProcessId> Workers,
            Func<T, U> Map,
            Func<int, U, int> HashFunc = null,
            RouterOption Options = RouterOption.Default,
            ProcessFlags Flags = ProcessFlags.Default,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
        {
            if (Workers == null) throw new ArgumentNullException(nameof(Workers));
            var workers = Workers.ToArray();
            if (workers.Length < 1) throw new ArgumentException($"{nameof(Workers)} should have a length of at least 1");
            HashFunc = HashFunc ?? DefaultHashFunction<U>();
            var router = spawn<T>(
                Name,
                msg =>
                {
                    var umsg = Map(msg);
                    fwd(workers[HashFunc(Children.Count, umsg)], umsg);
                },
                Flags,
                DefaultStrategy,
                MaxMailboxSize,
                Terminated: pid => { workers = workers.Where(x => x != pid).ToArray(); }
            );
            return WatchWorkers(router, workers, Options);
        }

        /// <summary>
        /// Spawns a new process with N worker processes, each message is mapped 
        /// from T to IEnumerable U before each resulting U is passed to the worker
        /// processes by getting each mapped message and calling GetHashCode and modding 
        /// by the number of workers to find the worker index to route to.
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
        public static ProcessId hashMapMany<S, T, U>(
            ProcessName Name,
            int Count,
            Func<S> Setup,
            Func<S, U, S> Inbox,
            Func<T, IEnumerable<U>> MapMany,
            Func<int, U, int> HashFunc = null,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            )
        {
            if (Inbox == null) throw new ArgumentNullException(nameof(Inbox));
            if (WorkerName == null) throw new ArgumentNullException(nameof(WorkerName));
            if (Count < 1) throw new ArgumentException($"{nameof(Count)} should be greater than 0");
            HashFunc = HashFunc ?? DefaultHashFunction<U>();

            return spawn<Unit, T>(
                Name,
                () =>
                {
                    spawnMany(Count, WorkerName, Setup, Inbox, Flags);
                    return unit;
                },
                (_, msg) =>
                {
                    var us = MapMany(msg);

                    foreach (var u in us)
                    {
                        var index = HashFunc(Children.Count, u);
                        var kids = Children.Skip(index);

                        var child = kids.Take(1).ToArray();
                        if (child.Length == 0)
                        {
                            kids = Children;
                            child = kids.Take(1).ToArray();
                            if (child.Length == 0)
                            {
                                throw new NoRouterWorkersException();
                            }
                        }

                        fwd(child[0].Value, u);
                        kids = kids.Skip(1);
                    }
                    return unit;
                },
                Flags,
                Strategy,
                MaxMailboxSize
            );
        }


        /// <summary>
        /// Spawns a new process with N worker processes, each message is mapped 
        /// from T to IEnumerable U before each resulting U is passed to the worker
        /// processes by getting each mapped message and calling GetHashCode and modding 
        /// by the number of workers to find the worker index to route to.
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
        public static ProcessId hashMapMany<T, U>(
            ProcessName Name,
            IEnumerable<ProcessId> Workers,
            Func<T, IEnumerable<U>> MapMany,
            Func<int, U, int> HashFunc = null,
            RouterOption Options = RouterOption.Default,
            ProcessFlags Flags = ProcessFlags.Default,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
        {
            if (Workers == null) throw new ArgumentNullException(nameof(Workers));
            var workers = Workers.ToArray();
            if (workers.Length < 1) throw new ArgumentException($"{nameof(Workers)} should have a length of at least 1");
            HashFunc = HashFunc ?? DefaultHashFunction<U>();

            var router = spawn<T>(
                Name,
                msg =>
                {
                    var us = MapMany(msg);

                    foreach (var u in us)
                    {
                        var index = HashFunc(Children.Count, u);

                        var worker = workers[index];
                        fwd(worker, u);
                    }
                },
                Flags,
                DefaultStrategy,
                MaxMailboxSize,
                Terminated: pid => { workers = workers.Where(x => x != pid).ToArray(); }
            );
            return WatchWorkers(router, workers, Options);
        }

        /// <summary>
        /// Spawns a new process with that routes each message by calling GetHashCode on the 
        /// message and modding by the number of workers and using that as the worker index
        /// to route to.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId hash<T>(
            ProcessName Name,
            int Count,
            Action<T> Inbox,
            Func<int, T, int> HashFunc = null,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            ) =>
            hash<Unit, T>(Name, Count, () => unit, (_, msg) => { Inbox(msg); return unit; }, HashFunc, Flags, Strategy, MaxMailboxSize, WorkerName);

        /// <summary>
        /// Spawns a new process where each message is sent to a worker
        /// process by calling GetHashCode on the mapped message and 
        /// modding it by the number of workers to find the worker index 
        /// to route to.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        public static ProcessId hashMap<T, U>(
            ProcessName Name,
            int Count,
            Action<U> Inbox,
            Func<T, U> Map,
            Func<int, T, int> HashFunc = null,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            ) =>
            hashMap(Name, Count, () => unit, (_, umsg) => { Inbox(umsg); return unit; }, Map, HashFunc, Flags, Strategy, MaxMailboxSize, WorkerName);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is mapped 
        /// from T to IEnumerable U before each resulting U is passed to the worker
        /// processes by getting each mapped message and calling GetHashCode and modding 
        /// by the number of workers to find the worker index to route to.
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
        public static ProcessId hashMapMany<T, U>(
            ProcessName Name,
            int Count,
            Action<U> Inbox,
            Func<T, IEnumerable<U>> MapMany,
            Func<int, U, int> HashFunc = null,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            string WorkerName = "worker"
            ) =>
            hashMapMany(Name, Count, () => unit, (_, umsg) => { Inbox(umsg); return unit; }, MapMany, HashFunc, Flags, Strategy, MaxMailboxSize, WorkerName);

    }
}
