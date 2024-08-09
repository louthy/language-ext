using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections;

public class IEnumerable
{
    [Fact]
    public void EmptyIEnumerableIsSuccessIEnumerable()
    {
        var ma = Iterable.empty<Validation<Error, int>>();
        var mb = ma.Traverse(x => x);
        Assert.Equal(Success<Error, Iterable<int>>(Iterable.empty<int>()), mb);
    }

    [Fact]
    public void IEnumerableSuccessIsSuccessIEnumerable()
    {
        var ma = IterableExtensions.AsIterable(List(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64)));
        var mb = ma.Traverse(x => x);
        Assert.Equal(Success<Error, Iterable<int>>(IterableExtensions.AsIterable(List(2, 8, 64))), mb);
    }

    [Fact]
    public void IEnumerableSuccAndFailIsFailedIEnumerable()
    {
        var ma = IterableExtensions.AsIterable(List(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12)));
        var mb = ma.Traverse(x => x);
        Assert.Equal(Fail<Error, Iterable<int>>(Error.New("failed")), mb);
    }
}
