using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class ArrSeq
    {
        [Fact]
        public void EmptyArrIsEmptySeq()
        {
            Arr<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void ArrSeqIsSeqArr()
        {
            var ma = Array(Seq1(1), Seq1(2), Seq1(3));

            var mb = ma.Sequence();

            var mc = Seq<Arr<int>>(Array(1, 2, 3));

            Assert.True(mb == mc);
        }
    }
}
