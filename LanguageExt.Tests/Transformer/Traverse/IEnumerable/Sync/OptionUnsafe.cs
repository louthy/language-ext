using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class OptionUnsafeIEnumerable
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = OptionUnsafe<IEnumerable<int>>.None;
            var mb = ma.Sequence();
            IEnumerable<OptionUnsafe<int>> mc = new[] { OptionUnsafe<int>.None };

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = SomeUnsafe<IEnumerable<int>>(Enumerable.Empty<int>());
            var mb = ma.Sequence();
            var mc = Enumerable.Empty<OptionUnsafe<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void SomeNonEmptyIEnumerableIsIEnumerableSomes()
        {
            var ma = SomeUnsafe(new[] { 1, 2, 3 }.AsEnumerable());
            var mb = ma.Sequence();
            var mc = new[] { SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3) }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
