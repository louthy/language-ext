using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync
{
    public class EitherUnsafeArr
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = LeftUnsafe<Error, Arr<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Array(LeftUnsafe<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, Arr<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Array<EitherUnsafe<Error, int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void RightNonEmptyArrIsArrRight()
        {
            var ma = RightUnsafe<Error, Arr<int>>(Array(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = Array(
                RightUnsafe<Error, int>(1),
                RightUnsafe<Error, int>(2),
                RightUnsafe<Error, int>(3),
                RightUnsafe<Error, int>(4));

            Assert.True(mb == mc);
        }
    }
}
