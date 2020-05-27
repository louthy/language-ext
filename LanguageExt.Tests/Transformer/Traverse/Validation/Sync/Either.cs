using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class Either
    {
        [Fact]
        public void LeftIsSuccessLeft()
        {
            var ma = Left<Error, Validation<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Success<Error, Either<Error, int>>(Left(Error.New("alt")));
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightSuccessIsSuccess()
        {
            var ma = Right<Error, Validation<Error, int>>(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, Either<Error, int>>(Right(12));
            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightFailIsFail()
        {
            var ma = Right<Error, Validation<Error, int>>(Fail<Error, int>(Error.New("Error")));
            var mb = ma.Sequence();
            var mc = Fail<Error, Either<Error, int>>(Error.New("Error"));
            Assert.Equal(mc, mb);
        }
    }
}
