using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class IEnumerableSeq
    {
        [Fact]
        public void EmptyIEnumerableIsEmptySeq()
        {
            var ma = Enumerable.Empty<Seq<int>>();

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void IEnumerableSeqIsSeqIEnumerable()
        {
            var ma = Enumerable.Range(1, 3).Map(Seq1);

            var mb = ma.Sequence();

            Assert.True(mb == Seq<IEnumerable<int>>(Seq1(1), Seq1(2), Seq1(3)));
        }
    }
}
