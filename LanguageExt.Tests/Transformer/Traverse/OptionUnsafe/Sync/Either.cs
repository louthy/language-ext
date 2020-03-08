using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Sync
{
    public class EitherOptionUnsafe
    {
        [Fact]
        public void LeftIsSomeLeft()
        {
            var ma = Left<Error, OptionUnsafe<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Left<Error, int>(Error.New("alt")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightNoneIsNone()
        {
            var ma = Right<Error, OptionUnsafe<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionUnsafe<Either<Error, int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightSomeIsSomeRight()
        {
            var ma = Right<Error, OptionUnsafe<int>>(SomeUnsafe(1234));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Right<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
