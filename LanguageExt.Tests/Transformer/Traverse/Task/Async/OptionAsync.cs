using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Async
{
    public class OptionAsyncTask
    {
        static Task<bool> Eq<A>(Task<OptionAsync<A>> ma, Task<OptionAsync<A>> mb) =>
            EqAsyncClass<Task<OptionAsync<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void NoneIsTaskNone()
        {
            var ma = OptionAsync<Task<int>>.None;
            var mb = ma.Sequence();
            var mc = TaskSucc(OptionAsync<int>.None);

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
                        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = SomeValueAsync(TaskFail<int>(new SystemException("fail")));
            var mb = ma.Sequence();
            var mc = TaskFail<OptionAsync<int>>(new SystemException("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }

        [Fact]
        public async void SomeTaskIsTaskSome()
        {
            var ma = SomeAsync<Task<int>>(TaskSucc(123));
            var mb = ma.Sequence();
            var mc = TaskSucc(SomeAsync<int>(123));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
    }
}
