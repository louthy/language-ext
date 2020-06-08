using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class OptionUnsafeTask
    {
        static Task<bool> Eq<A>(Task<OptionUnsafe<A>> ma, Task<OptionUnsafe<A>> mb) =>
            EqAsyncClass<Task<OptionUnsafe<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = OptionUnsafe<Task<int>>.None;
            var mb = ma.Sequence();
            var mc = TaskSucc(OptionUnsafe<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = SomeUnsafe<Task<int>>(TaskFail<int>(new Exception("err")));
            var mb = ma.Sequence();
            var mc = TaskFail<OptionUnsafe<int>>(new Exception("err"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = SomeUnsafe(TaskSucc(1234));
            var mb = ma.Sequence();
            var mc = TaskSucc(SomeUnsafe(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
