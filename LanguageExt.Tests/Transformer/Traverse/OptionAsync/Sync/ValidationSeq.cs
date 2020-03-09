using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class ValidationSeqOptionAsync
    {
        [Fact]
        public async void FailIsSomeFail()
        {
            var ma = Fail<Error, OptionAsync<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeAsync(Fail<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccessNoneIsNone()
        {
            var ma = Success<Error, OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<Validation<Error, int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccessSomeIsSomeSuccess()
        {
            var ma = Success<Error, OptionAsync<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(Success<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
