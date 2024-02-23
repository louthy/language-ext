using LanguageExt.Common;
using LanguageExt.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections;

public class LstEither
{
    [Fact]
    public void EmptyLstIsRightEmptyLst()
    {
        Lst<Either<Error, int>> ma = Empty;

        var mb = ma.KindT<LanguageExt.Lst, Either<Error>, Either<Error, int>, int>()
                   .Sequence()
                   .AsT<Either<Error>, LanguageExt.Lst, Lst<int>, int>()
                   .As();

        Assert.True(mb == Right(Lst<int>.Empty));
    }
        
    [Fact]
    public void LstRightsIsRightLsts()
    {
        var ma = List(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3));

        var mb = ma.KindT<LanguageExt.Lst, Either<Error>, Either<Error, int>, int>()
                   .Sequence()
                   .AsT<Either<Error>, LanguageExt.Lst, Lst<int>, int>()
                   .As();

        Assert.True(mb == Right(List(1, 2, 3)));
    }
        
    [Fact]
    public void LstRightAndLeftIsLeftEmpty()
    {
        var ma = List(Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative")));

        var mb = ma.KindT<LanguageExt.Lst, Either<Error>, Either<Error, int>, int>()
                   .Sequence()
                   .AsT<Either<Error>, LanguageExt.Lst, Lst<int>, int>()
                   .As();

        Assert.True(mb == Left(Error.New("alternative")));
    }
}
