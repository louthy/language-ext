using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class EitherUnsafeIEnumerable
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = LeftUnsafe<Error, IEnumerable<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = new[] { LeftUnsafe<Error, int>(Error.New("alt")) }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, IEnumerable<int>>(Enumerable.Empty<int>());
            var mb = ma.Sequence();
            var mc = Enumerable.Empty<EitherUnsafe<Error, int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void RightNonEmptyIEnumerableIsIEnumerableRight()
        {
            var ma = RightUnsafe<Error, IEnumerable<int>>(new[] { 1, 2, 3, 4 });
            var mb = ma.Sequence();
            var mc = new[]
            {
                RightUnsafe<Error, int>(1),
                RightUnsafe<Error, int>(2),
                RightUnsafe<Error, int>(3),
                RightUnsafe<Error, int>(4)
            }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
