using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class EitherQue
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = Left<Error, Que<int>>(Error.New("alt"));
            var mb = ma.Traverse(identity);
            var mc = Queue(Left<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Right<Error, Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<Either<Error, int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightNonEmptyQueIsQueRight()
        {
            var ma = Right<Error, Que<int>>(Queue(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Queue(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4));

            Assert.True(mb == mc);
        }
    }
}
