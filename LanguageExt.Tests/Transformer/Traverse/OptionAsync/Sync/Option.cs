using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class OptionOptionAsync
    {
        [Fact]
        public async void NoneIsSomeNone()
        {
            var ma = Option<OptionAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = SomeAsync(Option<int>.None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = Some<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<Option<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSomeIsSomeSome()
        {
            var ma = Some(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(Some(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
