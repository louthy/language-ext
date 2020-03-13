using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class TryTask
    {
        /*[Fact]
        public async void FailIsSomeFail()
        {
            var ma = Try<Task<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeAsync(Try<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = Try<Task<int>>(None);
            var mb = ma.Sequence();
            var mc = Task<Try<int>>.None;

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
        }*/
    }
}
