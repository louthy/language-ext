using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class ArrOptionAsync
    {
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = Array<OptionAsync<int>>();

            var mb = ma.Sequence();

            var mc = SomeAsync(Array<int>());
            
            Assert.True(await (mb == mc));        
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = Array(SomeAsync(1), SomeAsync(2), SomeAsync(3));

            var mb = ma.Sequence();

            var mc = SomeAsync(Array(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = Array(SomeAsync(1), SomeAsync(2), None);

            var mb = ma.Sequence();

            Assert.True(await (mb == None));
        }
    }
}
