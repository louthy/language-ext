using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int))]
    [GenericTypeArguments(typeof(string))]
    public class HashSetAddBenchmarks<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        T[] values;

        [GlobalSetup]
        public void Setup()
        {
            values = ValuesGenerator.Default.GenerateUniqueValues<T>(N);
        }

        [Benchmark]
        public ImmutableHashSet<T> SysColImmutableHashSet()
        {
            var set = ImmutableHashSet.Create<T>();
            foreach (var value in values)
            {
                set = set.Add(value);
            }

            return set;
        }

        [Benchmark]
        public System.Collections.Generic.HashSet<T> SysColHashSet()
        {
            var set = new System.Collections.Generic.HashSet<T>();
            foreach (var value in values)
            {
                set.Add(value);
            }

            return set;
        }

        [Benchmark]
        public HashSet<T> LangExtHashSet()
        {
            var set = HashSet<T>();
            foreach (var value in values)
            {
                set = set.Add(value);
            }

            return set;
        }

        [Benchmark]
        public Set<T> LangExtSet()
        {
            var set = Set<T>();
            foreach (var value in values)
            {
                set = set.Add(value);
            }

            return set;
        }
    }
}
