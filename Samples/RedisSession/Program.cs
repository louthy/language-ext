using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace RedisSession
{
    class Program
    {
        static void Main(string[] args)
        {
            // Remove this to get on-screen logging
            ProcessLog.Subscribe(Console.WriteLine);

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-session-test", "localhost:6379", "0");

            var ping = ProcessId.None;
            var pong = ProcessId.None;

            // Start a process which simply writes the messages it receives to std-out
            var logger = spawn<string>("logger", Console.WriteLine);

            sessionStart("xyz", 20*seconds);

            // Ping process
            ping = spawn<string>("ping", msg =>
            {
                tell(logger, msg);

                var txt = hasSession()
                    ? sessionId().ToString()
                    : "expired";

                tell(pong, "ping-"+txt, TimeSpan.FromMilliseconds(1000));
            });

            // Pong process
            pong = spawn<string>("pong", msg =>
            {
                tell(logger, msg);

                var txt = hasSession()
                    ? sessionId().ToString()
                    : "expired";

                tell(ping, "pong-" + txt, TimeSpan.FromMilliseconds(1000));
            });

            // Trigger
            tell(pong, "start");


            Console.ReadKey();
        }
    }
}
