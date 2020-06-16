using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class TryOptionEitherAsync
    {
        [Fact]
        public async void FailIsRightFail()
        {
            var ma = TryOption<EitherAsync<string, int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryOption<int>>(TryOption<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void NoneIsRightNone()
        {
            var ma = TryOptional<EitherAsync<string, int>>(None);
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryOption<int>>(TryOptional<int>(None));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccLeftIsLeft()
        {
            var ma = TryOption(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, TryOption<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccRightIsRightSucc()
        {
            var ma = TryOption(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, TryOption<int>>(TryOption(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
