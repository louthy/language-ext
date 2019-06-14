using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed
{
    public static class SeqPerf
    {
        const int runs = 5;

        public static void LazyTest()
        {
            var seq = Seq(Range(0, 5)).Strict();
        }

        public static int Sum2(this IEnumerable<int> ma) =>
            ma.Fold(0, (s, x) => s + x);

        public static void TestStrict()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nTestStrict");
            Console.ForegroundColor = ConsoleColor.White;

            const int count = 10000000;

            GC.Collect();

            for (int i = 0; i < runs; i++)
            {
                Console.WriteLine($"\nStream strict run {i + 1}");

                var listSW = Stopwatch.StartNew();

                var seq = Seq(Range(0, count));
                seq = seq.Strict();

                listSW.Stop();

                Console.WriteLine($"{count} items streamed (Strict) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestStrictEnum()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nTestStrictEnum");
            Console.ForegroundColor = ConsoleColor.White;

            const int count = 10000000;

            GC.Collect();

            for (int i = 0; i < runs; i++)
            {
                Console.WriteLine($"\nStream strict enum run {i + 1}");

                var seq = Seq(Range(0, count));
                seq = seq.Strict();

                var listSW = Stopwatch.StartNew();

                int j = 0;
                var results = new int[count];
                foreach (var item in seq)
                {
                    results[j] = item;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items streamed (Strict enum) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }


        public static void TestListTEnum()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nTestListTEnum");
            Console.ForegroundColor = ConsoleColor.White;

            const int count = 10000000;

            GC.Collect();

            for (int i = 0; i < runs; i++)
            {
                Console.WriteLine($"\nStream strict enum run {i + 1}");

                var seq = Range(0, count);
                seq = seq.ToList();

                var listSW = Stopwatch.StartNew();

                int j = 0;
                var results = new int[count];
                foreach (var item in seq)
                {
                    results[j] = item;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items streamed (List<T> enum) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestSeqStream()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nTestSeqStream");
            Console.ForegroundColor = ConsoleColor.White;

            const int count = 10000000;
            var results = new int[count];

            for (int i = 0; i < runs; i++)
            {
                Console.WriteLine($"\nStream lazy Seq run {i + 1}");

                GC.Collect();

                var listSW = Stopwatch.StartNew();

                int j = 0;
                var seq = Seq(Range(0, count));
                foreach(var item in seq)
                {
                    results[j] = item;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());
                Console.WriteLine($"{count} items streamed (lazy seq) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestEnumStream()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nTestEnumStream");
            Console.ForegroundColor = ConsoleColor.White;

            const int count = 10000000;
            var results = new int[count];

            for (int i = 0; i < runs; i++)
            {
                Console.WriteLine($"\nStream lazy enum run {i + 1}");

                GC.Collect();

                var listSW = Stopwatch.StartNew();

                int j = 0;
                var seq = Range(0, count);
                foreach (var item in seq)
                {
                    results[j] = item;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items streamed (lazy enum) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestAdd()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nTestAdd");
            Console.ForegroundColor = ConsoleColor.White;

            const int count = 10000000;

            // Warm up
            AddItems(new List<int>(), count);
            AddItems(Seq<int>(), count);

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nRun {i + 1}");

                // List
                var listSW = Stopwatch.StartNew();
                var list = new List<int>();
                AddItems(list, count);
                listSW.Stop();

                GC.Collect();

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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nTestCons");
            Console.ForegroundColor = ConsoleColor.White;

            const int count = 100000;

            // Warm up
            AddItems(new List<int>(), count);
            AddItems(Seq<int>(), count);

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nRun {i + 1}");

                // List
                var listSW = Stopwatch.StartNew();
                var list = new List<int>();
                ConsItems(list, count);
                listSW.Stop();

                GC.Collect();

                // Seq
                var seqSW = Stopwatch.StartNew();
                var seq = Seq<int>();
                ConsItems(Seq<int>(), count);
                seqSW.Stop();

                // Check
                AssertConsItems(seq, count);

                Console.WriteLine($"{count} items consd to a List<T>: {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per operation");
                Console.WriteLine($"{count} items consd to a Seq<T>: {seqSW.ElapsedMilliseconds}ms, which is {(float)seqSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per operation");
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
