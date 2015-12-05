using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    public static partial class Role
    {
        public static readonly ProcessId Broadcast;
        public static readonly ProcessId LeastBusy;
        public static readonly ProcessId Random;
        public static readonly ProcessId RoundRobin;

        static Role()
        {
            ProcessName broadcast   = "role-broadcast";
            ProcessName leastBusy   = "role-least-busy";
            ProcessName random      = "role-random";
            ProcessName roundRobin  = "role-round-robin";

            var roleNodes = fun((ProcessId leaf) =>
                ClusterNodes.Filter(node => node.Role == leaf.Take(1).GetName())
                            .Map(node => ProcessId.Top[node.Role].Append(leaf.Skip(1))));

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
