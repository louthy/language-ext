using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class Seq
    {
        [Fact]
        public void EmptySeqIsEmpty()
        {
            var ma = Seq<Identity<int>>.Empty;

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Seq<int>.Empty), mb);
        }

        [Fact]
        public void SeqOfIdentitiesIsIdentityOfSeq()
        {
            var ma = Seq(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Seq(1, 3, 5)), mb);
        }
    }
}
