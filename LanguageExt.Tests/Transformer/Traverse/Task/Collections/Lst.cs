using System;
using Xunit;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Collections
{
    public class LstTask
    {
        static Task<bool> Eq<A>(Task<Lst<A>> ma, Task<Lst<A>> mb) =>
            EqAsyncClass<Task<Lst<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = List<Task<int>>();
            var mb = ma.Sequence();
            var mc = TaskSucc(List<int>());
            
            Assert.True(await (Eq(mb, mc)));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = List(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.Sequence();
            var mc = TaskSucc(List(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = List(TaskSucc(1), TaskSucc(2), TaskFail<int>(new SystemException("fail")));
            var mb = ma.Sequence();
            var mc = TaskFail<Lst<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
