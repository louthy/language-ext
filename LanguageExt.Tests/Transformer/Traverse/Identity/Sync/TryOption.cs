using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class TryOption
    {
        [Fact]
        public void IdentityFailisFailIdentity()
        {
            var ma = TryOptionFail<Identity<int>>(new System.Exception("An Error"));

            var mb = ma.Traverse(identity);

            var mc = Id(TryOptionFail<int>(new System.Exception("An Error")));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void IdentitySuccIsSuccIdentity()
        {
            var ma = TryOptionSucc(Id(42));

            var mb = ma.Traverse(identity);

            var mc = Id(TryOptionSucc(42));

            Assert.Equal(mc, mb);
        }
    }
}
