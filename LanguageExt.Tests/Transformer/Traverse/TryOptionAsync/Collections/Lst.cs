using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Collections
{
    public class LstTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Lst<A>> ma, TryOptionAsync<Lst<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Lst<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = List<TryOptionAsync<int>>();

            var mb = ma.Sequence();

            var mc = TryOptionAsync(List<int>());
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = List(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.Sequence();

            var mc = TryOptionAsync(List(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = List(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryOptionAsync<Lst<int>>(new Exception("fail"));
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
    }
}
