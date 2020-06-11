using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections
{
    public class LstEither
    {
        [Fact]
        public void EmptyLstIsRightEmptyLst()
        {
            Lst<Either<Error, int>> ma = Empty;

            var mb = ma.Sequence();

            Assert.True(mb == Right(Lst<int>.Empty));
        }
        
        [Fact]
        public void LstRightsIsRightLsts()
        {
            var ma = List(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3));

            var mb = ma.Sequence();

            Assert.True(mb == Right(List(1, 2, 3)));
        }
        
        [Fact]
        public void LstRightAndLeftIsLeftEmpty()
        {
            var ma = List(Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative")));

            var mb = ma.Sequence();

            Assert.True(mb == Left(Error.New("alternative")));
        }
    }
}
