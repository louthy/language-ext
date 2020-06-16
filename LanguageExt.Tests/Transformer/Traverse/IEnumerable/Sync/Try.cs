using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class TryIEnumerable
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryFail<IEnumerable<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            IEnumerable<Try<int>> mc = new[] { TryFail<int>(new Exception("fail")) };

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc(Enumerable.Empty<int>());
            var mb = ma.Sequence();
            var mc = Enumerable.Empty<Try<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SuccNonEmptyIEnumerableIsIEnumerableSuccs()
        {
            var ma = TrySucc(new[] { 1, 2, 3 }.AsEnumerable());
            var mb = ma.Sequence();
            var mc = new[] { TrySucc(1), TrySucc(2), TrySucc(3) }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
