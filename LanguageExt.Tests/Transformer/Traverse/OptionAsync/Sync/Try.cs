using System;
using Xunit;
using LanguageExt.Common;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Sync
{
    public class TryOptionAsync
    {
        [Fact]
        public async void FailIsSomeFail()
        {
            var ma = Try<OptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeAsync(Try<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = Try<OptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionAsync<Try<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSomeIsSomeSucc()
        {
            var ma = TrySucc(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(TrySucc(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
