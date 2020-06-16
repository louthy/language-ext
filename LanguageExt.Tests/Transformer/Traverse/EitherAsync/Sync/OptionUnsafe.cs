using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class OptionUnsafeEitherAsync
    {
        [Fact]
        public async void NoneIsRightNone()
        {
            var ma = OptionUnsafe<EitherAsync<string, int>>.None;
            var mb = ma.Sequence();
            var mc = RightAsync<string, OptionUnsafe<int>>(None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeLeftIsLeft()
        {
            var ma = SomeUnsafe(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, OptionUnsafe<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeRightIsRightSome()
        {
            var ma = SomeUnsafe(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, OptionUnsafe<int>>(SomeUnsafe(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
