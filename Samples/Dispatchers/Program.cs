using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Process;

namespace Dispatchers
{
    class Program
    {
        static void Main(string[] args)
        {
            //RedisCluster.register();
            //Cluster.connect("redis", "disp-test", "localhost", "0", "dispatch-role");

            var pida = spawn<int>("A", x => Console.WriteLine("A" + x));
            var pidb = spawn<int>("B", x => Console.WriteLine("B" + x));
            var pidc = spawn<int>("C", x => Console.WriteLine("C" + x));

            Console.WriteLine("Press the numeric keys 1 to 4 to select the type of dispatch");
            Console.WriteLine("1. Broadcast");
            Console.WriteLine("2. Least busy");
            Console.WriteLine("3. Round robin");
            Console.WriteLine("4. Random");

            int i = 0;
            while (true)
            {
                ProcessId pid = ProcessId.None;

                switch (Console.ReadKey().KeyChar)
                {
                    case '1': pid = Dispatch.broadcast(pida, pidb, pidc); break;
                    case '2': pid = Dispatch.leastBusy(pida, pidb, pidc); break;
                    case '3': pid = Dispatch.roundRobin(pida, pidb, pidc); break;
                    case '4': pid = Dispatch.random(pida, pidb, pidc); break;
                }
                Console.WriteLine();

                if (pid.IsValid)
                {
                    tell(pid, i);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
