using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Sync
{
    public class Identity
    {
        [Fact]
        public void IdentityIdentityIsIdentityIdentity()
        {
            var ma = Id(Id(42));
            
            var mb = ma.Traverse(identity);

            var mc = Id(Id(42));

            Assert.Equal(mc, mb);
        }
    }
}
