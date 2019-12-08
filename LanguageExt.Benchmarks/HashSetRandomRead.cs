using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int), typeof(EqInt))]
    [GenericTypeArguments(typeof(string), typeof(EqString))]
    public class HashSetRandomReadBenchmark<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        T[] values;

        ImmutableHashSet<T> immutableSet;
        ImmutableSortedSet<T> immutableSortedSet;
        System.Collections.Generic.HashSet<T> sysHashSet;
        HashSet<T> hashSet;
        Set<T> set;

        [GlobalSetup]
        public void Setup()
        {
            values = ValuesGenerator.Default.GenerateUniqueValues<T>(N);

            immutableSet = SysColImmutableHashSetSetup(values);
            immutableSortedSet = SysColImmutableSortedSetSetup(values);
            sysHashSet = SysColHashSetSetup(values);
            hashSet = LangExtHashSetSetup(values);
            set = LangExtSetSetup(values);
        }

        [Benchmark]
        public bool SysColImmutableHashSet()
        {
            var result = true;
            foreach (var value in values)
            {
                result &= immutableSet.Contains(value);
            }

            return result;
        }

        [Benchmark]
        public bool SysColImmutableSortedSet()
        {
            var result = true;
            foreach (var value in values)
            {
                result &= immutableSortedSet.Contains(value);
            }

            return result;
        }

        [Benchmark]
        public bool SysColHashSet()
        {
            var result = true;
            foreach (var value in values)
            {
                result &= sysHashSet.Contains(value);
            }

            return result;
        }

        [Benchmark]
        public bool LangExtHashSet()
        {
            var result = true;
            foreach (var value in values)
            {
                result &= hashSet.Contains(value);
            }

            return result;
        }

        [Benchmark]
        public bool LangExtSet()
        {
            var result = true;
            foreach (var value in values)
            {
                result &= set.Contains(value);
            }

            return result;
        }

        public ImmutableHashSet<T> SysColImmutableHashSetSetup(T[] values)
        {
            var immutableSet = ImmutableHashSet.Create<T>();
            foreach (var value in values)
            {
                immutableSet = immutableSet.Add(value);
            }

            return immutableSet;
        }

        public ImmutableSortedSet<T> SysColImmutableSortedSetSetup(T[] values)
        {
            var immutableSet = ImmutableSortedSet.Create<T>();
            foreach (var value in values)
            {
                immutableSet = immutableSet.Add(value);
            }

            return immutableSet;
        }

        public System.Collections.Generic.HashSet<T> SysColHashSetSetup(T[] values)
        {
            var hashSet = new System.Collections.Generic.HashSet<T>();
            foreach (var value in values)
            {
                hashSet.Add(value);
            }

            return hashSet;
        }

        public HashSet<T> LangExtHashSetSetup(T[] values)
        {
            var hashSet = HashSet<T>();
            foreach (var value in values)
            {
                hashSet = hashSet.Add(value);
            }

            return hashSet;
        }

        private Set<T> LangExtSetSetup(T[] values)
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
