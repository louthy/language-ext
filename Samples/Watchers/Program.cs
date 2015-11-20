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

            ping = spawn<string>(
                Name:      "ping",
                Inbox:     msg =>
                {
                    Console.WriteLine(msg);
                    tell(pong, "pong", 100*ms);
                },
                Terminate: pid => Console.WriteLine(pid + " is pushing up the daisies")
            );

            pong = spawn<string>(
                Name:     "pong",
                Inbox:     msg =>
                {
                    Console.WriteLine(msg);
                    tell(ping, "ping", 100*ms);
                },
                Terminate: pid => Console.WriteLine(pid + " is pushing up the daisies")
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
