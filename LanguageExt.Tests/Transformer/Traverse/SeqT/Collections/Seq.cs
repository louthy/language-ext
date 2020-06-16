using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class SeqSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Seq<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SeqSeqCrossProduct()
        {
            var ma = Seq(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Seq(
                Seq(1, 10),
                Seq(1, 20),
                Seq(1, 30),
                Seq(2, 10),
                Seq(2, 20),
                Seq(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Seq(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Seq<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Seq<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
