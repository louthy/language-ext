using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class StckSeq
    {
        [Fact]
        public void EmptyStckIsEmptySeq()
        {
            Stck<Seq<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            Assert.True(mb == Empty);
        }

        [Fact]
        public void StckSeqIsSeqStck()
        {
            var ma = Stack(Seq1(1), Seq1(2), Seq1(3));

            var mb = ma.Traverse(identity);

            Assert.True(mb == Seq1(Stack(1, 2, 3)));
        }
    }
}
