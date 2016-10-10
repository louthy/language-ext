using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Process;

namespace RedisSubscribeSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Log what's going on
            // ProcessLog.Subscribe(Console.WriteLine);

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            ProcessConfig.initialise("sys", "global", "redis-subscribe-test", "localhost", "0");

            var pid = ProcessId.Top["redis-publish-test"]["user"]["redis-pubsub-random-test"];

            // Listen to the published results coming back from the Redis channel
            subscribe<int>(pid, Console.WriteLine);

            Console.ReadKey();
        }
    }
}
