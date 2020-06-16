using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Collections
{
    public class StckSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<Set<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            var mc = Set<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        // TODO: OrdDefault
        [Fact]
        public void StckSetCrossProduct()
        {
            var ma = Stack(Set(1, 2), Set(10, 20, 30));
            var mb = ma.Traverse(identity);
            var mc = Set(Stack(1, 10), Stack(1, 20), Stack(1, 30), Stack(2, 10), Stack(2, 20), Stack(2, 30));
            
            Assert.True(mb == mc);
        }
                
        [Fact]
        public void StckOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Stack(Set<int>(), Set(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Set<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack(Set<int>(), Set<int>());
            var mb = ma.Traverse(identity);
            var mc = Set<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
