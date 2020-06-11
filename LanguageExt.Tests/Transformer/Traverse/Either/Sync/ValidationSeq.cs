using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync
{
    public class ValidationSeqEither
    {
        [Fact]
        public void FailIsRightFail()
        {
            var ma = Fail<Error, Either<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Right<Error, Validation<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessLeftIsLeft()
        {
            var ma = Success<Error, Either<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = Left<Error, Validation<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessRightIsRight()
        {
            var ma = Success<Error, Either<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = Right<Error, Validation<Error, int>>(Success<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
