using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Async
{
    public class TryAsyncEitherAsync
    {
        [Fact]
        public async void FailIsRightFail()
        {
            var ma = TryAsyncFail<EitherAsync<string, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryAsync<int>>(TryAsyncFail<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccLeftIsLeft()
        {
            var ma = TryAsyncSucc(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, TryAsync<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccRightIsRightSucc()
        {
            var ma = TryAsync(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryAsync<int>>(TryAsync(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
