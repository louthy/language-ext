using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace Throughput
{
    class Program
    {
        static void Main(string[] args)
        {
            int pairs = args.Count() == 0 ? 16 : Int32.Parse(args[0]);
            int msgs = args.Count() == 2 ? Int32.Parse(args[1]) : 10;
            int sleepForMax = 20;

            var counters = new Counter[pairs];

            InitCounters(pairs, counters, msgs);

            int sleepFor = sleepForMax;
            while (sleepFor > 0)
            {
                Console.WriteLine("Sleeping for "+sleepFor+" seconds whilst it warms up");
                Thread.Sleep(1000);
                sleepFor--;
            }

            Console.WriteLine("Warm up sent "+ SumCounters(pairs,counters) + " messages. Running for real now...");
            ResetCounters(pairs, counters);

            Thread.Sleep(1000 * sleepForMax);

            shutdownAll();

            var sum = SumCounters(pairs, counters);

            Console.WriteLine("" + sum + " messages sent in "+ sleepForMax + " seconds.");

            decimal sps = ((decimal)sum) / ((decimal)sleepForMax);

            Console.WriteLine("That's " + sps + " messages per second");

            Console.ReadKey();
        }

        private static long SumCounters(int pairs, Counter[] counters)
        {
            long sum = 0;
            for (var i = 0; i < pairs; i++)
            {
                sum += counters[i].Count1 + counters[i].Count2;
            }
            return sum;
        }

        private static void InitCounters(int pairs, Counter[] counters, int messages)
        {
            for (var i = 0; i < pairs; i++)
            {
                counters[i] = new Counter { Count1 = 0, Count2 = 0 };
                StartPair(counters[i], i, messages);
            }
        }

        private static void ResetCounters(int pairs, Counter[] counters)
        {
            for (var i = 0; i < pairs; i++)
            {
                counters[i].Count1 = 0;
                counters[i].Count2 = 0;
            }
        }

        private static void StartPair(Counter counter, int index, int messages)
        {
            var ping = ProcessId.None;
            var pong = ProcessId.None;

            // Ping process
            ping = spawn<Unit>("ping-"+ index, msg =>
            {
                counter.Count1++;
                tell(pong, unit);
            });

            // Pong process
            pong = spawn<Unit>("pong-" + index, msg =>
            {
                counter.Count2++;
                tell(ping, unit);
            });

            for (int i = 0; i < messages; i++)
            {
                pong.Tell(unit);
            }
        }

        private class Counter
        {
            public long Count1;
            public long Count2;
        }
    }
}
