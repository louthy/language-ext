using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Collections
{
    public class SeqTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Seq<A>> ma, TryAsync<Seq<A>> mb) =>
            EqAsyncClass<TryAsync<Seq<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty_Parallel()
        {
            var ma = Seq<TryAsync<int>>();

            var mb = ma.SequenceParallel();

            var mc = TryAsync(Seq<int>());

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
        [Fact]
        public async void EmptyIsSuccEmpty_Serial()
        {
            var ma = Seq<TryAsync<int>>();

            var mb = ma.SequenceSerial();

            var mc = TryAsync(Seq<int>());

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Parallel()
        {
            var ma = Seq(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.SequenceParallel();

            var mc = TryAsync(Seq(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }

        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Serial()
        {
            var ma = Seq(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.SequenceSerial();

            var mc = TryAsync(Seq(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail_Parallel()
        {
            var ma = Seq(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.SequenceParallel();

            var mc = TryAsync<Seq<int>>(new Exception("fail"));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
        
                
        [Fact]
        public async void CollectionOfSuccsAndFailsIsNone_Serial()
        {
            var ma = Seq(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.SequenceSerial();

            var mc = TryAsync<Seq<int>>(new Exception("fail"));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);        
        }
    }
}
