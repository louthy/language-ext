using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class OptionUnsafeSeq
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = OptionUnsafe<Seq<int>>.None;
            var mb = ma.Sequence();
            var mc = Seq1(OptionUnsafe<int>.None);

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = SomeUnsafe<Seq<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Seq<OptionUnsafe<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeNonEmptySeqIsSeqSomes()
        {
            var ma = SomeUnsafe(Seq(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Seq(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            Assert.True(mb == mc);
        }
    }
}
