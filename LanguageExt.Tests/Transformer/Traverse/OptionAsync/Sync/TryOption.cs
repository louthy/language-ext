using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class TryOptionOptionAsync
    {
        [Fact]
        public async void FailIsSomeFail()
        {
            var ma = TryOption<OptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryOption<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = TryOption<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<TryOption<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSomeIsSomeSucc()
        {
            var ma = TryOption<OptionAsync<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryOption(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
