using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class OptionEitherAsync
    {
        [Fact]
        public async void NoneIsRightNone()
        {
            var ma = Option<EitherAsync<string, int>>.None;
            var mb = ma.Sequence();
            var mc = RightAsync<string, Option<int>>(None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeLeftIsLeft()
        {
            var ma = Some(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, Option<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeRightIsRightSome()
        {
            var ma = Some(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, Option<int>>(Some(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
