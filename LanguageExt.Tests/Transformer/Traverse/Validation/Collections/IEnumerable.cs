using LanguageExt.Common;
using L = LanguageExt;
using static LanguageExt.Prelude;
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
        var ma = L.Prelude.Lst(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
        var mb = ma.Traverse(x => x);
        Assert.Equal(Success<Error, Lst<int>>(L.Prelude.Lst(2, 8, 64)), mb);
    }

    [Fact]
    public void IEnumerableSuccAndFailIsFailedIEnumerable()
    {
        var ma = L.Prelude.Lst(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
        var mb = ma.Traverse(x => x);
        Assert.Equal(Fail<Error, Lst<int>>(Error.New("failed")), mb);
    }
}
