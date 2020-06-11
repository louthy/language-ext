using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Collections
{
    public class HashSetSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<Set<int>> ma = Empty;
            var mb = ma.Sequence();
            var mc = Set<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetSetCrossProduct()
        {
            var ma = HashSet(Set(1, 2), Set(10, 20, 30));
            var mb = ma.Sequence();
            var mc = Set(HashSet(1, 10), HashSet(1, 20), HashSet(1, 30), HashSet(2, 10), HashSet(2, 20), HashSet(2, 30));
        
            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = HashSet(Set<int>(), Set(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Set<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet(Set<int>(), Set<int>());
            var mb = ma.Sequence();
            var mc = Set<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
