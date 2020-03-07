using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync
{
    public class IdentitySeq
    {
        [Fact]
        public void IdentityEmptyIsEmpty()
        {
            var ma = new Identity<Seq<int>>(Empty);

            var mb = ma.Traverse(identity);

            Assert.True(mb == Empty);
        }

        [Fact]
        public void IdentitySeqIsSeqIdentity()
        {
            var ma = new Identity<Seq<int>>(Seq(1, 2, 3));

            var mb = ma.Traverse(identity);

            Assert.True(mb == Seq(new Identity<int>(1), new Identity<int>(2), new Identity<int>(3)));
        }
    }
}
