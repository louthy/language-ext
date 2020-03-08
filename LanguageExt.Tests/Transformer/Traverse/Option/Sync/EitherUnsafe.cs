using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Sync
{
    public class EitherUnsafeOption
    {
        [Fact]
        public void LeftIsNone()
        {
            var ma = LeftUnsafe<Error, Option<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Some(LeftUnsafe<Error, int>(Error.New("alt")));

            var mr = mb == mc;
            
            Assert.True(mr);

        }
        
        [Fact]
        public void RightNoneIsNone()
        {
            var ma = RightUnsafe<Error, Option<int>>(None);
            var mb = ma.Sequence();
            var mc = Option<EitherUnsafe<Error, int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void RightSomeIsSomeRight()
        {
            var ma = RightUnsafe<Error, Option<int>>(Some(1234));
            var mb = ma.Sequence();
            var mc = Some(RightUnsafe<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
