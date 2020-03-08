using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class SeqEitherUnsafe
    {
        [Fact]
        public void EmptySeqIsRightEmptySeq()
        {
            Seq<Either<Error, int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(Seq<int>.Empty));
        }
        
        [Fact]
        public void SeqRightsIsRightSeqs()
        {
            var ma = Seq(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3));

            var mb = ma.Sequence();

            Assert.True(mb == RightUnsafe(Seq(1, 2, 3)));
        }
        
        [Fact]
        public void SeqRightAndLeftIsLeftEmpty()
        {
            var ma = Seq(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative")));

            var mb = ma.Sequence();

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
