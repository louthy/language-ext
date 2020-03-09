using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class QueQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<Que<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Que<Que<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void QueQueCrossProduct()
        {
            var ma = Queue(Queue(1, 2), Queue(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Queue(
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
            var ma = Queue(Queue<int>(), Queue<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Que<Que<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void QueOfEmptiesIsEmpty()
        {
            var ma = Queue(Queue<int>(), Queue<int>());

            var mb = ma.Traverse(identity);

            var mc = Que<Que<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
