using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Collections
{
    public class ArrEitherAsync
    {
        [Fact]
        public async void EmptyIsRightEmpty()
        {
            var ma = Array<EitherAsync<string, int>>();

            var mb = ma.Sequence();

            var mc = RightAsync<string, Arr<int>>(Array<int>());
            
            Assert.True(await (mb == mc));        
        }

        [Fact]
        public async void CollectionOfRightssIsRightCollection()
        {
            var ma = Array(RightAsync<string, int>(1), RightAsync<string, int>(2), RightAsync<string, int>(3));

            var mb = ma.Sequence();

            var mc = RightAsync<string, Arr<int>>(Array(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfRightssAndLeftsIsLeft()
        {
            var ma = Array(RightAsync<string, int>(1), RightAsync<string, int>(2), LeftAsync<string, int>("alt"));

            var mb = ma.Sequence();

            var mc = LeftAsync<string, Arr<int>>("alt");
            
            Assert.True(await (mb == mc));
        }
    }
}
