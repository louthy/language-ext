using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync
{
    public class OptionUnsafeEither
    {
        [Fact]
        public void NoneLeftIsRightNone()
        {
            var ma = OptionUnsafe<Either<Error, int>>.None;
            var mb = ma.Sequence();
            var mc = Right<Error, OptionUnsafe<int>>(None);

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeLeftIsLeft()
        {
            var ma = SomeUnsafe<Either<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = Left<Error, OptionUnsafe<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeRightIsRight()
        {
            var ma = SomeUnsafe<Either<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = Right<Error, OptionUnsafe<int>>(SomeUnsafe(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
