using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class ArrIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Arr<IEnumerable<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Arr<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void ArrIEnumerableCrossProduct()
        {
            var ma = Array<IEnumerable<int>>(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Sequence();

            var mc = new[]
                {
                    Array(1, 10),
                    Array(1, 20),
                    Array(1, 30),
                    Array(2, 10),
                    Array(2, 20),
                    Array(2, 30)
                }
                .AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void ArrOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Array<IEnumerable<int>>(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Arr<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void ArrOfEmptiesIsEmpty()
        {
            var ma = Array<IEnumerable<int>>(Seq<int>(), Seq<int>());

            var mb = ma.Sequence();

            var mc = Enumerable.Empty<Arr<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
