using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace RedisWatcherB
{
    class Program
    {
        static void Main(string[] args)
        {
            var ping = ProcessId.Top["redis-watcher-a"]["user"]["ping"];

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-watcher-b", "localhost", "0", "global");

            var pong = spawn<string>(
                Name: "pong",
                Inbox: msg =>
                {
                    Console.WriteLine(msg);
                    tell(ping, "ping", 1000 * ms);
                },
                Terminated: pid => Console.WriteLine(pid + " is pushing up the daisies"),
                Flags: ProcessFlags.PersistInbox
            );

            watch(pong, ping);

            Console.ReadKey();
        }
    }
}
