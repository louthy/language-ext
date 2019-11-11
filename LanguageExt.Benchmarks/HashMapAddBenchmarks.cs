using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int))]
    [GenericTypeArguments(typeof(string))]
    public class HashMapAddBenchmark<T>
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
        public HashMap<T, T> LangExtHashMap()
        {
            var map = HashMap<T, T>();
            foreach (var kvp in values)
            {
                map = map.Add(kvp.Key, kvp.Value);
            }

            return map;
        }
    }
}
