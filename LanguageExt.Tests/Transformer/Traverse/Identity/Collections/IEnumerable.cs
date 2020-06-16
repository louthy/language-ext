using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class IEnumerable
    {
        [Fact]
        public void EmptyIEnumerableIsEmpty()
        {
            var ma = Enumerable.Empty<Identity<int>>();

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Enumerable.Empty<int>()), mb);
        }

        [Fact]
        public void EnumerableOfIdentitiesIsIdentityOfEnumerable()
        {
            var ma = List(Id(1), Id(3), Id(5)).AsEnumerable();

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(List(1, 3, 5).AsEnumerable()), mb);
        }
    }
}
