using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Async
{
    public class TryOptionAsyncOptionAsync
    {
        [Fact]
        public async void FailIsSomeFail()
        {
            var ma = TryOptionAsyncFail<OptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryOptionAsyncFail<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void NoneIsSomeNone()
        {
            var ma = TryOptionalAsync<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = SomeAsync(TryOptionalAsync<int>(None));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = TryOptionAsyncSucc<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<TryOptionAsync<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSomeIsSomeSucc()
        {
            var ma = TryOptionAsync(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryOptionAsync(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
