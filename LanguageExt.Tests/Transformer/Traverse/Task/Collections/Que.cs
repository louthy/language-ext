using System;
using Xunit;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Collections
{
    public class QueueTask
    {
        static Task<bool> Eq<A>(Task<Que<A>> ma, Task<Que<A>> mb) =>
            EqAsyncClass<Task<Que<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = Queue<Task<int>>();
            var mb = ma.Traverse(identity);
            var mc = TaskSucc(Queue<int>());
            
            Assert.True(await (Eq(mb, mc)));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = Queue(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.Traverse(identity);
            var mc = TaskSucc(Queue(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = Queue(TaskSucc(1), TaskSucc(2), TaskFail<int>(new SystemException("fail")));
            var mb = ma.Traverse(identity);
            var mc = TaskFail<Que<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
