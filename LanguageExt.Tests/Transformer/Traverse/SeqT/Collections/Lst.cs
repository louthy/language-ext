using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class LstSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Lst<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Seq<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstSeqCrossProduct()
        {
            var ma = List(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Seq(
                List(1, 10),
                List(1, 20),
                List(1, 30),
                List(2, 10),
                List(2, 20),
                List(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = List(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Seq<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstOfEmptiesIsEmpty()
        {
            var ma = List(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Seq<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
