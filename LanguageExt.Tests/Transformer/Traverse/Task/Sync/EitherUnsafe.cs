using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class EitherUnsafeTask
    {
        /*[Fact]
        public async void LeftIsNone()
        {
            var ma = LeftUnsafe<Error, Task<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeAsync(LeftUnsafe<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);

        }
        
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = RightUnsafe<Error, Task<int>>(None);
            var mb = ma.Sequence();
            var mc = Task<EitherUnsafe<Error, int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSomeIsSomeRight()
        {
            var ma = RightUnsafe<Error, Task<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(RightUnsafe<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }*/
    }
}
