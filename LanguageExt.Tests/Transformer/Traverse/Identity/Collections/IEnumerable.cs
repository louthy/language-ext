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

            Assert.Equal(Id(Iterable.empty<int>()), mb);
        }

        [Fact]
        public void EnumerableOfIdentitiesIsIdentityOfEnumerable()
        {
            var ma = IterableExtensions.AsIterable(List(Id(1), Id(3), Id(5)));

            var mb = ma.Traverse(x => x).As();

            Assert.Equal(Id(IterableExtensions.AsIterable(List(1, 3, 5))), mb);
        }
    }
}
