using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class EitherTask
    {
        /*[Fact]
        public async void LeftIsSomeLeft()
        {
            var ma = Left<Error, Task<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeAsync(Left<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = Right<Error, Task<int>>(None);
            var mb = ma.Sequence();
            var mc = Task<Either<Error, int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSomeIsSomeRight()
        {
            var ma = Right<Error, Task<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(Right<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }*/
    }
}
