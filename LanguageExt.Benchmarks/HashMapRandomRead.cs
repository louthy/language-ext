using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Attributes;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    [GenericTypeArguments(typeof(int), typeof(EqInt))]
    [GenericTypeArguments(typeof(string), typeof(EqString))]
    public class HashMapRandomReadBenchmark<T, TEq>
        where TEq : struct, Eq<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        T[] keys;

        ImmutableDictionary<T, T> immutableMap;
        Dictionary<T, T> dictionary;
        HashMap<TEq, T, T> hashMap;

        [GlobalSetup]
        public void Setup()
        {
            var values = ValuesGenerator.Default.GenerateDictionary<T, T>(N);
            keys = values.Keys.ToArray();

            immutableMap = SysColImmutableDictionarySetup(values);
            dictionary = SysColDictionarySetup(values);
            hashMap = LangExtHashMapSetup(values);
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

        public ImmutableDictionary<T, T> SysColImmutableDictionarySetup(Dictionary<T, T> values)
        {
            var immutableMap = ImmutableDictionary.Create<T, T>();
            foreach (var kvp in values)
            {
                immutableMap = immutableMap.Add(kvp.Key, kvp.Value);
            }

            return immutableMap;
        }

        public Dictionary<T, T> SysColDictionarySetup(Dictionary<T, T> values)
        {
            var dictionary = new Dictionary<T, T>();
            foreach (var kvp in values)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            return dictionary;
        }

        public HashMap<TEq, T, T> LangExtHashMapSetup(Dictionary<T, T> values)
        {
            var hashMap = HashMap<TEq, T, T>();
            foreach (var kvp in values)
            {
                hashMap = hashMap.Add(kvp.Key, kvp.Value);
            }

            return hashMap;
        }
    }
}
