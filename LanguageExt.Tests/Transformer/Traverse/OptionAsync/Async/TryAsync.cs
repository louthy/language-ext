using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Async
{
    public class TryAsyncOptionAsync
    {
        [Fact]
        public async void FailIsSomeFail()
        {
            var ma = TryAsyncFail<OptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryAsyncFail<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = TryAsyncSucc<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<TryAsync<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSomeIsSomeSucc()
        {
            var ma = TryAsync(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryAsync(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
