using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Async
{
    public class TaskTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Task<A>> ma, TryAsync<Task<A>> mb) =>
            EqAsyncClass<TryAsync<Task<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void TaskFailIsFailTask()
        {
            var ma = TryAsync<int>(new Exception("fail")).AsTask();
            var mb = ma.Sequence();
            var mc = TryAsync<Task<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void TaskSuccIsSuccTask()
        {
            var ma = TryAsync<int>(123).AsTask();
            var mb = ma.Sequence();
            var mc = TryAsync<Task<int>>(123.AsTask());

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
