using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class IdentityLst
    {
        [Fact]
        public void IdEmptyIsEmpty()
        {
            var ma = Id<Lst<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = List<Identity<int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void IdNonEmptyLstIsLstId()
        {
            var ma = Id(List(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = List(Id(1), Id(2), Id(3));
            
            Assert.True(mb == mc);
        }
    }
}
