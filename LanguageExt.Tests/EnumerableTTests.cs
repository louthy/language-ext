using System.Collections;
using System.Linq;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests;

public class EnumerableTTests
{
    [Fact]
    public void WrappedListTest()
    {
        var lst = List(List(1, 2, 3, 4, 5), List(1, 2, 3, 4, 5), List(1, 2, 3, 4, 5));

        var res  = lst.KindT<Lst, Lst, Lst<int>, int>().FoldT(0, (s, v) => s + v);
        var mlst = lst.KindT<Lst, Lst, Lst<int>, int>().MapT(x => x * 2);
        var mres = mlst.FoldT(0, (s, v) => s + v);

        Assert.True(res == 45, "Expected 45 got " + res);
        Assert.True(mres == 90, "Expected 90 got " + res);
        Assert.True(lst.KindT<Lst, Lst, Lst<int>, int>().CountT() == 15, "(lst) Expected 15 got " + lst.KindT<Lst, Lst, Lst<int>, int>().CountT());
        Assert.True(mlst.CountT() == 15, "(mlst) Expected 15 got " + mlst.CountT());

        lst = List<Lst<int>>();
        res = lst.KindT<Lst, Lst, Lst<int>, int>().FoldT(0, (s, v) => s + v);

        Assert.True(res                                           == 0, "Fold results, expected 0 got " + res);
        Assert.True(lst.KindT<Lst, Lst, Lst<int>, int>().CountT() == 0, "Empty count, expected 0 got "  + res);
    }

    [Fact]
    public void ChooseTest()
    {
        var input = List(
            Some(1),
            Some(2),
            Some(3),
            None,
            Some(4),
            None,
            Some(5));

        var actual = input.AsEnumerableM().Choose(x => x).ToList();

        var expected = List(1, 2, 3, 4, 5);

        var toString = fun((IEnumerable items) => string.Join(", ", items));

        Assert.True(EqEnumerable<int>.Equals(actual, expected), $"Expected {toString(expected)} but was {toString(actual)}");
    }
}
