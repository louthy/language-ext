using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class HashSetIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<IEnumerable<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<HashSet<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void HashSetIEnumerableCrossProduct()
        {
            var ma = HashSet<IEnumerable<int>>(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = new[]
                {
                    HashSet(1, 10),
                    HashSet(1, 20),
                    HashSet(1, 30),
                    HashSet(2, 10),
                    HashSet(2, 20),
                    HashSet(2, 30)
                }
                .AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = HashSet<IEnumerable<int>>(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<HashSet<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet<IEnumerable<int>>(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<HashSet<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
