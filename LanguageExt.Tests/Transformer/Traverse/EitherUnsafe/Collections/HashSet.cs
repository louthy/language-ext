using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class HashSetEitherUnsafe
    {
        [Fact]
        public void EmptyHashSetIsRightEmptyHashSet()
        {
            HashSet<EitherUnsafe<Error, int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Right(HashSet<int>.Empty));
        }
        
        [Fact]
        public void HashSetRightsIsRightHashSets()
        {
            var ma = HashSet(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3));

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(HashSet(1, 2, 3)));
        }
        
        [Fact]
        public void HashSetRightAndLeftIsLeftEmpty()
        {
            var ma = HashSet(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative")));

            var mb = ma.Sequence();

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
