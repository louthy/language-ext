using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Collections
{
    public class ArrTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Arr<A>> ma, TryAsync<Arr<A>> mb) =>
            EqAsyncClass<TryAsync<Arr<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = Array<TryAsync<int>>();

            var mb = ma.Sequence();

            var mc = TryAsync(Array<int>());
            
            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = Array(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.Sequence();

            var mc = TryAsync(Array(1, 2, 3));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = Array(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryAsync<Arr<int>>(new Exception("fail"));
            
            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
