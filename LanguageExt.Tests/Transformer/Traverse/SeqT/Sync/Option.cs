using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class OptionSeq
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = Option<Seq<int>>.None;
            var mb = ma.Sequence();
            var mc = Seq1(Option<int>.None);

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = Some<Seq<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Seq<Option<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeNonEmptySeqIsSeqSomes()
        {
            var ma = Some(Seq(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Seq(Some(1), Some(2), Some(3));

            Assert.True(mb == mc);
        }
    }
}
