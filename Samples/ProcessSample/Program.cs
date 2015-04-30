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
//            TestBed.RunTests();
//            return;

            var ping = ProcessId.None;
            var pong = ProcessId.None;

            // Start a process which simply writes the messages it receives to std-out
            var logger = spawn<string>("logger", Console.WriteLine);

            // Ping process
            ping = spawn<string>("ping", msg =>
            {
                tell(logger, msg);
                tell(pong, "ping");
            });

            // Pong process
            pong = spawn<string>("pong", msg =>
            {
                tell(logger, msg);
                tell(ping, "pong");
            });

            // Trigger
            tell(pong, "start");

            Console.ReadKey();
        }
    }
}
