using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int), typeof(OrdInt))]
    [GenericTypeArguments(typeof(string), typeof(OrdString))]
    public class HashSetRandomRemovalBenchmarks<T, TOrd>
        where TOrd : struct, Ord<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        T[] values;

        ImmutableHashSet<T> immutableSet;
        ImmutableSortedSet<T> immutableSortedSet;
        System.Collections.Generic.HashSet<T> sysHashSet;
        HashSet<TOrd, T> hashSet;
        Set<TOrd, T> set;

        [GlobalSetup]
        public void Setup()
        {
            values = ValuesGenerator.Default.GenerateUniqueValues<T>(N);

            immutableSet = ValuesGenerator.SysColImmutableHashSetSetup(values);
            immutableSortedSet = ValuesGenerator.SysColImmutableSortedSetSetup(values);
            sysHashSet = ValuesGenerator.SysColHashSetSetup(values);
            hashSet = ValuesGenerator.LangExtHashSetSetup<T, TOrd>(values);
            set = ValuesGenerator.LangExtSetSetup<T, TOrd>(values);
        }

        [Benchmark]
        public bool SysColImmutableHashSet()
        {
            var localImmutableSet = immutableSet;
            foreach (var value in values)
            {
                localImmutableSet = localImmutableSet.Remove(value);
            }

            return localImmutableSet.IsEmpty;
        }

        [Benchmark]
        public bool SysColImmutableSortedSet()
        {
            var localImmutableSortedSet = immutableSortedSet;
            foreach (var value in values)
            {
                localImmutableSortedSet = localImmutableSortedSet.Remove(value);
            }

            return localImmutableSortedSet.IsEmpty;
        }

        [Benchmark]
        public bool SysColHashSet()
        {
            // NB! no local variable - mutating field instance
            foreach (var value in values)
            {
                sysHashSet.Remove(value);
            }

            return sysHashSet.Count == 0;
        }

        [Benchmark]
        public bool LangExtHashSet()
        {
            var localHashSet = hashSet;
            foreach (var value in values)
            {
                localHashSet = localHashSet.Remove(value);
            }

            return localHashSet.IsEmpty;
        }

        [Benchmark]
        public bool LangExtSet()
        {
            var localSet = set;
            foreach (var value in values)
            {
                localSet = localSet.Remove(value);
            }

            return localSet.IsEmpty;
        }
    }
}
