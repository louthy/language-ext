using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Async
{
    public class TryOptionAsyncEitherAsync
    {
        [Fact]
        public async void FailIsRightFail()
        {
            var ma = TryOptionAsyncFail<EitherAsync<string, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryOptionAsync<int>>(TryOptionAsyncFail<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void NoneIsRightNone()
        {
            var ma = TryOptionalAsync<EitherAsync<string, int>>(None);
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryOptionAsync<int>>(TryOptionalAsync<int>(None));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccLeftIsLeft()
        {
            var ma = TryOptionAsyncSucc<EitherAsync<string, int>>("alt");
            var mb = ma.Sequence();
            var mc = LeftAsync<string, TryOptionAsync<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSomeIsSomeSucc()
        {
            var ma = TryOptionAsync(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryOptionAsync<int>>(TryOptionAsync(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
