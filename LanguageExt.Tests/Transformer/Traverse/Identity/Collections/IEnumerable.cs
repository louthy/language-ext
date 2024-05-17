using LanguageExt.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class IEnumerable
    {
        [Fact]
        public void EmptyIEnumerableIsEmpty()
        {
            var ma = EnumerableM.empty<Identity<int>>();

            var mb = ma.Traverse(x => x).As();

            Assert.Equal(Id(EnumerableM.empty<int>()), mb);
        }

        [Fact]
        public void EnumerableOfIdentitiesIsIdentityOfEnumerable()
        {
            var ma = List(Id(1), Id(3), Id(5)).AsEnumerableM();

            var mb = ma.Traverse(x => x).As();

            Assert.Equal(Id(List(1, 3, 5).AsEnumerableM()), mb);
        }
    }
}
