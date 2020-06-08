using System;
using Xunit;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Collections
{
    public class ArrTask
    {
        static Task<bool> Eq<A>(Task<Arr<A>> ma, Task<Arr<A>> mb) =>
            EqAsyncClass<Task<Arr<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = Array<Task<int>>();
            var mb = ma.Sequence();
            var mc = TaskSucc(Array<int>());
            
            Assert.True(await (Eq(mb, mc)));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = Array(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.Sequence();
            var mc = TaskSucc(Array(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = Array(TaskSucc(1), TaskSucc(2), TaskFail<int>(new SystemException("fail")));
            var mb = ma.Sequence();
            var mc = TaskFail<Arr<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
