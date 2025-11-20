using System.Linq;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections;

public class IEnumerableEither
{
    [Fact]
    public void EmptyIEnumerableIsRightEmptyIEnumerable()
    {
        var ma = Iterable.empty<Either<Error, int>>();

        var mb = ma.Traverse(x => x).As();

        var mr = mb.Map(b => ma.Count() == b.Count())
                   .IfLeft(false);
            
        Assert.True(mr);
    }

    [Fact]
    public void IEnumerableRightsIsRightIEnumerables()
    {
        var ma = new[] {Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3)}.AsIterable();

        var mb = ma.Traverse(x => x).As();

        Assert.True(mb.Map(b => EqEnumerable<int>.Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfLeft(false));
    }

    [Fact]
    public void IEnumerableRightAndLeftIsLeftEmpty()
    {
        var ma = new[] {Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative"))}.AsIterable();

        var mb = ma.Traverse(x => x).As();

        Assert.True(mb == Left(Error.New("alternative")));
    }
}
