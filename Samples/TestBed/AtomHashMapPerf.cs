using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed
{
    public class AtomHashMapPerf
    {
        const int iterations = 100000;
        const int parallelism = 32;
        static readonly object sync = new object();
        
        public static async Task Test()
        {
            // Warm up the types
            
            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
            
            var xs = HashMap<int, int>();
            xs = xs.Add(1, 1);
            xs = xs.Remove(1);

            var zs = AtomHashMap<int, int>();
            zs.Add(1, 1);
            zs.Remove(1);

            // Test a regular hash-map protected with locks

            sw1.Start();
            await Task.WhenAll(Range(0, parallelism).SequenceParallel(x => Task.Run<Unit>(() => sumItemsLocking(x))));
            sw1.Stop();
            

            // Test a lock-free atomic hash-map

            sw2.Start();
            await Task.WhenAll(Range(0, parallelism).SequenceParallel(x => Task.Run<Unit>(() => sumItemsAtomic(x))));
            sw2.Stop();

            // Show results
            
            Console.WriteLine($"{sw1.ElapsedMilliseconds}ms");
            Console.WriteLine($"{sw2.ElapsedMilliseconds}ms");
            
            Debug.Assert(xs.ForAll(static x => x == iterations));
            Debug.Assert(zs.ForAll(static z => z == iterations));

            Unit sumItemsLocking(int x)
            {
                for (var ix = 0; ix < iterations; ix++)
                {
                    lock (sync)
                    {
                        xs = xs.AddOrUpdate(x, Some: static v => v + 1, None: 1);
                    }
                }
                return unit;
            }

            Unit sumItemsAtomic(int x)
            {
                for (var ix = 0; ix < iterations; ix++)
                {
                    zs.AddOrUpdate(x, Some: static v => v + 1, None: 1);
                }
                return unit;
            }
        }
    }
}
