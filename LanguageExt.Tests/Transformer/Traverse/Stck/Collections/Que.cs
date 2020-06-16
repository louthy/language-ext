using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Collections
{
    public class Que
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<Stck<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            Stck<Que<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void QueueStckCrossProduct()
        {
            var ma = Queue(Stack(1, 2), Stack(3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Queue(2, 4), Queue(2, 3), Queue(1, 4), Queue(1, 3));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void QueueOfMixedEmptiesIsEmpty()
        {
            var ma = Queue(Stck<int>.Empty, Stack(1, 2));
            var mb = ma.Traverse(identity);
            Stck<Que<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void QueueOfEmptiesIsEmpty()
        {
            var ma = Queue(Stack<int>(), Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stck<Que<int>>.Empty;

            Assert.Equal(mc, mb);
        }
    }
}
