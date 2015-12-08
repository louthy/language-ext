using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    public class Dispatch
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

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used for
        /// operations like 'tell', the message is dispatched to all Processes in 
        /// the set.
        /// </summary>
        /// <example>
        ///     tell( Dispatch.broadcast(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId broadcast(IEnumerable<ProcessId> processIds) =>
            Broadcast[processIds];

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used 
        /// for operations like 'tell', the message is dispatched to the least busy
        /// Process from the set.
        /// </summary>
        /// <example>
        ///     tell( Dispatch.leastBusy(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId leastBusy(IEnumerable<ProcessId> processIds) =>
            LeastBusy[processIds];

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used 
        /// for operations like 'tell', the message is dispatched to a cryptographically
        /// random Process from the set.
        /// </summary>
        /// <example>
        ///     tell( Dispatch.random(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId random(IEnumerable<ProcessId> processIds) =>
            Random[processIds];

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used 
        /// for operations like 'tell', the message is dispatched to the Processes in a 
        /// round-robin fashion
        /// </summary>
        /// <example>
        ///     tell( Dispatch.roundRobin(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId roundRobin(IEnumerable<ProcessId> processIds) =>
            RoundRobin[processIds];

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used for
        /// operations like 'tell', the message is dispatched to all Processes in 
        /// the set.
        /// </summary>
        /// <example>
        ///     tell( Dispatch.broadcast(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId broadcast(ProcessId processId, params ProcessId[] processIds) =>
            Broadcast[processId.Cons(processIds)];

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used 
        /// for operations like 'tell', the message is dispatched to the least busy
        /// Process from the set.
        /// </summary>
        /// <example>
        ///     tell( Dispatch.leastBusy(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId leastBusy(ProcessId processId, params ProcessId[] processIds) =>
            LeastBusy[processId.Cons(processIds)];

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used 
        /// for operations like 'tell', the message is dispatched to a cryptographically
        /// random Process from the set.
        /// </summary>
        /// <example>
        ///     tell( Dispatch.random(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId random(ProcessId processId, params ProcessId[] processIds) =>
            Random[processId.Cons(processIds)];

        /// <summary>
        /// Builds a ProcessId that represents a set of Processes.  When used 
        /// for operations like 'tell', the message is dispatched to the Processes in a 
        /// round-robin fashion
        /// </summary>
        /// <example>
        ///     tell( Dispatch.roundRobin(pid1,pid2,pid3), "Hello" );
        /// </example>
        public static ProcessId roundRobin(ProcessId processId, params ProcessId[] processIds) =>
            RoundRobin[processId.Cons(processIds)];

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

            var processes = fun((ProcessId leaf) => leaf.GetSelection());

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
