using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Collections
{
    public class StackTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Stck<A>> ma, TryAsync<Stck<A>> mb) =>
            EqAsyncClass<TryAsync<Stck<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = Stack<TryAsync<int>>();

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryAsync(Stack<int>());
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = Stack(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryAsync(Stack(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = Stack(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryAsync<Stck<int>>(new Exception("fail"));
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
    }
}
