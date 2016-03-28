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

            Process.configureFromFile();

            var ping = ProcessId.None;
            var pong = ProcessId.None;

            // Start a process which simply writes the messages it receives to std-out
            var logger = spawn<string>("logger", x => Console.WriteLine(x));

            // Ping process
            ping = spawn<string>("ping", msg =>
            {
                tell(logger, msg);

                var res = settingList<int>("arr");

                tell(pong, $"ping {setting("name", "")}", TimeSpan.FromMilliseconds(100));
            });

            // Pong process
            pong = spawn<string>("pong", msg =>
            {
                tell(logger, msg);

                var map = settingMap<string>("map");

                tell(ping, $"pong {setting("name", "")}", TimeSpan.FromMilliseconds(100));
            });

            // Trigger
            tell(pong, "start");


            Console.ReadKey();
        }
    }
}
