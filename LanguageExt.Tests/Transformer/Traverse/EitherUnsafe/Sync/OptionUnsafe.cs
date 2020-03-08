using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Sync
{
    public class OptionUnsafeEitherUnsafe
    {
        [Fact]
        public void NoneLeftIsRightNone()
        {
            var ma = OptionUnsafe<EitherUnsafe<Error, int>>.None;
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, OptionUnsafe<int>>(None);

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeLeftIsLeft()
        {
            var ma = SomeUnsafe<EitherUnsafe<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, OptionUnsafe<int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SomeRightIsRight()
        {
            var ma = SomeUnsafe<EitherUnsafe<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, OptionUnsafe<int>>(SomeUnsafe(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
