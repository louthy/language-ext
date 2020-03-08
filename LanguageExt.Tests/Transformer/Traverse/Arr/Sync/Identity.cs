using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Sync
{
    public class IdentityArr
    {
        [Fact]
        public void IdEmptyIsEmpty()
        {
            var ma = Id<Arr<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Array<Identity<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void IdNonEmptyArrIsArrId()
        {
            var ma = Id(Array(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Array(Id(1), Id(2), Id(3));

            Assert.True(mb == mc);
        }
    }
}
