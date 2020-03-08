using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class StckSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<Seq<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Seq<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void StckSeqCrossProduct()
        {
            var ma = Stack(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Seq(
                Stack(1, 10),
                Stack(1, 20),
                Stack(1, 30),
                Stack(2, 10),
                Stack(2, 20),
                Stack(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void StckOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Stack(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Seq<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack(Seq<int>(), Seq<int>());

            var mb = ma.Traverse(identity);

            var mc = Seq<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
