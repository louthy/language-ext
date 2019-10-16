using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    [RPlotExporter, RankColumn]
    public class HashMapAddBenchmark
    {

        [Params(100, 1000, 10000, 100000)]
        public int N;

        [Benchmark]
        public void SysColImmutableDictionary()
        {
            var map = ImmutableDictionary.Create<int, int>();

            for (int j = 0; j < N; j++)
            {
                map = map.Add(j, j);
            }
        }

        [Benchmark]
        public void SysColDictionary()
        {
            var map = new Dictionary<int, int>();

            for (int j = 0; j < N; j++)
            {
                map.Add(j, j);
            }
        }

        [Benchmark]
        public void LangExtHashMap()
        {
            var map = HashMap<int, int>();

            for (int j = 0; j < N; j++)
            {
                map = map.Add(j, j);
            }
        }
    }
}
