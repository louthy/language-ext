using System;
using System.Collections.Generic;
using System.Diagnostics;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed
{
    public static class SeqPerf
    {
        public static void Test1()
        {
            const int count = 10000000;

            // Warm up
            AddItems(new List<int>(), count);
            AddItems(Seq<int>(), count);

            var listSW = Stopwatch.StartNew();
            AddItems(new List<int>(), count);
            listSW.Stop();

            var seqSW = Stopwatch.StartNew();
            AddItems(Seq<int>(), count);
            seqSW.Stop();

            Console.WriteLine($"{count} items added to a List<T>: {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds/(float)count * 1000000.0:F3}ns per operation");
            Console.WriteLine($"{count} items added to a Seq<T>: {seqSW.ElapsedMilliseconds}ms, which is {(float)seqSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per operation");
        }

        public static void AddItems(List<int> list, int amount)
        {
            for(;amount >=0; amount--)
            {
                list.Add(amount);
            }
        }

        public static void AddItems(Seq<int> seq, int amount)
        {
            for (; amount >= 0; amount--)
            {
                seq = seq.Add(amount);
            }
        }

        public static void ConsItems(List<int> list, int amount)
        {
            for (; amount >= 0; amount--)
            {
                list.Add(amount);
            }
        }

        public static void ConsItems(Seq<int> seq, int amount)
        {
            for (; amount >= 0; amount--)
            {
                seq = seq.Add(amount);
            }
        }

    }
}
