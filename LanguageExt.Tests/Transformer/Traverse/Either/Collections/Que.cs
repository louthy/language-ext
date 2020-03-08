using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections
{
    public class QueEither
    {
        [Fact]
        public void EmptyQueIsRightEmptyQue()
        {
            Que<Either<Error, int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Right(Que<int>.Empty));
        }
        
        [Fact]
        public void QueRightsIsRightQues()
        {
            var ma = Queue(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Right(Queue(1, 2, 3)));
        }
        
        [Fact]
        public void QueRightAndLeftIsLeftEmpty()
        {
            var ma = Queue(Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative")));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Left(Error.New("alternative")));
        }
    }
}
