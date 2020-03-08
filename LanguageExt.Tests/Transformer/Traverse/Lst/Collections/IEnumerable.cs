using System.Linq;
using Xunit;
using G = System.Collections.Generic;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    using static Prelude;
    
    public class IEnumerableLst
    {
        G.IEnumerable<T> mkEnum<T>(params T[] ts)
        {
            foreach (var t in ts)
                yield return t;
        }
        
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            var ma = Enumerable.Empty<Lst<int>>();

            var mb = ma.Sequence();

            var mc = Lst<G.IEnumerable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void EnumLstCrossProduct()
        {
            var ma = mkEnum(List(1, 2), List(10, 20, 30));

            var mb = ma.Sequence();

            var mc = List(mkEnum(1, 10), mkEnum(1, 20), mkEnum(1, 30), mkEnum(2, 10), mkEnum(2, 20), mkEnum(2, 30));
            
            Assert.True(mb.Map(toList) == mc.Map(toList));
            
        }
                
        [Fact]
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = mkEnum(List<int>(), List(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Lst<G.IEnumerable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = mkEnum(List<int>(), List<int>());

            var mb = ma.Sequence();

            var mc = Lst<G.IEnumerable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
