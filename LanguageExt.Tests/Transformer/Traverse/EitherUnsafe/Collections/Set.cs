using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class SetEitherUnsafe
    {
        [Fact]
        public void EmptySetIsRightEmptySet()
        {
            Set<EitherUnsafe<Error, int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(Set<int>.Empty));
        }
        
        [Fact]
        public void SetRightsIsRightSets()
        {
            var ma = Set(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3));

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(Set(1, 2, 3)));
        }
        
        [Fact]
        public void SetRightAndLeftIsLeftEmpty()
        {
            var ma = Set(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative")));

            var mb = ma.Sequence();

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
