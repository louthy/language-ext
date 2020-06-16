using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class EitherOptionAsync
    {
        [Fact]
        public async void LeftIsSomeLeft()
        {
            var ma = Left<Error, OptionAsync<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeAsync(Left<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = Right<Error, OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<Either<Error, int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSomeIsSomeRight()
        {
            var ma = Right<Error, OptionAsync<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(Right<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
