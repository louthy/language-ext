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
    public class ListIterationBenchmarks<T, TOrd>
        where TOrd : struct, Ord<T>
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        T[] values;
        ImmutableList<T> immutableList;
        Lst<T> lst;
        Seq<T> seq;

        [GlobalSetup]
        public void Setup()
        {
            values = ValuesGenerator.Default.GenerateUniqueValues<T>(N);
            immutableList = ImmutableList.CreateRange(ValuesGenerator.Default.GenerateUniqueValues<T>(N));
            lst = ValuesGenerator.Default.GenerateUniqueValues<T>(N).Freeze();
            seq = toSeq(ValuesGenerator.Default.GenerateUniqueValues<T>(N)).Strict();
        }

        [Benchmark]
        public T SysColImmutableList()
        {
            T result = default;

            var collection = immutableList;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }

        [Benchmark]
        public T LangExtLst()
        {
            T result = default;

            var collection = lst;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }

        [Benchmark]
        public T LangExtSeq()
        {
            T result = default;

            var collection = seq;
            foreach (var item in collection)
            {
                result = item;
            }

            return result;
        }
    }
}
