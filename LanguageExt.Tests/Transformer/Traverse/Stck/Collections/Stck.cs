using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Collections
{
    public class Stck
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<Stck<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            Stck<Stck<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void StckStckCrossProduct()
        {
            var ma = Stack(Stack(1, 2), Stack(3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(
                Stack(1, 3),
                Stack(1, 4),
                Stack(2, 3),
                Stack(2, 4));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void StckOfMixedEmptiesIsEmpty()
        {
            var ma = Stack(Stck<int>.Empty, Stack(1, 2));
            var mb = ma.Traverse(identity);
            Stck<Stck<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack(Stack<int>(), Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stck<Stck<int>>.Empty;

            Assert.Equal(mc, mb);
        }
    }
}
