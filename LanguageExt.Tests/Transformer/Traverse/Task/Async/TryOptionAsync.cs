using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Async
{
    public class TryOptionAsyncTask
    {
        static Task<bool> Eq<A>(Task<TryOptionAsync<A>> ma, Task<TryOptionAsync<A>> mb) =>
            EqAsyncClass<Task<TryOptionAsync<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsTaskFail()
        {
            var ma = TryOptionAsyncFail<Task<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TaskSucc(TryOptionAsyncFail<int>(new Exception("fail")));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
        
                
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = TryOptionAsyncSucc(TaskFail<int>(new SystemException("fail")));
            var mb = ma.Sequence();
            var mc = TaskFail<TryOptionAsync<int>>(new SystemException("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeTaskIsTaskSome()
        {
            var ma = TryOptionAsyncSucc<Task<int>>(TaskSucc(123));
            var mb = ma.Sequence();
            var mc = TaskSucc(TryOptionAsyncSucc<int>(123));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
    }
}
