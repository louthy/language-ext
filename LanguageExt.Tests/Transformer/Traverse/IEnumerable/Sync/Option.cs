using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class OptionIEnumerable
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = Option<Iterable<int>>.None;
            var mb = ma.Traverse(mx => mx).As();

            IEnumerable<Option<int>> mc = new[] { Option<int>.None };

            Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = Some(Iterable.empty<int>());
            var mb = ma.Traverse(mx => mx).As();

            var mc = Enumerable.Empty<Option<int>>();

            Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
        }

        [Fact]
        public void SomeNonEmptyIEnumerableIsIEnumerableSomes()
        {
            var ma = Some(new[] { 1, 2, 3 }.AsIterable());
            var mb = ma.Traverse(mx => mx).As();

            var mc = new[] { Some(1), Some(2), Some(3) }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
        }
    }
}
