using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class StackOptionAsync
    {
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = Stack<OptionAsync<int>>();

            var mb = ma.Traverse(identity);

            var mc = SomeAsync(Stack<int>());
            
            Assert.True(await (mb == mc));        
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = Stack(SomeAsync(1), SomeAsync(2), SomeAsync(3));

            var mb = ma.Traverse(identity);

            var mc = SomeAsync(Stack(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = Stack(SomeAsync(1), SomeAsync(2), None);

            var mb = ma.Traverse(identity);

            Assert.True(await (mb == None));
        }
    }
}
