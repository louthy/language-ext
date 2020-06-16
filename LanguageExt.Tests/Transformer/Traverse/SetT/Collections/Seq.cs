using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Collections
{
    public class SeqSet
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<Set<int>> ma = Empty;
            var mb = ma.Sequence();
            var mc = Set<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqSetCrossProduct()
        {
            var ma = Seq(Set(1, 2), Set(10, 20, 30));
            var mb = ma.Sequence();
            var mc = Set(Seq(1, 10), Seq(1, 20), Seq(1, 30), Seq(2, 10), Seq(2, 20), Seq(2, 30));
            
            Assert.True(mb == mc);
        }
                
        [Fact]
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Seq(Set<int>(), Set(1, 2, 3));
            var mb = ma.Sequence();
            var mc = Set<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq(Set<int>(), Set<int>());
            var mb = ma.Sequence();
            var mc = Set<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
