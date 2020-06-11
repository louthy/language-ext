using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class QueEitherUnsafe
    {
        [Fact]
        public void EmptyQueIsRightEmptyQue()
        {
            Que<EitherUnsafe<Error, int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == RightUnsafe(Que<int>.Empty));
        }
        
        [Fact]
        public void QueRightsIsRightQues()
        {
            var ma = Queue(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == RightUnsafe(Queue(1, 2, 3)));
        }
        
        [Fact]
        public void QueRightAndLeftIsLeftEmpty()
        {
            var ma = Queue(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative")));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
