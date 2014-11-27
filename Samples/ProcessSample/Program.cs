using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using LanguageExt;
using LanguageExt.Prelude;
using LanguageExt.Process;

namespace ProcessSample
{
    class Program
    {
        static void Main(string[] args)
        {
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
