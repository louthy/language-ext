using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Collections
{
    public class QueueEitherAsync
    {
        [Fact]
        public async void EmptyIsRightEmpty()
        {
            var ma = Queue<EitherAsync<string, int>>();

            var mb = ma.Traverse(identity);

            var mc = RightAsync<string, Que<int>>(Queue<int>());
            
            Assert.True(await (mb == mc));        
        }

        [Fact]
        public async void CollectionOfRightssIsRightCollection()
        {
            var ma = Queue(RightAsync<string, int>(1), RightAsync<string, int>(2), RightAsync<string, int>(3));

            var mb = ma.Traverse(identity);

            var mc = RightAsync<string, Que<int>>(Queue(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfRightssAndLeftsIsLeft()
        {
            var ma = Queue(RightAsync<string, int>(1), RightAsync<string, int>(2), LeftAsync<string, int>("alt"));

            var mb = ma.Traverse(identity);

            var mc = LeftAsync<string, Que<int>>("alt");
            
            Assert.True(await (mb == mc));
        }
    }
}
