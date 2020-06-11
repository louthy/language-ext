using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class EitherUnsafe
    {
        [Fact]
        public void LeftIsSuccessLeft()
        {
            var ma = LeftUnsafe<Error, Validation<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Success<Error, EitherUnsafe<Error, int>>(LeftUnsafe(Error.New("alt")));
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightSuccessIsSuccess()
        {
            var ma = RightUnsafe<Error, Validation<Error, int>>(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, EitherUnsafe<Error, int>>(RightUnsafe(12));
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightFailIsFail()
        {
            var ma = RightUnsafe<Error, Validation<Error, int>>(Fail<Error, int>(Error.New("Error")));
            var mb = ma.Sequence();
            var mc = Fail<Error, EitherUnsafe<Error, int>>(Error.New("Error"));
            Assert.Equal(mc, mb);
        }
    }
}
