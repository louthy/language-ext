using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class ArrIdentity
    {
        [Fact]
        public void EmptyArrayIsSuccess()
        {
            Arr<Identity<int>> ma = Empty;

            var mb = ma.Traverse(identity);
            
            Assert.Equal(Id(Arr<int>.Empty), mb);
        }

        [Fact]
        public void ArrOfIdentitiesIsIdentityOfArr()
        {
            var ma = Array(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Array(1, 3, 5)), mb);
        }
    }
}
