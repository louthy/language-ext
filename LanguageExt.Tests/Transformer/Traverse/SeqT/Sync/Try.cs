using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class TrySeq
    {
        [Fact]
        public void FailIsSingletonFail()
        {
            var ma = TryFail<Seq<int>>(new Exception("fail"));

            var mb = ma.Sequence();

            var mc = Seq1(TryFail<int>(new Exception("fail")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc<Seq<int>>(Empty);

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void SuccSeqIsSeqSucc()
        {
            var ma = TrySucc(Seq(1, 2, 3));

            var mb = ma.Sequence();

            Assert.True(mb == Seq(TrySucc(1), TrySucc(2), TrySucc(3)));
        }
    }
}
