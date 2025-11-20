using System.Linq;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync;

public class EitherIEnumerable
{
    [Fact]
    public void LeftIsSingletonLeft()
    {
        var ma = Left<Error, Iterable<int>>(Error.New("alt"));
        var mb = ma.Traverse(mx => mx).As();

        var mc = new[] { Left<Error, int>(Error.New("alt")) }.AsEnumerable();

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }

    [Fact]
    public void RightEmptyIsEmpty()
    {
        var ma = Right<Error, Iterable<int>>(Iterable.empty<int>());
        var mb = ma.Traverse(mx => mx).As();

        var mc = Enumerable.Empty<Either<Error, int>>();

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }

    [Fact]
    public void RightNonEmptyIEnumerableIsIEnumerableRight()
    {
        var ma = Right<Error, Iterable<int>>([1, 2, 3, 4]);
        var mb = ma.Traverse(mx => mx).As();

        var mc = new[] { Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4) };

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }
}
