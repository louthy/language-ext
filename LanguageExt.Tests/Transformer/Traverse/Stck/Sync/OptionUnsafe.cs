using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class OptionUnsafe
    {
        [Fact]
        public void SomeEmptyIsEmptySome()
        {
            var ma = SomeUnsafe(Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stack<OptionUnsafe<int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeStackIsStackSomes()
        {
            var ma = SomeUnsafe(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3), SomeUnsafe(4));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void NoneIsSingleNone()
        {
            var ma = OptionUnsafe<Stck<int>>.None;
            var mb = ma.Traverse(identity);
            var mc = Stack(OptionUnsafe<int>.None);

            Assert.Equal(mc, mb);
        }
    }
}
