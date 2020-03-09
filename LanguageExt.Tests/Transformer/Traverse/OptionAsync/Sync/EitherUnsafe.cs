using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class EitherUnsafeOptionAsync
    {
        [Fact]
        public async void LeftIsNone()
        {
            var ma = LeftUnsafe<Error, OptionAsync<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeAsync(LeftUnsafe<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);

        }
        
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = RightUnsafe<Error, OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<EitherUnsafe<Error, int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSomeIsSomeRight()
        {
            var ma = RightUnsafe<Error, OptionAsync<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(RightUnsafe<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
