using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class SetSeq
    {
        [Fact]
        public void EmptySetIsEmptySeq()
        {
            Set<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void SetSeqIsSeqSet()
        {
            var ma = Set(Seq1(1), Seq1(2), Seq1(3));

            var mb = ma.Sequence();

            Assert.True(mb == Seq(Set(1), Set(2), Set(3)));
        }
    }
}
