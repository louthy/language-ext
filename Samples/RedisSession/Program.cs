using System;
using System.Linq;
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
            ProcessSystemLog.Subscribe(Console.WriteLine);

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            ProcessConfig.initialiseFileSystem("session-test-1");

            var ping = ProcessId.None;
            var pong = ProcessId.None;

            // Start a process which simply writes the messages it receives to std-out
            var logger = spawn<string>("logger", x => Console.WriteLine(x), ProcessFlags.PersistInbox);

            sessionStart("xyz", 20 * seconds, "live");

            // Ping process
            ping = spawn<string>("ping", msg =>
            {
                tell(logger, msg);

                var txt = hasSession()
                    ? sessionId().ToString()
                    : "expired";

                var val = sessionGetData<int>("test");
                sessionSetData("test", val.FirstOrDefault() + 1);
                
                tell(pong, $"ping-{txt}-{val.FirstOrDefault()}", TimeSpan.FromMilliseconds(1000));
            }, ProcessFlags.PersistInbox);

            // Pong process
            pong = spawn<string>("pong", msg =>
            {
                tell(logger, msg);

                var txt = hasSession()
                    ? sessionId().ToString()
                    : "expired";

                var val = sessionGetData<int>("test");
                sessionSetData("test", val.FirstOrDefault() + 1);

                tell(ping, $"pong-{txt}-{val.FirstOrDefault()}", TimeSpan.FromMilliseconds(1000));
            }, ProcessFlags.PersistInbox);

            // Trigger
            tell(pong, "start");

            Console.ReadKey();
        }
    }
}
