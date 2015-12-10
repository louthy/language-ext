using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    /// <summary>
    /// Each node in the cluster has a role name and at all times the cluster-nodes
    /// have a list of the alive nodes and their roles (Process.ClusterNodes).  Nodes 
    /// are removed from Process.ClusterNodes if they don't phone in. Process.ClusterNodes 
    /// is at most 3 seconds out-of-date and can therefore be used to reliably find
    /// out which nodes are available and what roles they do.  
    /// 
    /// By using Role.First, Role.Broadcast, Role.LeastBusy, Role.Random and Role.RoundRobin
    /// you can build a ProcessId that is resolved at the time of doing a 'tell', 'ask',
    /// 'subscribe', etc.  This can allow reliable messaging to Processes in the cluster.
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for
        /// operations like 'tell', the message is dispatched to all nodes in the set.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Broadcast["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Broadcast["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId Broadcast;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for
        /// operations like 'tell', the message is dispatched to the least busy from
        /// the set.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.LeastBusy["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.LeastBusy["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId LeastBusy;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for
        /// operations like 'tell', the message is dispatched to a cryptographically
        /// random node from the set.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Random["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Random["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId Random;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for
        /// operations like 'tell', the message is dispatched to the nodes in a round-
        /// robin fashion
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Random["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Random["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId RoundRobin;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for 
        /// operations like 'tell', the node names are sorted in ascending order and 
        /// the message is dispatched to the first one.  This can be used for leader
        /// election for example.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.First["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.First["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId First;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for 
        /// operations like 'tell', the node names are sorted in ascending order and 
        /// the message is dispatched to the second one.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Second["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Second["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId Second;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for 
        /// operations like 'tell', the node names are sorted in ascending order and 
        /// the message is dispatched to the third one.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Third["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Third["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId Third;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for 
        /// operations like 'tell', the node names are sorted in descending order and 
        /// the message is dispatched to the first one.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Last["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Last["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId Last;

        internal static Unit init()
        {
            // Triggers static ctor
            return unit;
        }

        /// <summary>
        /// Static ctor
        /// Sets up the default roles
        /// </summary>
        static Role()
        {
            ProcessName first       = "role-first";
            ProcessName second      = "role-second";
            ProcessName third       = "role-third";
            ProcessName last        = "role-last";
            ProcessName broadcast   = "role-broadcast";
            ProcessName leastBusy   = "role-least-busy";
            ProcessName random      = "role-random";
            ProcessName roundRobin  = "role-round-robin";

            var roleNodes = fun((ProcessId leaf) =>
                ClusterNodes.Values
                            .Filter(node => node.Role == leaf.Take(1).GetName())
                            .Map(node => ProcessId.Top[node.Role].Append(leaf.Skip(1))));

            // First
            First = Dispatch.register(first, leaf => roleNodes(leaf).Take(1));

            // Second
            Second = Dispatch.register(second, leaf => roleNodes(leaf).Skip(1).Take(1));

            // Third
            Third = Dispatch.register(third, leaf => roleNodes(leaf).Skip(2).Take(1));

            // Last
            Last = Dispatch.register(last, leaf => roleNodes(leaf).Reverse().Take(1));

            // Broadcast
            Broadcast = Dispatch.register(broadcast, roleNodes);

            // Least busy
            LeastBusy = Dispatch.register(leastBusy, leaf =>
                            roleNodes(leaf)
                                .Map(pid => Tuple(inboxCount(pid), pid))
                                .OrderBy(tup => tup.Item1)
                                .Map(tup => tup.Item2)
                                .Take(1));

            // Random
            Random = Dispatch.register(random, leaf => {
                var workers = roleNodes(leaf).ToArray();
                return new ProcessId[1] { workers[Prelude.random(workers.Length)] };
                });

            // Round-robin
            object sync = new object();
            Map<string, int> roundRobinState = Map.empty<string, int>();
            RoundRobin = Dispatch.register(roundRobin, leaf => {
                var key = leaf.ToString();
                var workers = roleNodes(leaf).ToArray();
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
