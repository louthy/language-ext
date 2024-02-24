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
        var ma = EnumerableM.empty<Either<Error, int>>();

        var mb = ma.Sequence()
                   .AsT<Either<Error>, EnumerableM, EnumerableM<int>, int>()
                   .As();

        var mr = mb.Map(b => ma.Count() == b.Count())
                   .IfLeft(false);
            
        Assert.True(mr);
    }

    [Fact]
    public void IEnumerableRightsIsRightIEnumerables()
    {
        var ma = new[] {Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3)}.AsEnumerableM();

        var mb = ma.Sequence()
                   .AsT<Either<Error>, EnumerableM, EnumerableM<int>, int>()
                   .As();

        Assert.True(mb.Map(b => EqEnumerable<int>.Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfLeft(false));
    }

    [Fact]
    public void IEnumerableRightAndLeftIsLeftEmpty()
    {
        var ma = new[] {Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative"))}.AsEnumerableM();

        var mb = ma.Sequence()
                   .AsT<Either<Error>, EnumerableM, EnumerableM<int>, int>()
                   .As();

        Assert.True(mb == Left(Error.New("alternative")));
    }
}
