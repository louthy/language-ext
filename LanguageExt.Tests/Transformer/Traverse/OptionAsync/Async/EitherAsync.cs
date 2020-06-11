using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Async
{
    public class EitherAsyncOptionAsync
    {
        [Fact]
        public async void LeftIsSomeLeft()
        {
            var ma = LeftAsync<Error, OptionAsync<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeAsync(LeftAsync<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = RightAsync<Error, OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<EitherAsync<Error, int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSomeIsSomeRight()
        {
            var ma = RightAsync<Error, OptionAsync<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(RightAsync<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
