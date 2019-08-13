////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed
{
    public static class HashMapPerf
    {
        const int count = 1000000;
        const int runs = 5;

        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nAdds from empty tests");
            Console.ForegroundColor = ConsoleColor.White;

            HashMapPerf.TestImmutableDictionaryFromEmptyAdds();
            HashMapPerf.TestDictionaryFromEmptyAdds();
            HashMapPerf.TestHashMapFromEmptyAdds();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nRandom access tests");
            Console.ForegroundColor = ConsoleColor.White;

            HashMapPerf.TestImmutableDictionaryRandomAccess();
            HashMapPerf.TestDictionaryRandomAccess();
            HashMapPerf.TestHashMapRandomAccess();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nEnumeration tests");
            Console.ForegroundColor = ConsoleColor.White;

            HashMapPerf.TestImmutableDictionaryEnum();
            HashMapPerf.TestDictionaryEnum();
            HashMapPerf.TestHashMapEnum();
        }

        public static void TestHashMapEnum()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestHashMapEnum");
            Console.ForegroundColor = ConsoleColor.White;


            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nStream enum run {i + 1}");

                var seq = Seq(Range(0, count));

                var map = toHashMap(seq.Map(x => (x, x)));

                var listSW = Stopwatch.StartNew();

                int j = 0;
                var results = new int[count];
                foreach (var item in map)
                {
                    results[j] = item.Value;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items streamed (HashMap) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestDictionaryEnum()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestDictionaryEnum");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nStream enum run {i + 1}");

                var seq = Seq(Range(0, count));

                var map = seq.Map(x => (x, x)).ToDictionary(x => x.Item1, x => x.Item2);

                var listSW = Stopwatch.StartNew();

                int j = 0;
                var results = new int[count];
                foreach (var item in map)
                {
                    results[j] = item.Value;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items streamed (Dictionary) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestImmutableDictionaryEnum()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestImmutableDictionaryEnum");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nStream enum run {i + 1}");

                var seq = Seq(Range(0, count));

                var map = ImmutableDictionary.CreateRange(seq.Map(x => new KeyValuePair<int, int>(x, x)));

                var listSW = Stopwatch.StartNew();

                int j = 0;
                var results = new int[count];
                foreach (var item in map)
                {
                    results[j] = item.Value;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items streamed (ImmutableDictionary) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }


        public static void TestHashMapRandomAccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestHashMapRandomAccess");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nRandom access run {i + 1}");

                var seq = Seq(Range(0, count));
                var map = toHashMap(seq.Map(x => (x, x)));

                var listSW = Stopwatch.StartNew();

                var results = new int[count];
                for(int j = 0; j < count; j++)
                {
                    results[j] = map[j];
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items accessed (HashMap) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestDictionaryRandomAccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestDictionaryRandomAccess");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nRandom access run {i + 1}");

                var seq = Seq(Range(0, count));
                var map = seq.Map(x => (x, x)).ToDictionary(x => x.Item1, x => x.Item2);

                var listSW = Stopwatch.StartNew();

                var results = new int[count];
                for (int j = 0; j < count; j++)
                {
                    results[j] = map[j];
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items accessed (Dictionary) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestImmutableDictionaryRandomAccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestImmutableDictionaryRandomAccess");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nRandom access run {i + 1}");

                var seq = Seq(Range(0, count));
                var map = ImmutableDictionary.CreateRange(seq.Map(x => new KeyValuePair<int, int>(x, x)));

                var listSW = Stopwatch.StartNew();

                var results = new int[count];
                for (int j = 0; j < count; j++)
                {
                    results[j] = map[j];
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == Range(0, count).Sum2());

                Console.WriteLine($"{count} items accessed (ImmutableDictionary) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestHashMapFromEmptyAdds()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestHashMapFromEmptyAdds");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nFrom empty adding run {i + 1}");

                var map = HashMap<int,int>();

                var listSW = Stopwatch.StartNew();

                var results = new int[count];
                for (int j = 0; j < count; j++)
                {
                    map = map.Add(j, j);
                    results[j] = j;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == map.Values.Sum2());

                Console.WriteLine($"{count} items added (HashMap) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestDictionaryFromEmptyAdds()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestDictionaryFromEmptyAdds");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nFrom empty adding run {i + 1}");

                var map = new Dictionary<int, int>();

                var listSW = Stopwatch.StartNew();

                var results = new int[count];
                for (int j = 0; j < count; j++)
                {
                    map.Add(j, j);
                    results[j] = j;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == map.Values.Sum2());

                Console.WriteLine($"{count} items added (Dictionary) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }

        public static void TestImmutableDictionaryFromEmptyAdds()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTestImmutableDictionaryFromEmptyAdds");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < runs; i++)
            {
                GC.Collect();

                Console.WriteLine($"\nFrom empty adding run {i + 1}");

                var map = ImmutableDictionary.Create<int, int>();

                var listSW = Stopwatch.StartNew();

                var results = new int[count];
                for (int j = 0; j < count; j++)
                {
                    map = map.Add(j, j);
                    results[j] = j;
                    j++;
                }

                listSW.Stop();

                Debug.Assert(results.Sum2() == map.Values.Sum2());

                Console.WriteLine($"{count} items added (ImmutableDictionary) : {listSW.ElapsedMilliseconds}ms, which is {(float)listSW.ElapsedMilliseconds / (float)count * 1000000.0:F3}ns per item");
            }
        }
    }
}
