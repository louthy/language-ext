using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Async
{
    public class TryAsyncTask
    {
        static Task<bool> Eq<A>(Task<TryAsync<A>> ma, Task<TryAsync<A>> mb) =>
            EqAsyncClass<Task<TryAsync<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsTaskFail()
        {
            var ma = TryAsyncFail<Task<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TaskSucc(TryAsyncFail<int>(new Exception("fail")));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeTaskIsTaskSome()
        {
            var ma = TryAsyncSucc<Task<int>>(TaskSucc(123));
            var mb = ma.Sequence();
            var mc = TaskSucc(TryAsyncSucc<int>(123));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
    }
}
