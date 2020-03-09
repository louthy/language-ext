using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class QueArr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<Arr<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Arr<Que<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void QueArrCrossProduct()
        {
            var ma = Queue(Array(1, 2), Array(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Array(
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
            var ma = Queue(Array<int>(), Array<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Arr<Que<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void QueOfEmptiesIsEmpty()
        {
            var ma = Queue(Array<int>(), Array<int>());

            var mb = ma.Traverse(identity);

            var mc = Arr<Que<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
