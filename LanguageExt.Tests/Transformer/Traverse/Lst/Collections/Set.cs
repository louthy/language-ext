using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    public class SetLst
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<Lst<int>> ma = Empty;

            var mb = ma.Traverse(mx => mx).As();

            var mc = List.singleton(Set<int>.Empty);
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SetLstCrossProduct()
        {
            var ma = Set(List(1, 2), List(10, 20, 30));

            var mb = ma.Traverse(mx => mx).As();
            var mbb = ma.Traverse(x => x).As();

            var mc = List(
                Set(1, 10), 
                Set(1, 20), 
                Set(1, 30), 
                Set(2, 10), 
                Set(2, 20), 
                Set(2, 30));
            
            Assert.True(mb == mc);
            Assert.True(mbb == mc);
        }
        
                
        [Fact]
        public void SetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Set(List<int>(), List(1, 2, 3));

            var mb = ma.Traverse(mx => mx).As();

            var mc = Lst<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set(List<int>(), List<int>());

            var mb = ma.Traverse(mx => mx).As();

            var mc = Lst<Set<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
