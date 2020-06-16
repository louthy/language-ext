using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class EitherTask
    {
        static Task<bool> Eq<L, R>(Task<Either<L, R>> ma, Task<Either<L, R>> mb) =>
            EqAsyncClass<Task<Either<L, R>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = Left<Error, Task<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TaskSucc(Left<Error, int>(Error.New("alt")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = Right<Error, Task<int>>(TaskFail<int>(new Exception("err")));
            var mb = ma.Sequence();
            var mc = TaskFail<Either<Error, int>>(new Exception("err"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = Right<Error, Task<int>>(TaskSucc(1234));
            var mb = ma.Sequence();
            var mc = TaskSucc(Right<Error, int>(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
