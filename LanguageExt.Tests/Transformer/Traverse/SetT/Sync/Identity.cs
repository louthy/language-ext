using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class IdentitySet
    {
        [Fact]
        public void IdEmptyIsEmpty()
        {
            var ma = Id<Set<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Set<Identity<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void IdNonEmptySetIsSetId()
        {
            var ma = Id(Set(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Set(Id(1), Id(2), Id(3));
            
            Assert.True(mb == mc);
        }
    }
}
