using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Collections
{
    public class StackEitherAsync
    {
        [Fact]
        public async void EmptyIsRightEmpty()
        {
            var ma = Stack<EitherAsync<string, int>>();

            var mb = ma.Traverse(identity);

            var mc = RightAsync<string, Stck<int>>(Stack<int>());
            
            Assert.True(await (mb == mc));        
        }

        [Fact]
        public async void CollectionOfRightssIsRightCollection()
        {
            var ma = Stack(RightAsync<string, int>(1), RightAsync<string, int>(2), RightAsync<string, int>(3));

            var mb = ma.Traverse(identity);

            var mc = RightAsync<string, Stck<int>>(Stack(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfRightssAndLeftsIsLeft()
        {
            var ma = Stack(RightAsync<string, int>(1), RightAsync<string, int>(2), LeftAsync<string, int>("alt"));

            var mb = ma.Traverse(identity);

            var mc = LeftAsync<string, Stck<int>>("alt");
            
            Assert.True(await (mb == mc));
        }
    }
}
