using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class StckEitherUnsafe
    {
        [Fact]
        public void EmptyStckIsRightEmptyStck()
        {
            Stck<EitherUnsafe<Error, int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == RightUnsafe(Stck<int>.Empty));
        }
        
        [Fact]
        public void StckRightsIsRightStcks()
        {
            var ma = Stack(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3));
            var mb = ma.Traverse(Prelude.identity);
            var mc = RightUnsafe(Stack(1, 2, 3));
            
            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void StckRightAndLeftIsLeftEmpty()
        {
            var ma = Stack(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative")));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
