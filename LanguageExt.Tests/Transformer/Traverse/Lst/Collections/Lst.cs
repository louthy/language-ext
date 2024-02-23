using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    public class LstLst
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Lst<Lst<int>> ma = Empty;

            var mb = ma.Traverse(mx => mx).As();


            var mc = Lst<Lst<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void LstLstCrossProduct()
        {
            var ma = List(List(1, 2), List(10, 20, 30));

            var mb = ma.Traverse(mx => mx).As();


            var mc = List(List(1, 10), List(1, 20), List(1, 30), List(2, 10), List(2, 20), List(2, 30));
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void LstOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = List(List<int>(), List(1, 2, 3));

            var mb = ma.Traverse(mx => mx).As();


            var mc = Lst<Lst<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void LstOfEmptiesIsEmpty()
        {
            var ma = List(List<int>(), List<int>());

            var mb = ma.Traverse(mx => mx).As();


            var mc = Lst<Lst<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
