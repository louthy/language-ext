using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class Either
    {
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Right<Error, Stck<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Stack<Either<Error, int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightStackIsStackRight()
        {
            var ma = Right<Error, Stck<int>>(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Right<Error, int>(1), Right(2), Right(3), Right(4));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void LeftStackIsStackLeft()
        {
            var ma = Left<Error, Stck<int>>(Error.New("error"));
            var mb = ma.Traverse(identity);
            var mc = Stack(Left<Error, int>(Error.New("error")));

            Assert.Equal(mc, mb);
        }
    }
}
