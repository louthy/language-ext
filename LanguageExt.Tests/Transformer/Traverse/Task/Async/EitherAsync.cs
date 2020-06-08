using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Async
{
    public class EitherAsyncTask
    {
        static Task<bool> Eq<A>(Task<EitherAsync<Error, A>> ma, Task<EitherAsync<Error, A>> mb) =>
            EqAsyncClass<Task<EitherAsync<Error, A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void LeftIsTaskLeft()
        {
            var ma = LeftAsync<Error, Task<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TaskSucc(LeftAsync<Error, int>(Error.New("alt")));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
                
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = RightAsync<Error, Task<int>>(TaskFail<int>(new SystemException("fail")));
            var mb = ma.Sequence();
            var mc = TaskFail<EitherAsync<Error, int>>(new SystemException("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightTaskIsTaskRight()
        {
            var ma = RightAsync<Error, Task<int>>(TaskSucc(123));
            var mb = ma.Sequence();
            var mc = TaskSucc(RightAsync<Error, int>(123));

            var mr = await (Eq(mb, mc));
            
            Assert.True(mr);
        }
    }
}
