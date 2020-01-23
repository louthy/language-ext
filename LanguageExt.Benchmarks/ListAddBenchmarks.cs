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
    public class ListAddBenchmarks<T, TOrd>
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
        public ImmutableList<T> SysColImmutableList()
        {
            var collection = ImmutableList.Create<T>();
            foreach (var value in values)
            {
                collection = collection.Add(value);
            }

            return collection;
        }

        [Benchmark]
        public Lst<T> LangExtLst()
        {
            var collection = List<T>();
            foreach (var value in values)
            {
                collection = collection.Add(value);
            }

            return collection;
        }

        [Benchmark]
        public Seq<T> LangExtSeq()
        {
            var collection = Seq<T>();
            foreach (var value in values)
            {
                collection = collection.Add(value);
            }

            return collection;
        }
    }
}
