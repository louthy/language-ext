using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections
{
    public class QueIEnumerable
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<IEnumerable<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Enumerable.Empty<Que<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void QueIEnumerableCrossProduct()
        {
            var ma = Queue<IEnumerable<int>>(Seq(1, 2), Seq(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = new[]
                {
                    Queue(1, 10),
                    Queue(1, 20),
                    Queue(1, 30),
                    Queue(2, 10),
                    Queue(2, 20),
                    Queue(2, 30)
                }
                .AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void QueOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Queue<IEnumerable<int>>(Seq<int>(), Seq<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Enumerable.Empty<Que<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void QueOfEmptiesIsEmpty()
        {
            var ma = Queue<IEnumerable<int>>(Seq<int>(), Seq<int>());

            var mb = ma.Traverse(identity);

            var mc = Enumerable.Empty<Que<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
