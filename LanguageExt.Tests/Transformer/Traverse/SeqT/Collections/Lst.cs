using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class LstSeq
    {
        [Fact]
        public void EmptyLstIsEmptySeq()
        {
            Lst<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void LstSeqIsSeqLst()
        {
            var ma = List(Seq1(1), Seq1(2), Seq1(3));

            var mb = ma.Sequence();

            Assert.True(mb == Seq1(List(1, 2, 3)));
        }
    }
}
