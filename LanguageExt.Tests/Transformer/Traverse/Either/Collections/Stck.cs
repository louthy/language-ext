using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections
{
    public class StckEither
    {
        [Fact]
        public void EmptyStckIsRightEmptyStck()
        {
            Stck<Either<Error, int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Right(Stck<int>.Empty));
        }
        
        [Fact]
        public void StckRightsIsRightStcks()
        {
            var ma = Stack(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3));
            var mb = ma.Traverse(Prelude.identity);
            var mc = Right(Stack(1, 2, 3));
            
            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void StckRightAndLeftIsLeftEmpty()
        {
            var ma = Stack(Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative")));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Left(Error.New("alternative")));
        }
    }
}
