using Xunit;
using LanguageExt.Common;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections;

public class HashSetEither
{
    [Fact]
    public void EmptyHashSetIsRightEmptyHashSet()
    {
        HashSet<Either<Error, int>> ma = Empty;

        var mb = ma.Traverse(x => x).As();

        Assert.True(mb == Right(HashSet<int>.Empty));
    }
        
    [Fact]
    public void HashSetRightsIsRightHashSets()
    {
        var ma = HashSet(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3));

        var mb = ma.Traverse(x => x).As();

        Assert.True(mb == Right(HashSet(1, 2, 3)));
    }
        
    [Fact]
    public void HashSetRightAndLeftIsLeftEmpty()
    {
        var ma = HashSet(Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative")));
        
        var mb = ma.Traverse(x => x).As();
        
        Assert.True(mb == Left(Error.New("alternative")));
    }
}
