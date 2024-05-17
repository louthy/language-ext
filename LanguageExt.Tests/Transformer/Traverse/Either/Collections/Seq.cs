using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections;

public class SeqEither
{
    [Fact]
    public void EmptySeqIsRightEmptySeq()
    {
        Seq<Either<Error, int>> ma = Empty;

        var mb = ma.Traverse(x => x).As();

        Assert.True(mb == Right(Seq<int>.Empty));
    }
        
    [Fact]
    public void SeqRightsIsRightSeqs()
    {
        var ma = Seq(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3));

        var mb = ma.Traverse(x => x).As();

        Assert.True(mb == Right(Seq(1, 2, 3)));
    }
        
    [Fact]
    public void SeqRightAndLeftIsLeftEmpty()
    {
        var ma = Seq(Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative")));

        var mb = ma.KindT<Seq, Either<Error>, Either<Error, int>, int>()
                   .SequenceM()
                   .AsT<Either<Error>, Seq, Seq<int>, int>()
                   .As();

        Assert.True(mb == Left(Error.New("alternative")));
    }
}
