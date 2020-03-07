using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Collections
{
    public class HashSetSeq
    {
        [Fact]
        public void EmptyHashSetIsEmptySeq()
        {
            HashSet<Seq<int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Empty);
        }

        [Fact]
        public void HashSetSeqIsSeqHashSet()
        {
            var ma = HashSet(Seq1(1), Seq1(2), Seq1(3));

            var mb = ma.Sequence();

            Assert.Equal(3, mb.Count);
            Assert.True(mb.Contains(HashSet(1)));
            Assert.True(mb.Contains(HashSet(2)));
            Assert.True(mb.Contains(HashSet(3)));
        }
    }
}
