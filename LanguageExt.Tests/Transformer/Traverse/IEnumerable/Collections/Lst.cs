using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class LstIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Lst<IEnumerable<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Lst<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void LstIEnumerableCrossProduct()
        {
            var ma = List<IEnumerable<int>>(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = new[]
                {
                    List(1, 10),
                    List(1, 20),
                    List(1, 30),
                    List(2, 10),
                    List(2, 20),
                    List(2, 30)
                }
                .AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void LstOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = List<IEnumerable<int>>(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Lst<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void LstOfEmptiesIsEmpty()
        {
            var ma = List<IEnumerable<int>>(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Lst<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
