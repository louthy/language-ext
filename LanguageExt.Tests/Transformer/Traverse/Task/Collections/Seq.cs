using Xunit;
using System;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Collections
{
    public class SeqTask
    {
        static Task<bool> Eq<A>(Task<Seq<A>> ma, Task<Seq<A>> mb) =>
            EqAsyncClass<Task<Seq<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSomeEmpty_Parallel()
        {
            var ma = Seq<Task<int>>();
            var mb = ma.SequenceParallel();
            var mc = TaskSucc(Seq<int>());
            
            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void EmptyIsSomeEmpty_Serial()
        {
            var ma = Seq<Task<int>>();
            var mb = ma.SequenceSerial();
            var mc = TaskSucc(Seq<int>());
            
            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Parallel()
        {
            var ma = Seq(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.SequenceParallel();
            var mc = TaskSucc(Seq(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Serial()
        {
            var ma = Seq(TaskSucc(1), TaskSucc(2), TaskSucc(3));
            var mb = ma.SequenceSerial();
            var mc = TaskSucc(Seq(1, 2, 3));

            Assert.True(await (Eq(mb, mc)));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Parallel()
        {
            var ma = Seq(TaskSucc(1), TaskSucc(2), TaskFail<int>(new Exception("fail")));
            var mb = ma.SequenceParallel();
            var mc = TaskFail<Seq<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
                
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Serial()
        {
            var ma = Seq(TaskSucc(1), TaskSucc(2), TaskFail<int>(new SystemException("fail")));
            var mb = ma.SequenceSerial();
            var mc = TaskFail<Seq<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
