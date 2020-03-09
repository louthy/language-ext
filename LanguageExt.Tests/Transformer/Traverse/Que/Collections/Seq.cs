using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class SeqQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<Que<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Que<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SeqQueCrossProduct()
        {
            var ma = Seq(Queue(1, 2), Queue(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Queue(
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
            var ma = Seq(Queue<int>(), Queue<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Que<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq(Queue<int>(), Queue<int>());

            var mb = ma.Traverse(identity);

            var mc = Que<Seq<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
