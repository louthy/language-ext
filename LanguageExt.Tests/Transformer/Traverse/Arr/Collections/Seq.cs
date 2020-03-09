using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class SeqArr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<Arr<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Arr<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SeqArrCrossProduct()
        {
            var ma = Seq(Array(1, 2), Array(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Array(
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
            var ma = Seq(Array<int>(), Array<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Arr<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq(Array<int>(), Array<int>());

            var mb = ma.Sequence();

            var mc = Arr<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
