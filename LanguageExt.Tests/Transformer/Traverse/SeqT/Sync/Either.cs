using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class EitherSeq
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = Left<Error, Seq<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Seq1(Left<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Right<Error, Seq<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Seq<Either<Error, int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightNonEmptySeqIsSeqRight()
        {
            var ma = Right<Error, Seq<int>>(Seq(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = Seq(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4));

            Assert.True(mb == mc);
        }
    }
}
