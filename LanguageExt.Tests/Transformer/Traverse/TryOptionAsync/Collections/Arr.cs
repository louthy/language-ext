using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Collections
{
    public class ArrTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Arr<A>> ma, TryOptionAsync<Arr<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Arr<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = Array<TryOptionAsync<int>>();

            var mb = ma.Sequence();

            var mc = TryOptionAsync(Array<int>());
            
            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = Array(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.Sequence();

            var mc = TryOptionAsync(Array(1, 2, 3));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = Array(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryOptionAsync<Arr<int>>(new Exception("fail"));
            
            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
