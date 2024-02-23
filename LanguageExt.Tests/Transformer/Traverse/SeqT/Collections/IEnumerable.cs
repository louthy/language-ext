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
            var ma = EnumerableM.empty<Seq<int>>();

            var mb = ma.Traverse(mx => mx).As();


            var mc = Seq<EnumerableM<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableSeqCrossProduct()
        {
            var ma = new[] { Seq(1, 2), Seq(10, 20, 30) }.AsEnumerableM();

            var mb = ma.Traverse(mx => mx).As();


            var mc = Seq<EnumerableM<int>>(
                [1, 10],
                [1, 20],
                [1, 30],
                [2, 10],
                [2, 20],
                [2, 30]);

            Assert.True(mb.Map(Seq.singleton) == mc.Map(Seq.singleton));
        }

        [Fact]
        public void IEnumerableOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = new[] { Seq<int>(), Seq<int>(1, 2, 3) }.AsEnumerableM();

            var mb = ma.Traverse(mx => mx).As();


            var mc = Seq<EnumerableM<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void IEnumerableOfEmptiesIsEmpty()
        {
            var ma = new[] { Seq<int>(), Seq<int>() }.AsEnumerableM();

            var mb = ma.Traverse(mx => mx).As();


            var mc = Seq<EnumerableM<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
