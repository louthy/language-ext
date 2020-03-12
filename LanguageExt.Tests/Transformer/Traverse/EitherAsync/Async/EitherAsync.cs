using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Async
{
    public class EitherAsyncEitherAsync
    {
        [Fact]
        public async void LeftIsRightLeft()
        {
            var ma = LeftAsync<Error, EitherAsync<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = RightAsync<Error, EitherAsync<Error, int>>(LeftAsync<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightLeftIsLeft()
        {
            var ma = RightAsync<Error, EitherAsync<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<Error, EitherAsync<Error, int>>(Error.New("alt"));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightRightIsRightRight()
        {
            var ma = RightAsync<Error, EitherAsync<Error, int>>(RightAsync<Error, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<Error, EitherAsync<Error, int>>(RightAsync<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
