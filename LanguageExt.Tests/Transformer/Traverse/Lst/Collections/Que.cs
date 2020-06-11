using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    public class QueLst
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Que<Lst<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Lst<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void QueLstCrossProduct()
        {
            var ma = Queue(List(1, 2), List(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = List(Queue(1, 10), Queue(1, 20), Queue(1, 30), Queue(2, 10), Queue(2, 20), Queue(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void QueOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Queue(List<int>(), List(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Lst<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void QueOfEmptiesIsEmpty()
        {
            var ma = Queue(List<int>(), List<int>());

            var mb = ma.Traverse(identity);

            var mc = Lst<Que<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
