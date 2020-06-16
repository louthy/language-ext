using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class HashSetArr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<Arr<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Arr<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetArrCrossProduct()
        {
            var ma = HashSet(Array(1, 2), Array(10, 20, 30));

            var mb = ma.Sequence();
            mb = mb.OrderBy(x => x.ToArray()[1])
                .OrderBy(x => x.ToArray()[0])
                .ToArr();

            var mc = Array(
                HashSet(1, 10),
                HashSet(1, 20),
                HashSet(1, 30),
                HashSet(2, 10),
                HashSet(2, 20),
                HashSet(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = HashSet(Array<int>(), Array<int>(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Arr<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet(Array<int>(), Array<int>());
            var mb = ma.Sequence();
            var mc = Arr<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
