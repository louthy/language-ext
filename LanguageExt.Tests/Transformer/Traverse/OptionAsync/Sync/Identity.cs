using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class IdentityOptionAsync
    {
        [Fact]
        public async void IdentityNoneIsNone()
        {
            var ma = Id<OptionAsync<int>>(None);
            var mb = ma.Traverse(Prelude.identity);
            var mc = OptionAsync<Identity<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void IdentitySomeIsSomeIdentity()
        {
            var ma = Id<OptionAsync<int>>(1234);
            var mb = ma.Traverse(Prelude.identity);
            var mc = SomeAsync(new Identity<int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
