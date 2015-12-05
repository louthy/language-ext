using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    public partial class Dispatch
    {
        /// <summary>
        /// Registers a dispatcher for a role
        /// Dispatchers take in a 'leaf' ProcessId (i.e. /user/my-process) and return an enumerable
        /// of real ProcessIds that the Process system will use to deliver all of the standard functions
        /// like tell, ask, subscribe, etc.
        /// </summary>
        /// <param name="name">Name of the dispatcher</param>
        /// <param name="selector">Function that will be invoked every time a dispatcher based ProcessId
        /// is used.</param>
        /// <returns>A root dispatcher ProcessId.  Use this to create new ProcessIds that will
        /// be passed to the selector function whenever the dispatcher based ProcessId is used</returns>
        public static ProcessId register(ProcessName name, Func<ProcessId, IEnumerable<ProcessId>> selector) =>
            ActorContext.AddDispatcher(name, selector);

        /// <summary>
        /// Removes the dispatcher registration for the named dispatcher
        /// </summary>
        /// <param name="name">Name of the dispatcher to remove</param>
        public static Unit deregister(ProcessName name) =>
            ActorContext.RemoveDispatcher(name);

        public static ProcessId broadcast(IEnumerable<ProcessId> processIds) =>
            Broadcast.Append(String.Join(",", processIds.Map(x => x.Path)));

        public static ProcessId leastBusy(IEnumerable<ProcessId> processIds) =>
            LeastBusy.Append(String.Join(",", processIds.Map(x => x.Path)));

        public static ProcessId random(IEnumerable<ProcessId> processIds) =>
            Random.Append(String.Join(",", processIds.Map(x => x.Path)));

        public static ProcessId roundRobin(IEnumerable<ProcessId> processIds) =>
            RoundRobin.Append(String.Join(",", processIds.Map(x => x.Path)));

        public static ProcessId broadcast(ProcessId processId, params ProcessId[] processIds) =>
            Broadcast.Append(String.Join(",", processId.Cons(processIds).Map(x => x.Path)));

        public static ProcessId leastBusy(ProcessId processId, params ProcessId[] processIds) =>
            LeastBusy.Append(String.Join(",", processId.Cons(processIds).Map(x => x.Path)));

        public static ProcessId random(ProcessId processId, params ProcessId[] processIds) =>
            Random.Append(String.Join(",", processId.Cons(processIds).Map(x => x.Path)));

        public static ProcessId roundRobin(ProcessId processId, params ProcessId[] processIds) =>
            RoundRobin.Append(String.Join(",", processId.Cons(processIds).Map(x => x.Path)));

        static readonly ProcessId Broadcast;
        static readonly ProcessId LeastBusy;
        static readonly ProcessId Random;
        static readonly ProcessId RoundRobin;

        static Dispatch()
        {
            ProcessName broadcast  = "broadcast";
            ProcessName leastBusy  = "least-busy";
            ProcessName random     = "random";
            ProcessName roundRobin = "round-robin";

            var processes = fun((ProcessId leaf) => leaf.Path.Split(',').Map(x => new ProcessId(x)));

            // Broadcast
            Broadcast = Dispatch.register(broadcast, processes);

            // Least busy
            LeastBusy = Dispatch.register(leastBusy, leaf =>
                            processes(leaf)
                                .Map(pid => Tuple(inboxCount(pid), pid))
                                .OrderBy(tup => tup.Item1)
                                .Map(tup => tup.Item2)
                                .Take(1));

            // Random
            Random = Dispatch.register(random, leaf => {
                var workers = processes(leaf).ToArray();
                return new ProcessId[1] { workers[Prelude.random(workers.Length)] };
            });

            // Round-robin
            object sync = new object();
            Map<string, int> roundRobinState = Map.empty<string, int>();
            RoundRobin = Dispatch.register(roundRobin, leaf => {
                var key = leaf.ToString();
                var workers = processes(leaf).ToArray();
                int index = 0;
                lock (sync)
                {
                    roundRobinState = roundRobinState.AddOrUpdate(key, x => { index = x % workers.Length; return x + 1; }, 0);
                }
                return new ProcessId[1] { workers[index] };
            });
        }
    }
}
