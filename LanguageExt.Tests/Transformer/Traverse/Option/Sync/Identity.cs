using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Sync
{
    public class IdentityOption
    {
        [Fact]
        public void IdentityNoneIsNone()
        {
            var ma = new Identity<Option<int>>(None);
            var mb = ma.Traverse(Prelude.identity);
            var mc = Option<Identity<int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void IdentitySomeIsSomeIdentity()
        {
            var ma = new Identity<Option<int>>(1234);
            var mb = ma.Traverse(Prelude.identity);
            var mc = Some(new Identity<int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
