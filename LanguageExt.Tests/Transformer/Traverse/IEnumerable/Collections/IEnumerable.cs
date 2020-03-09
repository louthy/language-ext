using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class IEnumerableIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            var ma = Enumerable.Empty<IEnumerable<int>>();

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<IEnumerable<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void IEnumerableIEnumerableCrossProduct()
        {
            var ma = new IEnumerable<int>[] { Seq(1, 2), Seq(10, 20, 30) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = new IEnumerable<int>[]
                {
                    Seq(1, 10),
                    Seq(1, 20),
                    Seq(1, 30),
                    Seq(2, 10),
                    Seq(2, 20),
                    Seq(2, 30)
                }
                .AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void IEnumerableOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = new IEnumerable<int>[] { Seq<int>(), Seq<int>(1, 2, 3) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<IEnumerable<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void IEnumerableOfEmptiesIsEmpty()
        {
            var ma = new IEnumerable<int>[] { Seq<int>(), Seq<int>() };

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<IEnumerable<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
