using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class SetArr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<Arr<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Arr<Set<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetArrCrossProduct()
        {
            var ma = Set(Array(1, 2), Array(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Array(
                Set(1, 10),
                Set(1, 20),
                Set(1, 30),
                Set(2, 10),
                Set(2, 20),
                Set(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Set(Array<int>(), Array<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Arr<Set<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set(Array<int>(), Array<int>());

            var mb = ma.Sequence();

            var mc = Arr<Set<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
