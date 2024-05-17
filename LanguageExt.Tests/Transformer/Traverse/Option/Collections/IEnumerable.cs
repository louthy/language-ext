using System.Linq;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections;

public class IEnumerableOption
{
    [Fact]
    public void EmptyIEnumerableIsSomeEmptyIEnumerable()
    {
        var ma = EnumerableM.empty<Option<int>>();

        var mb = ma.Traverse(mx => mx).As();


        var mr = mb.Map(b => ma.Count() == b.Count())
                   .IfNone(false);
            
        Assert.True(mr);
    }

    [Fact]
    public void IEnumerableSomesIsSomeIEnumerables()
    {
        var ma = new[] {Some(1), Some(2), Some(3)}.AsEnumerableM();

        var mb = ma.Traverse(mx => mx).As();

        Assert.True(mb.Map(b => EqEnumerable<int>.Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfNone(false));
    }

    [Fact]
    public void IEnumerableSomeAndNoneIsNone()
    {
        var ma = new[] {Some(1), Some(2), None}.AsEnumerableM();
        var mb = ma.Traverse(mx => mx).As();
        Assert.True(mb == None);
    }
}
