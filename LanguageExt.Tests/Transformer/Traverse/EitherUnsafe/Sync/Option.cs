using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Sync
{
    public class OptionEitherUnsafe
    {
        [Fact]
        public void NoneLeftIsRightNone()
        {
            var ma = Option<EitherUnsafe<Error, int>>.None;
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, Option<int>>(None);

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeLeftIsLeft()
        {
            var ma = Some<EitherUnsafe<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, Option<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeRightIsRight()
        {
            var ma = Some<EitherUnsafe<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, Option<int>>(Some(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
