using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class Identity
    {
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Id<Stck<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Stack<Identity<int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void RightStackIsStackRight()
        {
            var ma = Id(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Id(1), Id(2), Id(3), Id(4));

            Assert.Equal(mc, mb);
        }
    }
}
