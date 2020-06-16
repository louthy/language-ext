using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class SetSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Seq<Set<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetSeqCrossProduct()
        {
            var ma = Set(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Seq(
                Set(1, 10),
                Set(1, 20),
                Set(1, 30),
                Set(2, 10),
                Set(2, 20),
                Set(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Set(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Seq<Set<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Seq<Set<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
