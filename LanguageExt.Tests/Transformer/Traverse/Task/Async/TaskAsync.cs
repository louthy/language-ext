using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Async
{
    public class TaskTask
    {
        static Task<bool> Eq<A>(Task<Task<A>> ma, Task<Task<A>> mb) =>
            EqAsyncClass<Task<Task<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TaskSucc(TaskSucc(123));
            var mb = ma.Sequence();
            var mc = TaskSucc(TaskSucc(123));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
 
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TaskSucc(TaskFail<int>(new SystemException("alt")));
            var mb = ma.Sequence();
            var mc = TaskFail<Task<int>>(new SystemException("alt"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }        
    }
}
