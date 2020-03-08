using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class QueSeq
    {
        [Fact]
        public void EmptyQueIsEmptySeq()
        {
            Que<Seq<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            Assert.True(mb == Empty);
        }

        [Fact]
        public void QueSeqIsSeqQue()
        {
            var ma = Queue(Seq1(1), Seq1(2), Seq1(3));

            var mb = ma.Traverse(identity);

            Assert.True(mb == Seq1(Queue(1, 2, 3)));
        }
    }
}
