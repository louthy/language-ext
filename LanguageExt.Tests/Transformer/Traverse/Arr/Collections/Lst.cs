using Xunit;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections;

public class LstArr
{
    [Fact]
    public void EmptyEmptyIsEmptyEmpty()
    {
        Lst<Arr<int>> ma = Empty;

        var mb = ma.KindT<LanguageExt.Lst, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, LanguageExt.Lst, Lst<int>, int>()
                   .As();

        var mc = Arr.singleton(Lst<int>.Empty);

        Assert.True(mb == mc);
    }

    [Fact]
    public void LstArrCrossProduct()
    {
        var ma = List(Array(1, 2), Array(10, 20, 30));

        var mb = ma.KindT<LanguageExt.Lst, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, LanguageExt.Lst, Lst<int>, int>()
                   .As();

        var mc = Array(
            List(1, 10),
            List(1, 20),
            List(1, 30),
            List(2, 10),
            List(2, 20),
            List(2, 30));

        var sb = mb.ToString();
        var sc = mc.ToString();
        
        Assert.True(mb == mc);
    }

    [Fact]
    public void LstOfEmptiesAndNonEmptiesIsEmpty()
    {
        var ma = List(Array<int>(), Array<int>(1, 2, 3));

        var mb = ma.KindT<LanguageExt.Lst, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, LanguageExt.Lst, Lst<int>, int>()
                   .As();

        var mc = Arr<Lst<int>>.Empty;

        Assert.True(mb == mc);
    }

    [Fact]
    public void LstOfEmptiesIsEmpty()
    {
        var ma = List(Array<int>(), Array<int>());

        var mb = ma.KindT<LanguageExt.Lst, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, LanguageExt.Lst, Lst<int>, int>()
                   .As();

        var mc = Arr<Lst<int>>.Empty;

        Assert.True(mb == mc);
    }
}
