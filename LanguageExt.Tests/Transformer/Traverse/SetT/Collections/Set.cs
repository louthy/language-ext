using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Collections
{
    public class SetSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<Set<int>> ma = Empty;
            var mb = ma.Sequence();
            var mc = Set<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SetSetCrossProduct()
        {
            var ma = Set(Set(1, 2), Set(10, 20, 30));
            var mb = ma.Sequence();
            var mc = Set(Set(1, 10), Set(1, 20), Set(1, 30), Set(2, 10), Set(2, 20), Set(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void SetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Set(Set<int>(), Set(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Set<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set(Set<int>(), Set<int>());
            var mb = ma.Sequence();
            var mc = Set<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
