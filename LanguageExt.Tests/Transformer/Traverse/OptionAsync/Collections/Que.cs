using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class QueueOptionAsync
    {
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = Queue<OptionAsync<int>>();

            var mb = ma.Traverse(identity);

            var mc = SomeAsync(Queue<int>());
            
            Assert.True(await (mb == mc));        
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = Queue(SomeAsync(1), SomeAsync(2), SomeAsync(3));

            var mb = ma.Traverse(identity);

            var mc = SomeAsync(Queue(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = Queue(SomeAsync(1), SomeAsync(2), None);

            var mb = ma.Traverse(identity);

            Assert.True(await (mb == None));
        }
    }
}
