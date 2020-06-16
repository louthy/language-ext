using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class EitherUnsafeEitherAsync
    {
        [Fact]
        public async void LeftIsRightLeft()
        {
            var ma = LeftUnsafe<string, EitherAsync<string, int>>("alt");
            var mb = ma.Sequence();
            var mc = RightAsync<string, EitherUnsafe<string, int>>(LeftUnsafe<string, int>("alt"));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightLeftIsLeft()
        {
            var ma = RightUnsafe<string, EitherAsync<string, int>>(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, EitherUnsafe<string, int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightRightIsRightRight()
        {
            var ma = RightUnsafe<string, EitherAsync<string, int>>(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, EitherUnsafe<string, int>>(RightUnsafe<string, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }

    }
}
