using System.Linq;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Collections;

public class HashSetIEnumerable
{
    [Fact]
    public void EmptyEmptyIsEmptyEmpty()
    {
        HashSet<Iterable<int>> ma = Empty;

        var mb = ma.Traverse(mx => mx).As();

        var mc = Iterable.singleton(HashSet.empty<int>());

        Assert.True(mb.ToSeq() == mc.ToSeq());
    }

    [Fact]
    public void HashSetIEnumerableCrossProduct()
    {
        var ma = HashSet<Iterable<int>>([1, 2], [10, 20, 30]);

        var mb = ma.Traverse(mx => mx).As();


        var mc = new[]
            {
                HashSet(1, 10),
                HashSet(1, 20),
                HashSet(1, 30),
                HashSet(2, 10),
                HashSet(2, 20),
                HashSet(2, 30)
            };

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }

    [Fact]
    public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
    {
        var ma = HashSet<Iterable<int>>([], [1, 2, 3]);

        var mb = ma.Traverse(mx => mx).As();

        var mc = Enumerable.Empty<HashSet<int>>();

        Assert.True(mb.ToSeq() == mc.AsIterable().ToSeq());
    }

    [Fact]
    public void HashSetOfEmptiesIsEmpty()
    {
        var ma = HashSet<Iterable<int>>([], []);

        var mb = ma.Traverse(mx => mx).As();

        var mc = Iterable.empty<HashSet<int>>();

        Assert.True(mb.ToSeq() == mc.ToSeq());
    }
}
