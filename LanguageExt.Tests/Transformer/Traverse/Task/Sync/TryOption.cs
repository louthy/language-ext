using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class TryOptionTask
    {
        /*[Fact]
        public async void FailIsSomeFail()
        {
            var ma = TryOption<Task<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryOption<int>(new Exception("fail")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = TryOption<Task<int>>(None);
            var mb = ma.Sequence();
            var mc = Task<TryOption<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSomeIsSomeSucc()
        {
            var ma = TryOption<Task<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(TryOption(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }*/
    }
}
