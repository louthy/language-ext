using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int), typeof(OrdInt))]
    [GenericTypeArguments(typeof(string), typeof(OrdString))]
    public class HashSetIterationBenchmarks<T, TOrd>
        where TOrd : struct, Ord<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        T[] values;

        ImmutableHashSet<T> immutableHashSet;
        ImmutableSortedSet<T> immutableSortedSet;
        System.Collections.Generic.HashSet<T> sysHashSet;
        HashSet<TOrd, T> hashSet;
        Set<TOrd, T> set;

        [GlobalSetup]
        public void Setup()
        {
            values = ValuesGenerator.Default.GenerateUniqueValues<T>(N);

            immutableHashSet = ValuesGenerator.SysColImmutableHashSetSetup(values);
            immutableSortedSet = ValuesGenerator.SysColImmutableSortedSetSetup(values);
            sysHashSet = ValuesGenerator.SysColHashSetSetup(values);
            hashSet = ValuesGenerator.LangExtHashSetSetup<T, TOrd>(values);
            set = ValuesGenerator.LangExtSetSetup<T, TOrd>(values);
        }

        [Benchmark]
        public T SysColImmutableHashSet()
        {
            T result = default;

            var collection = immutableHashSet;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }

        [Benchmark]
        public T SysColImmutableSortedSet()
        {
            T result = default;

            var collection = immutableSortedSet;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }

        [Benchmark]
        public T SysColHashSet()
        {
            T result = default;

            var collection = sysHashSet;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }

        [Benchmark]
        public T LangExtHashSet()
        {
            T result = default;

            var collection = hashSet;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }

        [Benchmark]
        public T LangExtSet()
        {
            T result = default;

            var collection = set;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }
    }
}
