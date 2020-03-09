using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class StckQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<Que<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Que<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void StckQueCrossProduct()
        {
            var ma = Stack(Queue(1, 2), Queue(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Queue(
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
            var ma = Stack(Queue<int>(), Queue<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Que<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack(Queue<int>(), Queue<int>());

            var mb = ma.Traverse(identity);

            var mc = Que<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
