using System.Linq;
using Xunit;
using G = System.Collections.Generic;
using F = LanguageExt;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Collections
{
    using static Prelude;
    
    public class IEnumerableSet
    {
        G.IEnumerable<T> mkEnum<T>(params T[] ts)
        {
            foreach (var t in ts)
                yield return t;
        }
        
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            var ma = Enumerable.Empty<Set<int>>();
            var mb = ma.Sequence();
            var mc = F.Set<G.IEnumerable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void EnumSetCrossProduct()
        {
            var ma = mkEnum(Set(1, 2), Set(10, 20, 30));
            var mb = ma.Sequence();
            var mc = Set(mkEnum(1, 10), mkEnum(1, 20), mkEnum(1, 30), mkEnum(2, 10), mkEnum(2, 20), mkEnum(2, 30));
            
            Assert.True(toArray(mb.Map(toArray)) == toArray(mc.Map(toArray)));
            
        }
                
        [Fact]
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = mkEnum(Set<int>(), Set(1, 2, 3));
            var mb = ma.Sequence();
            var mc = F.Set<G.IEnumerable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = mkEnum(Set<int>(), Set<int>());
            var mb = ma.Sequence();
            var mc = F.Set<G.IEnumerable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
