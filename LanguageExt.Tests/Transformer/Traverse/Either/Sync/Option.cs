using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Sync
{
    public class OptionEither
    {
        [Fact]
        public void NoneLeftIsRightNone()
        {
            var ma = Option<Either<Error, int>>.None;
            var mb = ma.Sequence();
            var mc = Right<Error, Option<int>>(None);

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeLeftIsLeft()
        {
            var ma = Some<Either<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = Left<Error, Option<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeRightIsRight()
        {
            var ma = Some<Either<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = Right<Error, Option<int>>(Some(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
