using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class EitherIEnumerable
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = Left<Error, IEnumerable<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = new[] { Left<Error, int>(Error.New("alt")) }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Right<Error, IEnumerable<int>>(Enumerable.Empty<int>());
            var mb = ma.Sequence();
            var mc = Enumerable.Empty<Either<Error, int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void RightNonEmptyIEnumerableIsIEnumerableRight()
        {
            var ma = Right<Error, IEnumerable<int>>(new[] { 1, 2, 3, 4 });
            var mb = ma.Sequence();
            var mc = new[] { Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4) };

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
