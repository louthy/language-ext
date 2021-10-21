using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class TrySeq
    {
        [Fact]
        public void FailIsSingletonNone()
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
            var mc = Seq<Try<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptySeqIsSeqSuccs()
        {
            var ma = TrySucc(Seq(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Seq(TrySucc(1), TrySucc(2), TrySucc(3));

            Assert.True(mb == mc);
        }
    }
}
