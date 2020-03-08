using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    public class SeqLst
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Seq<Lst<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Lst<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqLstCrossProduct()
        {
            var ma = Seq(List(1, 2), List(10, 20, 30));

            var mb = ma.Sequence();

            var mc = List(Seq(1, 10), Seq(1, 20), Seq(1, 30), Seq(2, 10), Seq(2, 20), Seq(2, 30));
            
            Assert.True(mb == mc);
        }
                
        [Fact]
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Seq(List<int>(), List(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Lst<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = Seq(List<int>(), List<int>());

            var mb = ma.Sequence();

            var mc = Lst<Seq<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
