using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class StckArr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<Arr<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Arr<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void StckArrCrossProduct()
        {
            var ma = Stack(Array(1, 2), Array(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Array(
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
            var ma = Stack(Array<int>(), Array<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Arr<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack(Array<int>(), Array<int>());

            var mb = ma.Traverse(identity);

            var mc = Arr<Stck<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
