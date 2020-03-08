using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    public class ArrLst
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Arr<Lst<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Lst<Arr<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void ArrLstCrossProduct()
        {
            var ma = Array(List(1, 2), List(10, 20, 30));

            var mb = ma.Sequence();

            var mc = List(Array(1, 10), Array(1, 20), Array(1, 30), Array(2, 10), Array(2, 20), Array(2, 30));
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void ArrOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Array(List<int>(), List(1, 2, 3));

            var mb = ma.Sequence();

            var mc = List<Arr<int>>();
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void ArrOfEmptiesIsEmpty()
        {
            var ma = Array(List<int>(), List<int>());

            var mb = ma.Sequence();

            var mc = List<Arr<int>>();
            
            Assert.True(mb == mc);
        }
    }
}
