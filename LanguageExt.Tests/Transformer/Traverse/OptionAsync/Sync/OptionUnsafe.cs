using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class OptionUnsafeOptionAsync
    {
        [Fact]
        public async void NoneIsSomeNone()
        {
            var ma = OptionUnsafe<OptionAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = SomeAsync(OptionUnsafe<int>.None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = SomeUnsafe<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<OptionUnsafe<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSomeIsSomeSome()
        {
            var ma = SomeUnsafe(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(SomeUnsafe(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
