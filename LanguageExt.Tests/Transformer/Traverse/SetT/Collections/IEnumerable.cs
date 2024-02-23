using Xunit;
using G = System.Collections.Generic;
using F = LanguageExt;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Collections;

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
        var ma = EnumerableM.empty<Set<int>>();
        var mb = ma.Traverse(mx => mx).As();

        var mc = F.Set<EnumerableM<int>>.Empty;
            
        Assert.True(mb == mc);
    }
        
    [Fact]
    public void EnumSetCrossProduct()
    {
        var ma = mkEnum(Set(1, 2), Set(10, 20, 30)).AsEnumerableM();
        var mb = ma.Traverse(mx => mx).As();

        var mc = Set(mkEnum(1, 10), mkEnum(1, 20), mkEnum(1, 30), mkEnum(2, 10), mkEnum(2, 20), mkEnum(2, 30));
            
        Assert.True(toArray(mb.Map(toArray)) == toArray(mc.Map(toArray)));
            
    }
                
    [Fact]
    public void SeqOfEmptiesAndNonEmptiesIsEmpty()
    {
        var ma = mkEnum(Set<int>(), Set(1, 2, 3)).AsEnumerableM();
        var mb = ma.Traverse(mx => mx).As();

        var mc = F.Set<EnumerableM<int>>.Empty;
            
        Assert.True(mb == mc);
    }
        
    [Fact]
    public void SeqOfEmptiesIsEmpty()
    {
        var ma = mkEnum(Set<int>(), Set<int>()).AsEnumerableM();
        var mb = ma.Traverse(mx => mx).As();

        var mc = F.Set<EnumerableM<int>>.Empty;
            
        Assert.True(mb == mc);
    }
}
