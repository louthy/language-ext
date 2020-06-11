using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Sync
{
    public class EitherUnsafeOptionUnsafe
    {
        [Fact]
        public void LeftIsSomeLeft()
        {
            var ma = LeftUnsafe<Error, OptionUnsafe<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeUnsafe<EitherUnsafe<Error, int>>(Error.New("alt"));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightNoneIsNone()
        {
            var ma = RightUnsafe<Error, OptionUnsafe<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionUnsafe<EitherUnsafe<Error, int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightSomeIsSomeRight()
        {
            var ma = RightUnsafe<Error, OptionUnsafe<int>>(SomeUnsafe(1234));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(RightUnsafe<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
