using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Collections
{
    public class SeqEitherAsync
    {
        [Fact]
        public async void EmptyIsSomeEmpty_Parallel()
        {
            var ma = Seq<EitherAsync<string, int>>();

            var mb = ma.SequenceParallel();

            var mc = RightAsync<string, Seq<int>>(Seq<int>());
            
            Assert.True(await (mb == mc));        
        }
        
        [Fact]
        public async void EmptyIsSomeEmpty_Serial()
        {
            var ma = Seq<EitherAsync<string, int>>();

            var mb = ma.SequenceSerial();

            var mc = RightAsync<string, Seq<int>>(Seq<int>());
            
            Assert.True(await (mb == mc));        
        }
        
        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Parallel()
        {
            var ma = Seq(RightAsync<string, int>(1), RightAsync<string, int>(2), RightAsync<string, int>(3));

            var mb = ma.SequenceParallel();

            var mc = RightAsync<string, Seq<int>>(Seq(1, 2, 3));

            Assert.True(await (mb == mc));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Serial()
        {
            var ma = Seq(RightAsync<string, int>(1), RightAsync<string, int>(2), RightAsync<string, int>(3));

            var mb = ma.SequenceSerial();

            var mc = RightAsync<string, Seq<int>>(Seq(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfSomesAndLeftsIsLeft_Parallel()
        {
            var ma = Seq(RightAsync<string, int>(1), RightAsync<string, int>(2), LeftAsync<string, int>("alt"));

            var mb = ma.SequenceParallel();

            var mc = LeftAsync<string, Seq<int>>("alt");
            
            Assert.True(await (mb == mc));
        }
        
                
        [Fact]
        public async void CollectionOfSomesAndLeftsIsLeft_Serial()
        {
            var ma = Seq(RightAsync<string, int>(1), RightAsync<string, int>(2), LeftAsync<string, int>("alt"));

            var mb = ma.SequenceSerial();

            var mc = LeftAsync<string, Seq<int>>("alt");
            
            Assert.True(await (mb == mc));
        }
    }
}
