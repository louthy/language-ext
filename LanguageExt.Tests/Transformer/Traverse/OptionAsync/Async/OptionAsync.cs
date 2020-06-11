using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Async
{
    public class OptionAsyncOptionAsync
    {
        [Fact]
        public async void NoneIsSomeNone()
        {
            var ma = OptionAsync<OptionAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = SomeAsync(OptionAsync<int>.None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = SomeAsync<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<OptionAsync<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSomeIsSomeSome()
        {
            var ma = SomeAsync(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(SomeAsync(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
