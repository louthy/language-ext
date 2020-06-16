using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class TryTask
    {
        static Task<bool> Eq<A>(Task<Try<A>> ma, Task<Try<A>> mb) =>
            EqAsyncClass<Task<Try<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = Try<Task<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TaskSucc(Try<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TrySucc<Task<int>>(TaskFail<int>(new Exception("err")));
            var mb = ma.Sequence();
            var mc = TaskFail<Try<int>>(new Exception("err"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TrySucc(TaskSucc(1234));
            var mb = ma.Sequence();
            var mc = TaskSucc(TrySucc(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
