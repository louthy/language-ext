using System;
using LanguageExt;
using static LanguageExt.Process;

namespace RedisStateSample
{
    class Program
    {
        //
        //  Launch a process that adds the int sent in a message to its state
        //  Then calls itself after a second to do the same again.  The state value gradually
        //  increases.  
        //
        //  If you stop the sample and restart you'll notice the state has been persisted
        //
        static void Main(string[] args)
        {
            // Log what's going on
            // ProcessLog.Subscribe(Console.WriteLine);

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-test", "localhost", "0");

            // Spawn the process
            var pid = spawn<int, int>("redis-state-sample", Setup, Inbox, ProcessFlags.PersistState);

            // Subscribe locally to the state changes
            observeState<int>(pid).Subscribe(Console.WriteLine);

            // Start it off by sending the first message
            tell(pid, 1);

            Console.ReadKey();
        }

        //
        //  Get the initial state of the process
        //
        static int Setup() => 0;

        // 
        //  Inbox message handleer
        // 
        static int Inbox(int state, int value)
        {
            state += value;
            tellSelf(value,TimeSpan.FromSeconds(1));
            return state;
        }
    }
}
