using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    public class HashMapRandomReadBenchmark
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        ImmutableDictionary<int, int> immutableMap;
        Dictionary<int, int> dictionary;
        HashMap<EqInt, int, int> hashMap;

        [IterationSetup]
        public void Setup()
        {
            SysColImmutableDictionarySetup();
            SysColDictionarySetup();
            LangExtHashMapSetup();
        }

        [Benchmark]
        public void SysColImmutableDictionary()
        {
            int result = default;
            for (int j = 0; j < N; j++)
            {
                result ^= immutableMap[j];
            }
        }

        [Benchmark]
        public void SysColDictionary()
        {
            int result = default;
            for (int j = 0; j < N; j++)
            {
                result ^= dictionary[j];
            }
        }

        [Benchmark]
        public void LangExtHashMap()
        {
            int result = default;
            for (int j = 0; j < N; j++)
            {
                result ^= hashMap[j];
            }
        }

        public void SysColImmutableDictionarySetup()
        {
            immutableMap = ImmutableDictionary.Create<int, int>();
            for (int j = 0; j < N; j++)
            {
                immutableMap = immutableMap.Add(j, j);
            }
        }

        public void SysColDictionarySetup()
        {
            dictionary = new Dictionary<int, int>();
            for (int j = 0; j < N; j++)
            {
                dictionary.Add(j, j);
            }
        }

        public void LangExtHashMapSetup()
        {
            hashMap = HashMap<EqInt, int, int>();
            for (int j = 0; j < N; j++)
            {
                hashMap = hashMap.Add(j, j);
            }
        }
    }
}
