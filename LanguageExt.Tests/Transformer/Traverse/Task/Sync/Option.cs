using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class OptionTask
    {
        static Task<bool> Eq<A>(Task<Option<A>> ma, Task<Option<A>> mb) =>
            EqAsyncClass<Task<Option<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = Option<Task<int>>.None;
            var mb = ma.Sequence();
            var mc = TaskSucc(Option<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = Some<Task<int>>(TaskFail<int>(new Exception("err")));
            var mb = ma.Sequence();
            var mc = TaskFail<Option<int>>(new Exception("err"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = Some(TaskSucc(1234));
            var mb = ma.Sequence();
            var mc = TaskSucc(Some(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
