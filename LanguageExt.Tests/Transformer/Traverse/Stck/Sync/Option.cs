using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class Option
    {
        [Fact]
        public void SomeEmptyIsEmptySome()
        {
            var ma = Some(Stack<int>());
            var mb = ma.Traverse(identity);
            var mc = Stack<Option<int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeStackIsStackSomes()
        {
            var ma = Some(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Some(1), Some(2), Some(3), Some(4));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void NoneIsSingleNone()
        {
            var ma = Option<Stck<int>>.None;
            var mb = ma.Traverse(identity);
            var mc = Stack(Option<int>.None);

            Assert.Equal(mc, mb);
        }
    }
}
