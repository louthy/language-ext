using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Async
{
    public class TaskTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Task<A>> ma, TryOptionAsync<Task<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Task<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void TaskFailIsFailTask()
        {
            var ma = TryOptionAsync<int>(new Exception("fail")).AsTask();
            var mb = ma.Sequence();
            var mc = TryOptionAsync<Task<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void TaskSuccIsSuccTask()
        {
            var ma = TryOptionAsync<int>(123).AsTask();
            var mb = ma.Sequence();
            var mc = TryOptionAsync<Task<int>>(123.AsTask());

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
