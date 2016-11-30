using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace RedisWatcherA
{
    class Program
    {
        static void Main(string[] args)
        {
            var pong = ProcessId.Top["redis-watcher-b"]["user"]["pong"];

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            ProcessConfig.initialise("sys", "global", "redis-watcher-a", "localhost", "0");

            var ping = spawn<string>(
                Name: "ping",
                Inbox: msg =>
                {
                    Console.WriteLine(msg);
                    tell(pong, "pong", 1000 * ms);
                },
                Terminated: pid => Console.WriteLine(pid + " is pushing up the daisies"),
                Flags: ProcessFlags.PersistInbox
            );

            Console.WriteLine("Now run RedisWatcherB and press enter");
            Console.ReadKey();

            watch(ping, pong);
            tell(ping, "running");
            Console.ReadKey();
        }
    }
}
