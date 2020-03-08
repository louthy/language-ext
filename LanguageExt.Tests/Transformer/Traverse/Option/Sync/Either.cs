using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Sync
{
    public class EitherOption
    {
        [Fact]
        public void LeftIsSomeLeft()
        {
            var ma = Left<Error, Option<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Some(Left<Error, int>(Error.New("alt")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightNoneIsNone()
        {
            var ma = Right<Error, Option<int>>(None);
            var mb = ma.Sequence();
            var mc = Option<Either<Error, int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightSomeIsSomeRight()
        {
            var ma = Right<Error, Option<int>>(Some(1234));
            var mb = ma.Sequence();
            var mc = Some(Right<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
