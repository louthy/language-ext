using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class Lst
    {
        [Fact]
        public void EmptyLstIsEmpty()
        {
            var ma = Lst<Identity<int>>.Empty;

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Lst<int>.Empty), mb);
        }

        [Fact]
        public void LstOfIdentitiesIsIdentityOfLst()
        {
            var ma = List(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(List(1, 3, 5)), mb);
        }
    }
}
