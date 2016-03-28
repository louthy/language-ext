using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace ProcessSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Remove this to get on-screen logging
            ProcessSystemLog.Subscribe(Console.WriteLine);

            RedisCluster.register();
            //Cluster.connect("redis", "ping-pong", "localhost", "0", "ping-pong");
            //Process.readConfigFromCluster();
            //Process.readConfigFromFile();
            //Process.writeConfigToCluster();
            //if ( readSetting("name","") == "" )
            //{
            //    writeSetting("name", "Paul");
            //}
            //else
            //{
            //    Console.WriteLine(readSetting("name", ""));
            //    writeSetting("name", "Paul");
            //}

            var ping = ProcessId.None;
            var pong = ProcessId.None;

            // Start a process which simply writes the messages it receives to std-out
            var logger = spawn<string>("logger", x => Console.WriteLine(x));

            // Ping process
            ping = spawn<string>("ping", msg =>
            {
                tell(logger, msg);

                var res = readListSetting<int>("arr");
                var name = readSetting("name", "");
                var ind = readSetting("count", 0);
                writeSetting("count", ind + 1);

                tell(pong, $"ping {name}-{ind}", TimeSpan.FromMilliseconds(100));
            });

            // Pong process
            pong = spawn<string>("pong", msg =>
            {
                tell(logger, msg);

                var map = readMapSetting<string>("map");
                var name = readSetting("name", "");
                var ind = readSetting("count", 0);
                writeSetting("count", ind + 1);

                tell(ping, $"pong {name}-{ind}", TimeSpan.FromMilliseconds(100));
            });

            // Trigger
            tell(pong, "start");


            Console.ReadKey();
        }
    }
}
