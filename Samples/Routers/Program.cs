using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Threading;

namespace Routers
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            Test2();
            Test3();
        }

        static void Test1()
        {
            var test = Router.broadcast("broadcast1", 10, (string msg) => Console.WriteLine(msg));
            tell(test, "Hello");

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        static void Test2()
        {
            var hello = spawn("hello", (string msg) => Console.WriteLine(msg + " Hello"));
            var world = spawn("world", (string msg) => Console.WriteLine(msg + " World"));

            var test = Router.broadcast<string>(
                "broadcast2", 
                new[] { hello, world }, 
                Options: RouterOption.RemoveWorkerWhenTerminated
                );

            tell(test, "--> ");

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        static void Test3()
        {
            var one = spawn("one", (string msg) => { Thread.Sleep(01); Console.WriteLine("1. " + msg); });
            var two = spawn("two", (string msg) => { Thread.Sleep(25); Console.WriteLine("\t2. " + msg); });
            var thr = spawn("thr", (string msg) => { Thread.Sleep(50); Console.WriteLine("\t\t3. " + msg); });

            var test = Router.leastBusy<string>("least", new[] { one, two, thr });

            Range(0, 100).Iter(x => tell(test, "testing " + x));

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
