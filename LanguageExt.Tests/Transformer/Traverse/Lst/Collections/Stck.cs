using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    public class StckLst
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Stck<Lst<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Lst<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void StckLstCrossProduct()
        {
            var ma = Stack(List(1, 2), List(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = List(Stack(1, 10), Stack(1, 20), Stack(1, 30), Stack(2, 10), Stack(2, 20), Stack(2, 30));

            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void StckOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Stack(List<int>(), List(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Lst<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void StckOfEmptiesIsEmpty()
        {
            var ma = Stack(List<int>(), List<int>());

            var mb = ma.Traverse(identity);

            var mc = Lst<Stck<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
