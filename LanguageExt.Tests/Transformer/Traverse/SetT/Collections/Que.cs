using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Collections
{
    public class QueSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<Set<int>> ma = Empty;
            var mb = ma.Traverse(identity);
            var mc = Set<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void QueSetCrossProduct()
        {
            var ma = Queue(Set(1, 2), Set(10, 20, 30));
            var mb = ma.Traverse(identity);
            var mc = Set(Queue(1, 10), Queue(1, 20), Queue(1, 30), Queue(2, 10), Queue(2, 20), Queue(2, 30));
            
            Assert.True(mb == mc);
        }
                
        [Fact]
        public void QueOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Queue(Set<int>(), Set(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Set<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void QueOfEmptiesIsEmpty()
        {
            var ma = Queue(Set<int>(), Set<int>());
            var mb = ma.Traverse(identity);
            var mc = Set<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
