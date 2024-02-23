using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync;

public class ValidationArr
{
    [Fact]
    public void FailIsSingletonFail()
    {
        var ma = Fail<Error, Arr<int>>(Error.New("alt"));
        var mb = ma.KindT<Validation<Error>, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, Validation<Error>, Validation<Error, int>, int>()
                   .As();
        var mc = Array(Fail<Error, int>(Error.New("alt")));

        Assert.True(mb == mc);
    }

    [Fact]
    public void SuccessEmptyIsEmpty()
    {
        var ma = Success<Error, Arr<int>>(Empty);
        var mb = ma.KindT<Validation<Error>, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, Validation<Error>, Validation<Error, int>, int>()
                   .As();
        var mc = Array<Validation<Error, int>>();

        Assert.True(mb == mc);
    }

    [Fact]
    public void SuccessNonEmptyArrIsArrSuccesses()
    {
        var ma = Success<Error, Arr<int>>(Array(1, 2, 3, 4));
        var mb = ma.KindT<Validation<Error>, Arr, Arr<int>, int>()
                   .Sequence()
                   .AsT<Arr, Validation<Error>, Validation<Error, int>, int>()
                   .As();
        var mc = Array(Success<Error, int>(1), Success<Error, int>(2), Success<Error, int>(3), Success<Error, int>(4));

        Assert.True(mb == mc);
    }
}
