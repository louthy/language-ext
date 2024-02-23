using LanguageExt.Common;
using LanguageExt.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections;

public class SetEither
{
    [Fact]
    public void EmptySetIsRightEmptySet()
    {
        Set<Either<Error, int>> ma = Empty;

        var mb = ma.KindT<Set, Either<Error>, Either<Error, int>, int>()
                   .Sequence()
                   .AsT<Either<Error>, Set, Set<int>, int>()
                   .As();

        Assert.True(mb == Right(Set<int>.Empty));
    }
        
    [Fact]
    public void SetRightsIsRightSets()
    {
        var ma = Set(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3));

        var mb = ma.KindT<Set, Either<Error>, Either<Error, int>, int>()
                   .Sequence()
                   .AsT<Either<Error>, Set, Set<int>, int>()
                   .As();

        Assert.True(mb == Right(Set(1, 2, 3)));
    }
        
    [Fact]
    public void SetRightAndLeftIsLeftEmpty()
    {
        var ma = Set(Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative")));

        var mb = ma.KindT<Set, Either<Error>, Either<Error, int>, int>()
                   .Sequence()
                   .AsT<Either<Error>, Set, Set<int>, int>()
                   .As();

        Assert.True(mb == Left(Error.New("alternative")));
    }
}
