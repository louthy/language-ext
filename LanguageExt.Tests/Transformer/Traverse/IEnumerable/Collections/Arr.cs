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
            Arr<EnumerableM<int>> ma = Empty;

            var mb = ma.Traverse(mx => mx).As();


            var mc = EnumerableM.empty<Arr<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void ArrIEnumerableCrossProduct()
        {
            var ma = Array<EnumerableM<int>>([1, 2], [10, 20, 30]);

            var mb = ma.Traverse(mx => mx).As();


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
            var ma = Array<EnumerableM<int>>([], [1, 2, 3]);

            var mb = ma.Traverse(mx => mx).As();

            var mc = EnumerableM.empty<Arr<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void ArrOfEmptiesIsEmpty()
        {
            var ma = Array<EnumerableM<int>>([], []);

            var mb = ma.Traverse(mx => mx).As();


            var mc = EnumerableM.empty<Arr<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
