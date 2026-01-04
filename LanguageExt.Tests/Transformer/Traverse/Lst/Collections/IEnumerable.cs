using Xunit;
using G = System.Collections.Generic;
using L = LanguageExt;

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
            var ma = Iterable<Lst<int>>();

            var mb = ma.Traverse(mx => mx).As();

            var mc = L.Lst.singleton<Iterable<int>>(Empty);
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void EnumLstCrossProduct()
        {
            var ma = mkEnum(Lst(1, 2), Lst(10, 20, 30)).AsIterable();

            var mb = ma.Traverse(mx => mx).As();

            var mc = Lst(mkEnum(1, 10), 
                         mkEnum(1, 20), 
                         mkEnum(1, 30), 
                         mkEnum(2, 10), 
                         mkEnum(2, 20), 
                         mkEnum(2, 30));
            
            Assert.True(mb.Map(toLst) == mc.Map(toLst));
            
        }
                
        [Fact]
        public void SeqOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = mkEnum(Lst<int>(), Lst(1, 2, 3)).AsIterable();

            var mb = ma.Traverse(mx => mx).As();


            var mc = L.Lst<Iterable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SeqOfEmptiesIsEmpty()
        {
            var ma = mkEnum(Lst<int>(), Lst<int>()).AsIterable();

            var mb = ma.Traverse(mx => mx).As();


            var mc = L.Lst<Iterable<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
