using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class TryOptionIEnumerable
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryOptionFail<IEnumerable<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            IEnumerable<TryOption<int>> mc = new[] { TryOptionFail<int>(new Exception("fail")) };

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc(Enumerable.Empty<int>());
            var mb = ma.Sequence();
            var mc = Enumerable.Empty<TryOption<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SuccNonEmptyIEnumerableIsIEnumerableSuccs()
        {
            var ma = TryOptionSucc(new[] { 1, 2, 3 }.AsEnumerable());
            var mb = ma.Sequence();
            var mc = new[] { TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3) }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
