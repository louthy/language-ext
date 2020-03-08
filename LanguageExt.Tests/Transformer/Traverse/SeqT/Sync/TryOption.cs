using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class TryOptionSeq
    {
        [Fact]
        public void FailIsSingletonFail()
        {
            var ma = TryOptionFail<Seq<int>>(new Exception("fail"));

            var mb = ma.Sequence();
            
            var mc = Seq1(TryOptionFail<int>(new Exception("fail")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = TryOption<Seq<int>>(None);

            var mb = ma.Sequence();

            var mc = Seq1(TryOptional<int>(None));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc<Seq<int>>(Empty);

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void SuccSeqIsSeqSucc()
        {
            var ma = TryOptionSucc(Seq(1, 2, 3));

            var mb = ma.Sequence();

            Assert.True(mb == Seq(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3)));
        }
    }
}
