using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Collections
{
    public class Arr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Arr<Stck<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            Stck<Arr<int>> mc = Empty;
            
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void ArrStckCrossProduct()
        {
            var ma = Array(Stack(1, 2), Stack(3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Array(2, 4), Array(2, 3), Array(1, 4), Array(1, 3));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void ArrayOfMixedEmptiesIsEmpty()
        {
            var ma = Array(Stck<int>.Empty, Stack(1, 2));
            var mb = ma.Traverse(identity);
            Stck<Arr<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void ArrayOfEmptiesIsEmpty()
        {
            var ma = Array(Stack<int>(), Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stck<Arr<int>>.Empty;

            Assert.Equal(mc, mb);
        }
    }
}
