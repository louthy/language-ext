using Xunit;
using System.Linq;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections;

public class IEnumerableArr
{
    private static EnumerableM<T> enumerable<T>(params T[] items) => items.AsEnumerableM();

    [Fact]
    public void EmptyEmptyIsEmptyEmpty()
    {
        var ma = EnumerableM.empty<Arr<int>>();

        var mb = ma.KindT<EnumerableM, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, EnumerableM, EnumerableM<int>, int>()
                   .As();

        var mc = Arr<EnumerableM<int>>.Empty;

        Assert.True(mb == mc);
    }

    [Fact]
    public void IEnumerableArrCrossProduct()
    {
        var ma = new[] { Array(1, 2), Array(10, 20, 30) }.AsEnumerableM();

        var mb = ma.KindT<EnumerableM, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, EnumerableM, EnumerableM<int>, int>()
                   .As();

        var mc = Array(
            enumerable(1, 10),
            enumerable(1, 20),
            enumerable(1, 30),
            enumerable(2, 10),
            enumerable(2, 20),
            enumerable(2, 30));

        Assert.True(mb.Map(toArray) == mc.Map(toArray));
    }

    [Fact]
    public void IEnumerableOfEmptiesAndNonEmptiesIsEmpty()
    {
        var ma = enumerable(Array<int>(), Array(1, 2, 3));

        var mb = ma.KindT<EnumerableM, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, EnumerableM, EnumerableM<int>, int>()
                   .As();

        var mc = Arr<EnumerableM<int>>.Empty;

        Assert.True(mb == mc);
    }

    [Fact]
    public void IEnumerableOfEmptiesIsEmpty()
    {
        var ma = enumerable(Array<int>(), Array<int>());

        var mb = ma.KindT<EnumerableM, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, EnumerableM, EnumerableM<int>, int>()
                   .As();

        var mc = Arr<EnumerableM<int>>.Empty;

        Assert.True(mb == mc);
    }
}
