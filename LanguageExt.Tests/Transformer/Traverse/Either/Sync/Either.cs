using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync
{
    public class EitherEither
    {
        [Fact]
        public void LeftIsRightLeft()
        {
            var ma = Left<Error, Either<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Right<Error, Either<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightLeftIsLeft()
        {
            var ma = Right<Error, Either<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = Left<Error, Either<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightRightIsRight()
        {
            var ma = Right<Error, Either<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = Right<Error, Either<Error, int>>(Right(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
