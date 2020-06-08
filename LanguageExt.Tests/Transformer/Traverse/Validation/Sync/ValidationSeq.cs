using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class ValidationSeq
    {
        [Fact]
        public void FailIsSuccessFail()
        {
            var ma = Fail<Error, Validation<Error, int>>(Error.New("Fail"));
            var mb = ma.Sequence();
            var mc = Success<Error, Validation<Error, int>>(Fail<Error, int>(Error.New("Fail")));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SuccessIsRightSuccess()
        {
            var ma = Success<Error, Validation<Error, int>>(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, Validation<Error, int>>(Success<Error, int>(12));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SuccessFailIsFail()
        {
            var ma = Success<Error, Validation<Error, int>>(Fail<Error, int>(Error.New("Fail")));
            var mb = ma.Sequence();
            var mc = Fail<Error, Validation<Error, int>>(Error.New("Fail"));

            Assert.Equal(mc, mb);
        }
    }
}
