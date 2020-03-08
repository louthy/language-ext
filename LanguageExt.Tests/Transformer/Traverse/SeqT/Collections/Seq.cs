using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class SeqSeq
    {
        [Fact]
        public void EmptySeqIsEmptySeq()
        {
            Seq<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void SeqSeqIsSeqSeq()
        {
            var ma = Seq(Seq1(1), Seq1(2), Seq1(3));

            var mb = ma.Sequence();

            Assert.True(mb == Seq1(Seq(1, 2, 3)));
        }
    }
}
