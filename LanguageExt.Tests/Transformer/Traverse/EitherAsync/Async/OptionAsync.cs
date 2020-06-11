using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Async
{
    public class OptionAsyncEitherAsync
    {
        [Fact]
        public async void NoneIsRightNone()
        {
            var ma = OptionAsync<EitherAsync<Error, int>>.None;
            var mb = ma.Sequence();
            var mc = RightAsync<Error, OptionAsync<int>>(OptionAsync<int>.None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeLeftIsLeft()
        {
            var ma = SomeAsync<EitherAsync<Error, int>>(LeftAsync<Error, int>(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = LeftAsync<Error, OptionAsync<int>>(Error.New("alt"));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeRightIsRightSome()
        {
            var ma = SomeAsync(RightAsync<Error, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<Error, OptionAsync<int>>(SomeAsync(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
