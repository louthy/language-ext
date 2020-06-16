using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class StckIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<IEnumerable<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Enumerable.Empty<Stck<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void StckIEnumerableCrossProduct()
        {
            var ma = Stack<IEnumerable<int>>(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = new[]
                {
                    Stack(1, 10),
                    Stack(1, 20),
                    Stack(1, 30),
                    Stack(2, 10),
                    Stack(2, 20),
                    Stack(2, 30)
                }
                .AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void StckOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Stack<IEnumerable<int>>(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Enumerable.Empty<Stck<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack<IEnumerable<int>>(Seq<int>(), Seq<int>());

            var mb = ma.Traverse(identity);

            var mc = Enumerable.Empty<Stck<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
