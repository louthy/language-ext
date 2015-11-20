using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace Watchers
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
        }

        static void Test1()
        {
            ProcessId pong = ProcessId.None;
            ProcessId ping = ProcessId.None;

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-watchers", "localhost", "0");

            ping = spawn<string>(
                Name:      "ping",
                Inbox:     msg =>
                {
                    Console.WriteLine(msg);
                    tell(pong, "pong", 100*ms);
                },
                Terminated: pid => Console.WriteLine(pid + " is pushing up the daisies"),
                Flags: ProcessFlags.PersistInbox
            );

            pong = spawn<string>(
                Name:     "pong",
                Inbox:     msg =>
                {
                    Console.WriteLine(msg);
                    tell(ping, "ping", 100*ms);
                },
                Terminated: pid => Console.WriteLine(pid + " is pushing up the daisies"),
                Flags: ProcessFlags.PersistInbox
            );

            watch(ping, pong);
            watch(pong, ping);

            tell(pong, "Running");

            Console.WriteLine("Press key to kill ping");
            Console.ReadKey();
            kill(ping);
            Console.WriteLine("Press key to kill pong");
            Console.ReadKey();
            kill(pong);
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }
    }
}
