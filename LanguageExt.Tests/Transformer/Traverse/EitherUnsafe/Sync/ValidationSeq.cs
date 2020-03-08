using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Sync
{
    public class ValidationSeqEitherUnsafe
    {
        [Fact]
        public void FailIsRightFail()
        {
            var ma = Fail<Error, EitherUnsafe<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, Validation<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessLeftIsLeft()
        {
            var ma = Success<Error, EitherUnsafe<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, Validation<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessRightIsRight()
        {
            var ma = Success<Error, EitherUnsafe<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, Validation<Error, int>>(Success<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
