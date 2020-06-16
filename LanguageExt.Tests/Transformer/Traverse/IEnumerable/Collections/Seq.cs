using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class SeqIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<IEnumerable<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Seq<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SeqIEnumerableCrossProduct()
        {
            var ma = Seq<IEnumerable<int>>(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = new[]
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
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Seq<IEnumerable<int>>(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Seq<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq<IEnumerable<int>>(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Seq<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
