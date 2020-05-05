using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class Set
    {
        [Fact]
        public void EmptySetIsEmpty()
        {
            var ma = Set<Identity<int>>.Empty;

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Set<int>.Empty), mb);
        }

        [Fact]
        public void SetOfIdentitiesIsIdentityOfSet()
        {
            var ma = Set(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Set(1, 3, 5)), mb);
        }
    }
}
