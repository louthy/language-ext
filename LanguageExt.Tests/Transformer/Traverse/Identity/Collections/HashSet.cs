using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class HashSet
    {
        [Fact]
        public void EmptyHashSetIsSuccess()
        {
            HashSet<Identity<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(HashSet<int>.Empty), mb);
        }

        [Fact]
        public void HashSetOfIdentitiesIsSuccess()
        {
            var ma = HashSet(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(HashSet(1, 3, 5)), mb);
        }
    }
}
