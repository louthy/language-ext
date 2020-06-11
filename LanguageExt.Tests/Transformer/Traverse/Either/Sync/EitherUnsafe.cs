using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync
{
    public class EitherUnsafeEither
    {
        [Fact]
        public void LeftIsRightLeft()
        {
            var ma = LeftUnsafe<Error, Either<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Right<Error, EitherUnsafe<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightLeftIsLeft()
        {
            var ma = RightUnsafe<Error, Either<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = Left<Error, EitherUnsafe<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightRightIsRight()
        {
            var ma = RightUnsafe<Error, Either<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = Right<Error, EitherUnsafe<Error, int>>(Right(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
