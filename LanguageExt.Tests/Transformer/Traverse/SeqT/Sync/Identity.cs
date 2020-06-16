using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class IdentitySeq
    {
        [Fact]
        public void IdEmptyIsEmpty()
        {
            var ma = Id<Seq<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Seq<Identity<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void IdNonEmptySeqIsSeqId()
        {
            var ma = Id(Seq(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Seq(Id(1), Id(2), Id(3));

            Assert.True(mb == mc);
        }
    }
}
