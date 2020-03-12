using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class TryEitherAsync
    {
        [Fact]
        public async void FailIsRightFail()
        {
            var ma = Try<EitherAsync<string, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = RightAsync<string, Try<int>>(Try<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccLeftIsLeft()
        {
            var ma = Try(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, Try<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccRightIsRightSucc()
        {
            var ma = Try(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, Try<int>>(Try(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
