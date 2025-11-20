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
            var ma = Iterable.empty<Identity<int>>();

            var mb = ma.Traverse(x => x).As();

            Assert.True(Id(Iterable.empty<int>()) == mb);
        }

        [Fact]
        public void EnumerableOfIdentitiesIsIdentityOfEnumerable()
        {
            var ma = Iterable.create(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(x => x).As();

            Assert.True(Id(Iterable.create(1, 3, 5)) == mb);
        }
    }
}
