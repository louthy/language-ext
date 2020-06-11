using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Collections
{
    public class QueueTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Que<A>> ma, TryAsync<Que<A>> mb) =>
            EqAsyncClass<TryAsync<Que<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void EmptyIsSuccEmpty()
        {
            var ma = Queue<TryAsync<int>>();

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryAsync(Queue<int>());
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSuccsIsSuccCollection()
        {
            var ma = Queue(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryAsync(Queue(1, 2, 3));

            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail()
        {
            var ma = Queue(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryAsync<Que<int>>(new Exception("fail"));
            
            var mr = await (Eq(mb, mc));
            Assert.True(mr);
        }
    }
}
