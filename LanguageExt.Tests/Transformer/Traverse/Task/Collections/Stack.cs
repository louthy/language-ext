using System;
using Xunit;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Collections
{
    public class StackTask
    {
        static Task<bool> Eq<A>(Task<Stck<A>> ma, Task<Stck<A>> mb) =>
            EqAsyncClass<Task<Stck<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = Stack<Task<int>>();
            var mb = ma.Traverse(identity);
            var mc = TaskSucc(Stack<int>());

            Assert.True(await (Eq(mb, mc)));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = Stack(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.Traverse(identity);
            var mc = TaskSucc(Stack(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = Stack(TaskSucc(1), TaskSucc(2), TaskFail<int>(new SystemException("fail")));
            var mb = ma.Traverse(identity);
            var mc = TaskFail<Stck<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
