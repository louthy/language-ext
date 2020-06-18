using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Collections
{
    public class HashSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<Stck<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            Stck<HashSet<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void HashSetStckCrossProduct()
        {
            var ma = HashSet(Stack(1, 2), Stack(3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(HashSet(2, 4), HashSet(2, 3), HashSet(1, 4), HashSet(1, 3));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void HashSetOfMixedEmptiesIsEmpty()
        {
            var ma = HashSet(Stck<int>.Empty, Stack(1, 2));
            var mb = ma.Traverse(identity);
            Stck<HashSet<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet(Stack<int>(), Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stck<HashSet<int>>.Empty;

            Assert.Equal(mc, mb);
        }
    }
}
