using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class ValidationSeqTask
    {
        /*[Fact]
        public async void FailIsSomeFail()
        {
            var ma = Fail<Error, Task<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeAsync(Fail<Error, int>(Error.New("alt")));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccessNoneIsNone()
        {
            var ma = Success<Error, Task<int>>(None);
            var mb = ma.Sequence();
            var mc = Task<Validation<Error, int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccessSomeIsSomeSuccess()
        {
            var ma = Success<Error, Task<int>>(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(Success<Error, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }*/
    }
}
