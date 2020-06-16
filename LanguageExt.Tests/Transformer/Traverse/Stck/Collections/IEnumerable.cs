using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Collections
{
    public class IEnumerable
    {
        static IEnumerable<T> Enumerate<T>(params T[] ts) => ts.AsEnumerable();

        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            var ma = Enumerable.Empty<Stck<int>>();
            var mb = ma.Traverse(identity);
            Stck<IEnumerable<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void HashSetStckCrossProduct()
        {
            var ma = Enumerate(Stack(1, 2), Stack(3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Enumerate(2, 4), Enumerate(2, 3), Enumerate(1, 4), Enumerate(1, 3));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void HashSetOfMixedEmptiesIsEmpty()
        {
            var ma = Enumerate(Stck<int>.Empty, Stack(1, 2));
            var mb = ma.Traverse(identity);
            Stck<IEnumerable<int>> mc = Empty;

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = Enumerate(Stack<int>(), Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stck<IEnumerable<int>>.Empty;

            Assert.Equal(mc, mb);
        }
    }
}
