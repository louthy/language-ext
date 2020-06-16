using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Async
{
    public class TaskOptionAsync
    {
        [Fact]
        public async void TaskNoneIsNoneTask()
        {
            var ma = OptionAsync<int>.None.AsTask();
            var mb = ma.Sequence();
            var mc = OptionAsync<Task<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void TaskSomeIsSomeTask()
        {
            var ma = OptionAsync<int>.Some(123).AsTask();
            var mb = ma.Sequence();
            var mc = OptionAsync<Task<int>>.Some(123.AsTask());

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
