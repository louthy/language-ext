using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class ArrSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Arr<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Seq<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrSeqCrossProduct()
        {
            var ma = Array(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Seq(
                Array(1, 10),
                Array(1, 20),
                Array(1, 30),
                Array(2, 10),
                Array(2, 20),
                Array(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Array(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Seq<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrOfEmptiesIsEmpty()
        {
            var ma = Array(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Seq<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
