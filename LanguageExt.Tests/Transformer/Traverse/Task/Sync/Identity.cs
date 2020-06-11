using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class IdentityTask
    {
        static Task<bool> Eq<A>(Task<Identity<A>> ma, Task<Identity<A>> mb) =>
            EqAsyncClass<Task<Identity<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void IdentityFailIsFail()
        {
            var ma = Id<Task<int>>(TaskFail<int>(new Exception("err")));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TaskFail<Identity<int>>(new Exception("err"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void IdentitySuccIsSuccIdentity()
        {
            var ma = Id<Task<int>>(TaskSucc(1234));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TaskSucc(new Identity<int>(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
