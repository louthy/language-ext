using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class QueSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<Seq<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Seq<Que<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void QueSeqCrossProduct()
        {
            var ma = Queue(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Seq(
                Queue(1, 10),
                Queue(1, 20),
                Queue(1, 30),
                Queue(2, 10),
                Queue(2, 20),
                Queue(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void QueOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Queue(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Seq<Que<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void QueOfEmptiesIsEmpty()
        {
            var ma = Queue(Seq<int>(), Seq<int>());

            var mb = ma.Traverse(identity);

            var mc = Seq<Que<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
