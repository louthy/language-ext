using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class EitherUnsafeSeq
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = LeftUnsafe<Error, Seq<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Seq1(LeftUnsafe<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, Seq<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Seq<EitherUnsafe<Error, int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightNonEmptySeqIsSeqRight()
        {
            var ma = RightUnsafe<Error, Seq<int>>(Seq(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = Seq(
                RightUnsafe<Error, int>(1),
                RightUnsafe<Error, int>(2),
                RightUnsafe<Error, int>(3),
                RightUnsafe<Error, int>(4));

            Assert.True(mb == mc);
        }
    }
}
