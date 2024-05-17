using LanguageExt.Common;
using LanguageExt.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync;

public class EitherArr
{
    [Fact]
    public void LeftIsSingletonLeft()
    {
        var ma = Left<Error, Arr<int>>(Error.New("alt"));

        var mb = ma.KindT<Either<Error>, Arr, Arr<int>, int>()
                   .SequenceM()
                   .AsT<Arr, Either<Error>, Either<Error, int>, int>()
                   .As();
            
        var mc = Array(Left<Error, int>(Error.New("alt")));

        Assert.True(mb == mc);
    }

    [Fact]
    public void RightEmptyIsEmpty()
    {
        var ma = Right<Error, Arr<int>>(Empty);
        var mb = ma.KindT<Either<Error>, Arr, Arr<int>, int>()
                   .SequenceM()
                   .AsT<Arr, Either<Error>, Either<Error, int>, int>()
                   .As();
        var mc = Array<Either<Error, int>>();

        Assert.True(mb == mc);
    }

    [Fact]
    public void RightNonEmptyArrIsArrRight()
    {
        var ma = Right<Error, Arr<int>>(Array(1, 2, 3, 4));
        var mb = ma.KindT<Either<Error>, Arr, Arr<int>, int>()
                   .SequenceM()
                   .AsT<Arr, Either<Error>, Either<Error, int>, int>()
                   .As();
        var mc = Array(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4));

        Assert.True(mb == mc);
    }
}
