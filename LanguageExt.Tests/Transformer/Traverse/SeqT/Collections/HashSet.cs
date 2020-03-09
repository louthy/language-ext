using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class HashSetSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Seq<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetSeqCrossProduct()
        {
            var ma = HashSet(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Seq(
                HashSet(1, 10),
                HashSet(1, 20),
                HashSet(1, 30),
                HashSet(2, 10),
                HashSet(2, 20),
                HashSet(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = HashSet(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Seq<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Seq<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
