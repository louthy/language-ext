using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class SeqOptionAsync
    {
        [Fact]
        public async void EmptyIsSomeEmpty_Parallel()
        {
            var ma = Seq<OptionAsync<int>>();

            var mb = ma.SequenceParallel();

            var mc = SomeAsync(Seq<int>());
            
            Assert.True(await (mb == mc));        
        }
        
        [Fact]
        public async void EmptyIsSomeEmpty_Serial()
        {
            var ma = Seq<OptionAsync<int>>();

            var mb = ma.SequenceSerial();

            var mc = SomeAsync(Seq<int>());
            
            Assert.True(await (mb == mc));        
        }
        
        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Parallel()
        {
            var ma = Seq(SomeAsync(1), SomeAsync(2), SomeAsync(3));

            var mb = ma.SequenceParallel();

            var mc = SomeAsync(Seq(1, 2, 3));

            Assert.True(await (mb == mc));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Serial()
        {
            var ma = Seq(SomeAsync(1), SomeAsync(2), SomeAsync(3));

            var mb = ma.SequenceSerial();

            var mc = SomeAsync(Seq(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Parallel()
        {
            var ma = Seq(SomeAsync(1), SomeAsync(2), None);

            var mb = ma.SequenceParallel();

            Assert.True(await (mb == None));
        }
        
                
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Serial()
        {
            var ma = Seq(SomeAsync(1), SomeAsync(2), None);

            var mb = ma.SequenceSerial();

            Assert.True(await (mb == None));
        }
    }
}
