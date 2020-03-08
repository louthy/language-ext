using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Sync
{
    public class EitherUnsafeEitherUnsafe
    {
        [Fact]
        public void LeftIsRightLeft()
        {
            var ma = LeftUnsafe<Error, EitherUnsafe<Error, int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, EitherUnsafe<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightLeftIsLeft()
        {
            var ma = RightUnsafe<Error, EitherUnsafe<Error, int>>(Left(Error.New("alt")));
            var mb = ma.Sequence();
            var mc = LeftUnsafe<Error, EitherUnsafe<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightRightIsRight()
        {
            var ma = RightUnsafe<Error, EitherUnsafe<Error, int>>(Right(1234));
            var mb = ma.Sequence();
            var mc = RightUnsafe<Error, EitherUnsafe<Error, int>>(Right(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
