using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int), typeof(OrdInt))]
    [GenericTypeArguments(typeof(string), typeof(OrdString))]
    public class HashSetAddBenchmarks<T, TOrd>
        where TOrd : struct, Ord<T>
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
        public ImmutableSortedSet<T> SysColImmutableSortedSet()
        {
            var set = ImmutableSortedSet.Create<T>();
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
        public HashSet<TOrd, T> LangExtHashSet()
        {
            var set = HashSet<TOrd, T>();
            foreach (var value in values)
            {
                set = set.Add(value);
            }

            return set;
        }

        [Benchmark]
        public Set<TOrd, T> LangExtSet()
        {
            var set = Set<TOrd, T>();
            foreach (var value in values)
            {
                set = set.Add(value);
            }

            return set;
        }
    }
}
