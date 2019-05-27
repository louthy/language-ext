using System;
using System.Collections.Generic;
using System.Diagnostics;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed
{
    public static class SeqPerf
    {
        public static void TestAdd()
        {
            const int count = 10000000;

            // Warm up
            AddItems(new List<int>(), count);
            AddItems(Seq<int>(), count);

            for (int i = 0; i < 10; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nRun {i + 1}\n");

                // List
                var listSW = Stopwatch.StartNew();
                var list = new List<int>();
                AddItems(list, count);
                listSW.Stop();

                // Seq
                var seqSW = Stopwatch.StartNew();
                var seq = Seq<int>();
                AddItems(Seq<int>(), count);
                seqSW.Stop();

                // Check
                AssertAddItems(seq, count);

                Console.WriteLine($"{count} items added to a List<T>: {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per operation");
                Console.WriteLine($"{count} items added to a Seq<T>: {seqSW.ElapsedMilliseconds}ms, which is {(float)seqSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per operation");
            }
        }

        public static void TestCons()
        {
            const int count = 100000;

            // Warm up
            AddItems(new List<int>(), count);
            AddItems(Seq<int>(), count);

            for (int i = 0; i < 10; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nRun {i + 1}\n");

                // List
                var listSW = Stopwatch.StartNew();
                var list = new List<int>();
                ConsItems(list, count);
                listSW.Stop();

                // Seq
                var seqSW = Stopwatch.StartNew();
                var seq = Seq<int>();
                ConsItems(Seq<int>(), count);
                seqSW.Stop();

                // Check
                AssertConsItems(seq, count);

                Console.WriteLine($"{count} items added to a List<T>: {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per operation");
                Console.WriteLine($"{count} items added to a Seq<T>: {seqSW.ElapsedMilliseconds}ms, which is {(float)seqSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per operation");
            }
        }

        static void AssertAddItems(Seq<int> seq, int amount)
        {
            Debug.Assert(seq.Count == amount);

            for (; amount >= 0; amount--)
            {
                Debug.Assert(seq.Head == amount);
                seq = seq.Tail;
            }
        }

        static void AssertConsItems(Seq<int> seq, int amount)
        {
            Debug.Assert(seq.Count == amount);

            for (var i = 0; i < amount; i++)
            {
                Debug.Assert(seq.Head == i);
                seq = seq.Tail;
            }
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
                list.Insert(0, amount);
            }
        }

        public static void ConsItems(Seq<int> seq, int amount)
        {
            for (; amount >= 0; amount--)
            {
                seq = amount.Cons(seq);
            }
        }

    }
}
