using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class ArrEitherUnsafe
    {
        [Fact]
        public void EmptyArrIsRightEmptyArr()
        {
            Arr<EitherUnsafe<Error, int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Right(Arr<int>.Empty));
        }
        
        [Fact]
        public void ArrRightsIsRightArrs()
        {
            var ma = Array(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3));

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(Array(1, 2, 3)));
        }
        
        [Fact]
        public void ArrRightAndLeftIsLeftEmpty()
        {
            var ma = Array(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative")));

            var mb = ma.Sequence();

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
