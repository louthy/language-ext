using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class IEnumerableArr
    {
        private static IEnumerable<T> enumerable<T>(params T[] items) => items.AsEnumerable();

        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            var ma = Enumerable.Empty<Arr<int>>();

            var mb = ma.Sequence();

            var mc = Arr<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableArrCrossProduct()
        {
            var ma = new[] { Array(1, 2), Array(10, 20, 30) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = Array(
                enumerable(1, 10),
                enumerable(1, 20),
                enumerable(1, 30),
                enumerable(2, 10),
                enumerable(2, 20),
                enumerable(2, 30));

            Assert.True(mb.Map(toArray) == mc.Map(toArray));
        }

        [Fact]
        public void IEnumerableOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = enumerable(Array<int>(), Array<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Arr<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableOfEmptiesIsEmpty()
        {
            var ma = enumerable(Array<int>(), Array<int>());

            var mb = ma.Sequence();

            var mc = Arr<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
