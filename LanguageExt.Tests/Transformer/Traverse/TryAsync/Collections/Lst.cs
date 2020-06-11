using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Collections
{
    public class LstTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Lst<A>> ma, TryAsync<Lst<A>> mb) =>
            EqAsyncClass<TryAsync<Lst<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = List<TryAsync<int>>();

            var mb = ma.Sequence();

            var mc = TryAsync(List<int>());
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = List(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.Sequence();

            var mc = TryAsync(List(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = List(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryAsync<Lst<int>>(new Exception("fail"));
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
    }
}
