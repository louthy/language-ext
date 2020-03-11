using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Collections
{
    public class SeqTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Seq<A>> ma, TryOptionAsync<Seq<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Seq<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty_Parallel()
        {
            var ma = Seq<TryOptionAsync<int>>();

            var mb = ma.SequenceParallel();

            var mc = TryOptionAsync(Seq<int>());

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
        [Fact]
        public async void EmptyIsSuccEmpty_Serial()
        {
            var ma = Seq<TryOptionAsync<int>>();

            var mb = ma.SequenceSerial();

            var mc = TryOptionAsync(Seq<int>());

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Parallel()
        {
            var ma = Seq(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.SequenceParallel();

            var mc = TryOptionAsync(Seq(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }

        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Serial()
        {
            var ma = Seq(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.SequenceSerial();

            var mc = TryOptionAsync(Seq(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail_Parallel()
        {
            var ma = Seq(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.SequenceParallel();

            var mc = TryOptionAsync<Seq<int>>(new Exception("fail"));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
                
        [Fact]
        public async void CollectionOfSuccsAndFailsIsNone_Serial()
        {
            var ma = Seq(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.SequenceSerial();

            var mc = TryOptionAsync<Seq<int>>(new Exception("fail"));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
    }
}
