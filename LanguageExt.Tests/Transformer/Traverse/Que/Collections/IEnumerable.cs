using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class IEnumerableQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            var ma = Enumerable.Empty<Que<int>>();

            var mb = ma.Traverse(identity);

            var mc = Que<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableQueCrossProduct()
        {
            var ma = new[] { Queue(1, 2), Queue(10, 20, 30) }.AsEnumerable();

            var mb = ma.Traverse(identity);

            var mc = Queue<IEnumerable<int>>(
                Seq(1, 10),
                Seq(1, 20),
                Seq(1, 30),
                Seq(2, 10),
                Seq(2, 20),
                Seq(2, 30));

            Assert.True(mb.ToSeq().Map(toArray) == mc.ToSeq().Map(toArray));
        }

        [Fact]
        public void IEnumerableOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = new[] { Queue<int>(), Queue<int>(1, 2, 3) }.AsEnumerable();

            var mb = ma.Traverse(identity);

            var mc = Que<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableOfEmptiesIsEmpty()
        {
            var ma = new[] { Queue<int>(), Queue<int>() }.AsEnumerable();

            var mb = ma.Traverse(identity);

            var mc = Que<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
