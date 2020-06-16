using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class LstOptionAsync
    {
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = List<OptionAsync<int>>();

            var mb = ma.Sequence();

            var mc = SomeAsync(List<int>());
            
            Assert.True(await (mb == mc));        
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = List(SomeAsync(1), SomeAsync(2), SomeAsync(3));

            var mb = ma.Sequence();

            var mc = SomeAsync(List(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = List(SomeAsync(1), SomeAsync(2), None);

            var mb = ma.Sequence();

            Assert.True(await (mb == None));
        }
    }
}
