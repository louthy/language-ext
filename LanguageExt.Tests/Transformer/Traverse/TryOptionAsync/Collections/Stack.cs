using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Collections
{
    public class StackTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Stck<A>> ma, TryOptionAsync<Stck<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Stck<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = Stack<TryOptionAsync<int>>();

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryOptionAsync(Stack<int>());
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = Stack(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryOptionAsync(Stack(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = Stack(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryOptionAsync<Stck<int>>(new Exception("fail"));
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
    }
}
