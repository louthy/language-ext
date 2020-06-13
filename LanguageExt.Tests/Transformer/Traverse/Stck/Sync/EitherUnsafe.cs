using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class EitherUnsafe
    {
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, Stck<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Stack<EitherUnsafe<Error, int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightStackIsStackRight()
        {
            var ma = RightUnsafe<Error, Stck<int>>(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(RightUnsafe<Error, int>(4), Right(3), Right(2), Right(1));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void LeftStackIsStackLeft()
        {
            var ma = LeftUnsafe<Error, Stck<int>>(Error.New("error"));
            var mb = ma.Traverse(identity);
            var mc = Stack(LeftUnsafe<Error, int>(Error.New("error")));

            Assert.Equal(mc, mb);
        }
    }
}
