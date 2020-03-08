using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class IEnumerableSeq
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            var ma = Enumerable.Empty<Seq<int>>();

            var mb = ma.Sequence();

            var mc = Seq<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableSeqCrossProduct()
        {
            var ma = new[] { Seq(1, 2), Seq(10, 20, 30) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = Seq<IEnumerable<int>>(
                Seq(1, 10),
                Seq(1, 20),
                Seq(1, 30),
                Seq(2, 10),
                Seq(2, 20),
                Seq(2, 30));

            Assert.True(mb.Map(Prelude.Seq) == mc.Map(Prelude.Seq));
        }

        [Fact]
        public void IEnumerableOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = new[] { Seq<int>(), Seq<int>(1, 2, 3) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = Seq<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableOfEmptiesIsEmpty()
        {
            var ma = new[] { Seq<int>(), Seq<int>() }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = Seq<IEnumerable<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
