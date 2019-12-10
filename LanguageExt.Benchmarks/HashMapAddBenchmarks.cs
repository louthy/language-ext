using System.Collections.Generic;
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
    public class HashMapAddBenchmarks<T, TOrd>
        where TOrd : struct, Ord<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        Dictionary<T, T> values;

        [GlobalSetup]
        public void Setup()
        {
            values = ValuesGenerator.Default.GenerateDictionary<T, T>(N);
        }

        [Benchmark]
        public ImmutableDictionary<T, T> SysColImmutableDictionary()
        {
            var map = ImmutableDictionary.Create<T, T>();
            foreach (var kvp in values)
            {
                map = map.Add(kvp.Key, kvp.Value);
            }

            return map;
        }

        [Benchmark]
        public ImmutableSortedDictionary<T, T> SysColImmutableSortedDictionary()
        {
            var map = ImmutableSortedDictionary.Create<T, T>();
            foreach (var kvp in values)
            {
                map = map.Add(kvp.Key, kvp.Value);
            }

            return map;
        }

        [Benchmark]
        public Dictionary<T, T> SysColDictionary()
        {
            var map = new Dictionary<T, T>();
            foreach (var kvp in values)
            {
                map.Add(kvp.Key, kvp.Value);
            }

            return map;
        }

        [Benchmark]
        public HashMap<TOrd, T, T> LangExtHashMap()
        {
            var map = HashMap<TOrd, T, T>();
            foreach (var kvp in values)
            {
                map = map.Add(kvp.Key, kvp.Value);
            }

            return map;
        }

        [Benchmark]
        public Map<TOrd, T, T> LangExtMap()
        {
            var map = Map<TOrd, T, T>();
            foreach (var kvp in values)
            {
                map = map.Add(kvp.Key, kvp.Value);
            }

            return map;
        }
    }
}
