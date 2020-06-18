using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Collections
{
    public class Seq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<Stck<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            Stck<Seq<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SeqStckCrossProduct()
        {
            var ma = Seq(Stack(1, 2), Stack(3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Seq(2, 4), Seq(2, 3), Seq(1, 4), Seq(1, 3));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SeqOfMixedEmptiesIsEmpty()
        {
            var ma = Queue(Stck<int>.Empty, Stack(1, 2));
            var mb = ma.Traverse(identity);
            Stck<Que<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq(Stack<int>(), Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stck<Seq<int>>.Empty;

            Assert.Equal(mc, mb);
        }
    }
}
