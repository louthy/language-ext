using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class ArrArr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Arr<Arr<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Arr<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrArrCrossProduct()
        {
            var ma = Array(Array(1, 2), Array(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Array(
                Array(1, 10),
                Array(1, 20),
                Array(1, 30),
                Array(2, 10),
                Array(2, 20),
                Array(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Array(Array<int>(), Array<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Arr<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrOfEmptiesIsEmpty()
        {
            var ma = Array(Array<int>(), Array<int>());

            var mb = ma.Sequence();

            var mc = Arr<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
