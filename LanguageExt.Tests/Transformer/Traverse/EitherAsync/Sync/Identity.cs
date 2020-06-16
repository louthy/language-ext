using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class IdentityEitherAsync
    {
        [Fact]
        public async void IdentityLeftIsLeft()
        {
            var ma = Id(LeftAsync<string, int>("alt"));
            var mb = ma.Traverse(Prelude.identity);
            var mc = LeftAsync<string, Identity<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void IdentityRightIsRightIdentity()
        {
            var ma = Id<EitherAsync<string, int>>(1234);
            var mb = ma.Traverse(Prelude.identity);
            var mc = RightAsync<string, Identity<int>>(new Identity<int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
