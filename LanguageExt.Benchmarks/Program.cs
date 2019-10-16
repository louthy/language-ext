using System;
using BenchmarkDotNet.Running;

namespace LanguageExt.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary1 = BenchmarkRunner.Run<HashMapAddBenchmark>();
            Console.Write(summary1);

            var summary2 = BenchmarkRunner.Run<HashMapRandomReadBenchmark>();
            Console.Write(summary2);
        }
    }
}
