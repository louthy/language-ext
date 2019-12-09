using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int), typeof(OrdInt))]
    [GenericTypeArguments(typeof(string), typeof(OrdString))]
    public class HashMapRandomReadBenchmarks<T, TOrd>
        where TOrd : struct, Ord<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        T[] keys;

        ImmutableDictionary<T, T> immutableMap;
        ImmutableSortedDictionary<T, T> immutableSortedMap;
        Dictionary<T, T> dictionary;
        HashMap<TOrd, T, T> hashMap;
        Map<TOrd, T, T> map;

        [GlobalSetup]
        public void Setup()
        {
            var values = ValuesGenerator.Default.GenerateDictionary<T, T>(N);
            keys = values.Keys.ToArray();

            immutableMap = ValuesGenerator.SysColImmutableDictionarySetup(values);
            immutableSortedMap = ValuesGenerator.SysColImmutableSortedDictionarySetup(values);
            dictionary = ValuesGenerator.SysColDictionarySetup(values);
            hashMap = ValuesGenerator.LangExtHashMapSetup<T, TOrd>(values);
            map = ValuesGenerator.LangExtMapSetup<T, TOrd>(values);
        }

        [Benchmark]
        public int SysColImmutableDictionary()
        {
            int result = default;
            foreach (var key in keys)
            {
                result ^= immutableMap[key].GetHashCode();
            }

            return result;
        }

        [Benchmark]
        public int SysColImmutableSortedDictionary()
        {
            int result = default;
            foreach (var key in keys)
            {
                result ^= immutableSortedMap[key].GetHashCode();
            }

            return result;
        }

        [Benchmark]
        public int SysColDictionary()
        {
            int result = default;
            foreach (var key in keys)
            {
                result ^= dictionary[key].GetHashCode();
            }

            return result;
        }

        [Benchmark]
        public int LangExtHashMap()
        {
            int result = default;
            foreach (var key in keys)
            {
                result ^= hashMap[key].GetHashCode();
            }

            return result;
        }

        [Benchmark]
        public int LangExtMap()
        {
            int result = default;
            foreach (var key in keys)
            {
                result ^= map[key].GetHashCode();
            }

            return result;
        }
    }
}
