using Xunit;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections;

public class ArrArr
{
    [Fact]
    public void EmptyEmptyIsEmptyEmpty()
    {
        Arr<Arr<int>> ma = Empty;

        var mb = ma.KindT<Arr, Arr, Arr<int>, int>()
                   .SequenceM()
                   .AsT<Arr, Arr, Arr<int>, int>()
                   .As();

        var mc = Arr.singleton(Arr<int>.Empty);

        Assert.True(mb == mc);
    }

    [Fact]
    public void ArrArrCrossProduct()
    {
        var ma = Arr(Arr(1, 2), Arr(10, 20, 30));

        var mb = ma.KindT<Arr, Arr, Arr<int>, int>()
                   .SequenceM()
                   .AsT<Arr, Arr, Arr<int>, int>()
                   .As();

        var mc = Arr(Arr(1, 10),
                     Arr(1, 20),
                     Arr(1, 30),
                     Arr(2, 10),
                     Arr(2, 20),
                     Arr(2, 30));

        Assert.True(mb == mc);
    }

    [Fact]
    public void ArrOfEmptiesAndNonEmptiesIsEmpty()
    {
        var ma = Arr(Arr<int>(), Arr(1, 2, 3));

        var mb = ma.KindT<Arr, Arr, Arr<int>, int>()
                   .SequenceM()
                   .AsT<Arr, Arr, Arr<int>, int>()
                   .As();

        var mc = Arr<Arr<int>>.Empty;

        Assert.True(mb == mc);
    }

    [Fact]
    public void ArrOfEmptiesIsEmpty()
    {
        var ma = Arr(Arr<int>(), Arr<int>());

        var mb = ma.KindT<Arr, Arr, Arr<int>, int>()
                   .SequenceM()
                   .AsT<Arr, Arr, Arr<int>, int>()
                   .As();

        var mc = Arr<Arr<int>>.Empty;

        Assert.True(mb == mc);
    }
}
