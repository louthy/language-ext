using System.Linq;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync;

public class EitherIEnumerable
{
    [Fact]
    public void LeftIsSingletonLeft()
    {
        var ma = Left<Error, EnumerableM<int>>(Error.New("alt"));
        var mb = ma.Traverse(mx => mx).As();

        var mc = new[] { Left<Error, int>(Error.New("alt")) }.AsEnumerable();

        Assert.True(mb.ToSeq() == mc.AsEnumerableM().ToSeq());
    }

    [Fact]
    public void RightEmptyIsEmpty()
    {
        var ma = Right<Error, EnumerableM<int>>(EnumerableM.empty<int>());
        var mb = ma.Traverse(mx => mx).As();

        var mc = Enumerable.Empty<Either<Error, int>>();

        Assert.True(mb.ToSeq() == mc.AsEnumerableM().ToSeq());
    }

    [Fact]
    public void RightNonEmptyIEnumerableIsIEnumerableRight()
    {
        var ma = Right<Error, EnumerableM<int>>([1, 2, 3, 4]);
        var mb = ma.Traverse(mx => mx).As();

        var mc = new[] { Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4) };

        Assert.True(mb.ToSeq() == mc.AsEnumerableM().ToSeq());
    }
}
