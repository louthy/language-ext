using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class TryOptionTask
    {
        static Task<bool> Eq<A>(Task<TryOption<A>> ma, Task<TryOption<A>> mb) =>
            EqAsyncClass<Task<TryOption<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryOptionFail<Task<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TaskSucc(TryOption<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TryOptionSucc<Task<int>>(TaskFail<int>(new Exception("err")));
            var mb = ma.Sequence();
            var mc = TaskFail<TryOption<int>>(new Exception("err"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TryOptionSucc<Task<int>>(TaskSucc(1234));
            var mb = ma.Sequence();
            var mc = TaskSucc(TryOptionSucc(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
