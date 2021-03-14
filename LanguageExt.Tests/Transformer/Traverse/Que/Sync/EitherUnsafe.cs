using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class EitherUnsafeQue
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = LeftUnsafe<Error, Que<int>>(Error.New("alt"));
            var mb = ma.Traverse(identity);
            var mc = Queue(LeftUnsafe<Error, int>(Error.New("alt")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<EitherUnsafe<Error, int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightNonEmptyQueIsQueRight()
        {
            var ma = RightUnsafe<Error, Que<int>>(Queue(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Queue(
                RightUnsafe<Error, int>(1),
                RightUnsafe<Error, int>(2),
                RightUnsafe<Error, int>(3),
                RightUnsafe<Error, int>(4));

            Assert.True(mb == mc);
        }
    }
}
