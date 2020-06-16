using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Collections
{
    public class Set
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<Stck<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            Stck<Set<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SetStckCrossProduct()
        {
            var ma = Set(Stack(1, 2), Stack(3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Set(2, 4), Set(2, 3), Set(1, 4), Set(1, 3));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SetOfMixedEmptiesIsEmpty()
        {
            var ma = Set(Stck<int>.Empty, Stack(1, 2));
            var mb = ma.Traverse(identity);
            Stck<Set<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set(Stack<int>(), Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stck<Set<int>>.Empty;

            Assert.Equal(mc, mb);
        }
    }
}
