using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Collections
{
    public class QueueTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Que<A>> ma, TryOptionAsync<Que<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Que<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = Queue<TryOptionAsync<int>>();

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryOptionAsync(Queue<int>());
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = Queue(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryOptionAsync(Queue(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = Queue(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryOptionAsync<Que<int>>(new Exception("fail"));
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
    }
}
