using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class EitherEitherAsync
    {
        [Fact]
        public async void LeftIsRightLeft()
        {
            var ma = Left<string, EitherAsync<string, int>>("alt");
            var mb = ma.Sequence();
            var mc = RightAsync<string, Either<string, int>>(Left<string, int>("alt"));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightLeftIsLeft()
        {
            var ma = Right<string, EitherAsync<string, int>>(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, Either<string, int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightRightIsRightRight()
        {
            var ma = Right<string, EitherAsync<string, int>>(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, Either<string, int>>(Right<string, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
