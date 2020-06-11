using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Sync
{
    public class ValidationSeqEitherAsync
    {
        [Fact]
        public async void FailIsRightFail()
        {
            var ma = Fail<string, EitherAsync<string, int>>("alt");
            var mb = ma.Sequence();
            var mc = RightAsync<string, Validation<string, int>>(Fail<string, int>("alt"));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccLeftIsLeft()
        {
            var ma = Success<string, EitherAsync<string, int>>(LeftAsync<string, int>("alt"));
            var mb = ma.Sequence();
            var mc = LeftAsync<string, Validation<string, int>>("alt");

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccRightIsRightSucc()
        {
            var ma = Success<string, EitherAsync<string, int>>(RightAsync<string, int>(1234));
            var mb = ma.Sequence();
            var mc = RightAsync<string, Validation<string, int>>(Success<string, int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }

    }
}
