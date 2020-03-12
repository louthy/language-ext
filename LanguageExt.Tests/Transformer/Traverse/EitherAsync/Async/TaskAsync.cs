using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Async
{
    public class TaskEitherAsync
    {
        [Fact]
        public async void TaskLeftIsLeftTask()
        {
            var ma = LeftAsync<string, int>("alt").AsTask();
            var mb = ma.Sequence();
            var mc = LeftAsync<string, Task<int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void TaskRightIsRightTask()
        {
            var ma = RightAsync<string, int>(123).AsTask();
            var mb = ma.Sequence();
            var mc = RightAsync<string, Task<int>>(123.AsTask());

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
    }
}
