using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using LanguageExt.Prelude;
using LanguageExt.Process;
using System.Threading;

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

        /*

        Random testing


        static string check = "< WHY WHY WHY WHY WHY WHY WHY WHY WHY WHY ";

        static int CrashCount = 0;

        static string GetCheck()
        {
            return check;
        }

        static void Main(string[] args)
        {
            var logger = spawn<string>("logger", Console.WriteLine);

            var setter = spawn<HashSet<string>, string>(
                "store", 
                () =>
                {
                    for (var i = 0; i < 1; i++)
                    {
                        var subby = spawn<int, string>("subby" + i, () => 0, Subprocess);
                        tell(subby, GetCheck());
                    }

                    CrashCount++;
                    check = "< " + CrashCount + " blood";

                    return new HashSet<string>();
                }, 
                MyProcess
            );

            tell(setter, "Hello");

            for (var i = 0; i < 1000; i++)
            {
                tell(setter, "World "+i);
                //Thread.Sleep(1);
            }

            Console.ReadKey();
        }

        public static HashSet<string> MyProcess(HashSet<string> state, string message)
        {
            state.Add(message);

            if (state.Count % 10 == 0) tell("/system/logger", message + " received.  " + state.Count + " items in store");

            //if (state.Count % 10 == 9) kill();
            if (state.Count % 100 == 99) throw new Exception("craaaaaaaaaaaaash");

            return state;
        }

        public static int Subprocess(int state, string message)
        {
            if( state % 1000 == 0 ) tell("/system/logger", message);
            tell(self(), message);
            //Thread.Sleep(3000);
            return state + 1;
        }*/
    }
    }
