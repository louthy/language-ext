using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections;

public class IEnumerable
{
    [Fact]
    public void EmptyIEnumerableIsSuccessIEnumerable()
    {
        var ma = EnumerableM.empty<Validation<Error, int>>();
        var mb = ma.Traverse(x => x);
        Assert.Equal(Success<Error, EnumerableM<int>>(EnumerableM.empty<int>()), mb);
    }

    [Fact]
    public void IEnumerableSuccessIsSuccessIEnumerable()
    {
        var ma = List(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64))
           .AsEnumerableM();
        var mb = ma.Traverse(x => x);
        Assert.Equal(Success<Error, EnumerableM<int>>(List(2, 8, 64).AsEnumerableM()), mb);
    }

    [Fact]
    public void IEnumerableSuccAndFailIsFailedIEnumerable()
    {
        var ma = List(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12)).AsEnumerableM();
        var mb = ma.Traverse(x => x);
        Assert.Equal(Fail<Error, EnumerableM<int>>(Error.New("failed")), mb);
    }
}
