using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Sync
{
    public class IdentityOptionUnsafe
    {
        [Fact]
        public void IdentityNoneIsNone()
        {
            var ma = new Identity<OptionUnsafe<int>>(None);
            var mb = ma.Traverse(Prelude.identity);
            var mc = OptionUnsafe<Identity<int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void IdentitySomeIsSomeIdentity()
        {
            var ma = new Identity<OptionUnsafe<int>>(1234);
            var mb = ma.Traverse(Prelude.identity);
            var mc = SomeUnsafe(new Identity<int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
