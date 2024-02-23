using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class ValidationIEnumerable
    {
        [Fact]
        public void FailIsSingletonFail()
        {
            var ma = Fail<Error, EnumerableM<int>>(Error.New("alt"));
            var mb = ma.Traverse(mx => mx).As();

            IEnumerable<Validation<Error, int>> mc = new[] { Fail<Error, int>(Error.New("alt")) };

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<Error, EnumerableM<int>>(EnumerableM.empty<int>());
            var mb = ma.Traverse(mx => mx).As();

            var mc = Enumerable.Empty<Validation<Error, int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SuccessNonEmptyIEnumerableIsIEnumerableSuccesses()
        {
            var ma = Success<Error, EnumerableM<int>>([1, 2, 3, 4]);
            var mb = ma.Traverse(mx => mx).As();

            var mc = new[]
            {
                Success<Error, int>(1),
                Success<Error, int>(2),
                Success<Error, int>(3),
                Success<Error, int>(4)
            }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
