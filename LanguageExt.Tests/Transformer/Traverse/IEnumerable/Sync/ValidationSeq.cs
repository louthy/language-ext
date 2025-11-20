using System.Collections.Generic;
using System.Linq;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync;

public class ValidationIEnumerable
{
    [Fact]
    public void FailIsSingletonFail()
    {
        var ma = Fail<Error, Iterable<int>>(Error.New("alt"));
        var mb = ma.Traverse(mx => mx).As();

        IEnumerable<Validation<Error, int>> mc = new[] { Fail<Error, int>(Error.New("alt")) };

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }

    [Fact]
    public void SuccessEmptyIsEmpty()
    {
        var ma = Success<Error, Iterable<int>>(Iterable.empty<int>());
        var mb = ma.Traverse(mx => mx).As();

        var mc = Enumerable.Empty<Validation<Error, int>>();

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }

    [Fact]
    public void SuccessNonEmptyIEnumerableIsIEnumerableSuccesses()
    {
        var ma = Success<Error, Iterable<int>>([1, 2, 3, 4]);
        var mb = ma.Traverse(mx => mx).As();

        var mc = new[]
                 {
                     Success<Error, int>(1),
                     Success<Error, int>(2),
                     Success<Error, int>(3),
                     Success<Error, int>(4)
                 }.AsEnumerable();

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }
}
