using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class EitherUnsafeTask
    {
        static Task<bool> Eq<L, R>(Task<EitherUnsafe<L, R>> ma, Task<EitherUnsafe<L, R>> mb) =>
            EqAsyncClass<Task<EitherUnsafe<L, R>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = LeftUnsafe<Error, Task<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TaskSucc(LeftUnsafe<Error, int>(Error.New("alt")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = RightUnsafe<Error, Task<int>>(TaskFail<int>(new Exception("err")));
            var mb = ma.Sequence();
            var mc = TaskFail<EitherUnsafe<Error, int>>(new Exception("err"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = RightUnsafe<Error, Task<int>>(TaskSucc(1234));
            var mb = ma.Sequence();
            var mc = TaskSucc(RightUnsafe<Error, int>(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
