using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class Stck
    {
        [Fact]
        public void EmptyStckIsEmpty()
        {
            var ma = Stck<Identity<int>>.Empty;

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Stck<int>.Empty), mb);
        }

        [Fact]
        public void StckOfIdentitiesIsIdentityOfStck()
        {
            var ma = Stack(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Stack(1, 3, 5)), mb);
        }
    }
}
