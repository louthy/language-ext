using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class SetIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<IEnumerable<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Set<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SetIEnumerableCrossProduct()
        {
            var ma = Set<IEnumerable<int>>(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = new[]
                {
                    Set(1, 10),
                    Set(1, 20),
                    Set(1, 30),
                    Set(2, 10),
                    Set(2, 20),
                    Set(2, 30)
                }
                .AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Set<IEnumerable<int>>(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Set<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set<IEnumerable<int>>(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Set<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
