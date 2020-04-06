using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class Try
    {
        [Fact]
        public void TrySuccIdentityIsIdentityTrySucc()
        {
            var ma = TrySucc(Id(42));

            var mb = ma.Traverse(identity);

            var mc = Id(TrySucc(42));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void TryFailIdentityIsIdentityTryFail()
        {
            var ma = TryFail<Identity<int>>(new System.Exception("Failed"));

            var mb = ma.Traverse(identity);

            var mc = Id(TryFail<int>(new System.Exception("Failed")));

            Assert.Equal(mc, mb);
        }
    }
}
