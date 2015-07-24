using System;
using LanguageExt;
using static LanguageExt.Process;

namespace RedisStateSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Log what's going on
            //ProcessLog.Subscribe(Console.WriteLine);

            RedisCluster.register();
            Cluster.connect("redis", new ClusterConfig("redis-test", "localhost", "0"));

            var pid = spawn<int, int>("redis-state-sample", () => 0, Inbox, ProcessFlags.PersistentState);

            observeState<int>(pid).Subscribe(Console.WriteLine);

            tell(pid, 1);

            Console.ReadKey();
        }

        static int Inbox(int state, int value)
        {
            state += value;
            tellSelf(value,TimeSpan.FromSeconds(1));
            return state;
        }
    }
}
