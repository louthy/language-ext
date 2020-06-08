using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Collections
{
    public class IEnumerableTask
    {
        static Task<bool> Eq<A>(Task<IEnumerable<A>> ma, Task<IEnumerable<A>> mb) =>
            EqAsyncClass<Task<IEnumerable<A>>>.EqualsAsync(ma, mb);

        static IEnumerable<A> mkEnum<A>(params A[] xs)
        {
            foreach (var x in xs)
            {
                yield return x;
            }
        }

        [Fact]
        public async void EmptyIsSomeEmpty_Parallel()
        {
            var ma = mkEnum<Task<int>>();
            var mb = ma.SequenceParallel();
            var mc = TaskSucc(mkEnum<int>());
            
            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void EmptyIsSomeEmpty_Serial()
        {
            var ma = mkEnum<Task<int>>();
            var mb = ma.SequenceSerial();
            var mc = TaskSucc(mkEnum<int>());
            
            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Parallel()
        {
            var ma = mkEnum(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.SequenceParallel();
            var mc = TaskSucc(mkEnum(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Serial()
        {
            var ma = mkEnum(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.SequenceSerial();
            var mc = TaskSucc(mkEnum(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Parallel()
        {
            var ma = mkEnum(TaskSucc(1), TaskSucc(2), TaskFail<int>(new Exception("fail")));
            var mb = ma.SequenceParallel();
            var mc = TaskFail<IEnumerable<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
                
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Serial()
        {
            var ma = mkEnum(TaskSucc(1), TaskSucc(2), TaskFail<int>(new SystemException("fail")));
            var mb = ma.SequenceSerial();
            var mc = TaskFail<IEnumerable<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
