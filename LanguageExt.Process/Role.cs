using System.Collections.Generic;
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
        /// Find out the role that this node is a part of
        /// </summary>
        public static ProcessName Current =>
            Root.Take(1).GetName();

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for
        /// operations like 'tell', the message is dispatched to all nodes in the set.
        /// See remarks.
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
        /// See remarks.
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
        /// See remarks.
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
        /// See remarks.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.RoundRobin["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.RoundRobin["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId RoundRobin;

        /// <summary>
        /// A ProcessId that represents a set of nodes in a cluster.  When used for 
        /// operations like 'tell', the node names are sorted in ascending order and 
        /// the message is dispatched to the first one.  This can be used for leader
        /// election for example.
        /// See remarks.
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
        /// See remarks.
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
        /// See remarks.
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
        /// See remarks.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Last["my-role"]["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Last["message-role"]["user"]["message-log"], "Hello" );
        /// </example>
        public static readonly ProcessId Last;

        /// <summary>
        /// Builds a ProcessId that represents the next node in the role that this node
        /// is a part of.  If there is only one node in the role then any messages sent
        /// will be sent to the leaf-process with itself.  Unlike other Roles, you do 
        /// not specify the role-name as the first child. 
        /// See remarks.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Next["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Next["user"]["message-log"], "Hello" );
        /// </example>
        public static ProcessId Next =>
            nextRoot[Root.GetName()];

        /// <summary>
        /// Builds a ProcessId that represents the previous node in the role that this 
        /// node is a part of.  If there is only one node in the role then any messages 
        /// sent will be sent to the leaf-process with itself.  Unlike other Roles, you 
        /// do not specify the role-name as the first child. 
        /// See remarks.
        /// </summary>
        /// <remarks>
        /// You may create a reference to child nodes in the usual way:
        ///     Role.Prev["user"]["child-1"][...]
        /// </remarks>
        /// <example>
        ///     tell( Role.Prev["user"]["message-log"], "Hello" );
        /// </example>
        public static ProcessId Prev =>
            prevRoot[Root.GetName()];

        public static IEnumerable<ProcessId> NodeIds(ProcessId leaf) =>
            Nodes(leaf).Values.Map(node => ProcessId.Top[node.NodeName].Append(leaf.Skip(1)));

        public static Map<ProcessName, ClusterNode> Nodes(ProcessId leaf) =>
            ClusterNodes.Filter(node => node.Role == leaf.Take(1).GetName());

        static readonly ProcessId nextRoot;
        static readonly ProcessId prevRoot;

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
            ProcessName next        = "role-next";
            ProcessName prev        = "role-prev";
            ProcessName broadcast   = "role-broadcast";
            ProcessName leastBusy   = "role-least-busy";
            ProcessName random      = "role-random";
            ProcessName roundRobin  = "role-round-robin";

            var nextNode = fun((bool fwd) => fun((ProcessId leaf) =>
            {
                var self = leaf.Take(1).GetName();
                var isNext = false;
                var nodeMap = Nodes(leaf);

                var nodes = fwd
                    ? nodeMap.Values.Append(nodeMap.Values)
                    : nodeMap.Values.Append(nodeMap.Values).Reverse(); //< TODO: Inefficient

                foreach (var node in nodes)
                {
                    if (isNext)
                    {
                        return new[] { ProcessId.Top[node.NodeName].Append(leaf.Skip(1)) }.AsEnumerable();
                    }

                    if (node.NodeName == self)
                    {
                        isNext = true;
                    }
                }
                return new ProcessId[0].AsEnumerable();
            }));

            // Next 
            nextRoot = Dispatch.register(next, nextNode(true));

            // Prev 
            prevRoot = Dispatch.register(prev, nextNode(false));

            // First
            First = Dispatch.register(first, leaf => NodeIds(leaf).Take(1));

            // Second
            Second = Dispatch.register(second, leaf => NodeIds(leaf).Skip(1).Take(1));

            // Third
            Third = Dispatch.register(third, leaf => NodeIds(leaf).Skip(2).Take(1));

            // Last
            Last = Dispatch.register(last, leaf => NodeIds(leaf).Reverse().Take(1));

            // Broadcast
            Broadcast = Dispatch.register(broadcast, NodeIds);

            // Least busy
            LeastBusy = Dispatch.register(leastBusy, leaf =>
                            NodeIds(leaf)
                                .Map(pid => Tuple(inboxCount(pid), pid))
                                .OrderBy(tup => tup.Item1)
                                .Map(tup => tup.Item2)
                                .Take(1));

            // Random
            Random = Dispatch.register(random, leaf => {
                var workers = NodeIds(leaf).ToArray();
                return new ProcessId[1] { workers[Prelude.random(workers.Length)] };
                });

            // Round-robin
            object sync = new object();
            Map<string, int> roundRobinState = Map.empty<string, int>();
            RoundRobin = Dispatch.register(roundRobin, leaf => {
                var key = leaf.ToString();
                var workers = NodeIds(leaf).ToArray();
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
