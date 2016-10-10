using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using static LanguageExt.ProcessConfig;
using LanguageExt.Config;

namespace ProcessSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Remove this to get on-screen logging
            ProcessSystemLog.Subscribe(Console.WriteLine);

            RedisCluster.register();
            ProcessConfig.initialiseFileSystem("ping-pong-1");

            var ping = ProcessId.None;
            var pong = ProcessId.None;

            Process.PreShutdown.Subscribe(_ =>
            {
                kill(ping);
                kill(pong);
            });

            // Start a process which simply writes the messages it receives to std-out
            var logger = spawn<string>("logger", x => Console.WriteLine(x));

            // Ping process
            ping = spawn<string>("ping", msg =>
            {
                tell(logger, msg);

                var res = readList<int>("arr");
                var name = read("name", "");
                var ind = read("count", 0);
                write("count", ind + 1);

                tell(pong, $"ping {name}-{ind}", TimeSpan.FromMilliseconds(100));
            });

            // Pong process
            pong = spawn<string>("pong", msg =>
            {
                tell(logger, msg);

                var map = readMap<string>("map");
                var name = read("name", "");
                var ind = read("count", 0);
                write("count", ind + 1);

                tell(ping, $"pong {name}-{ind}", TimeSpan.FromMilliseconds(100));
            });
                        
            // Trigger
            tell(ping, "start");

            Console.ReadKey();

            Process.shutdownAll();
        }
    }
}
