using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class LstEitherUnsafe
    {
        [Fact]
        public void EmptyLstIsRightEmptyLst()
        {
            Lst<EitherUnsafe<Error, int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(Lst<int>.Empty));
        }
        
        [Fact]
        public void LstRightsIsRightLsts()
        {
            var ma = List(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3));

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(List(1, 2, 3)));
        }
        
        [Fact]
        public void LstRightAndLeftIsLeftEmpty()
        {
            var ma = List(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative")));

            var mb = ma.Sequence();

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
