using Xunit;
using System.Linq;

namespace LanguageExt.Tests;

public class RangeTests
{
    [Fact]
    public void IntRangeAsc()
    {
        var r = Range.fromMinMax(2, 5);
        var x = r.ToLst();
        Assert.True(x == [2, 3, 4, 5]);
    }

    [Fact]
    public void IntRangeAscReversed()
    {
        var x = Range.fromMinMax(2, 5).Reverse().AsIterable();
        Assert.True(x == [5, 4, 3, 2]);
    }

    [Fact]
    public void IntRangeDesc()
    {
        var x = Range.fromMinMax(5, 2).ToLst();
        Assert.True(x == [5, 4, 3, 2]);
    }

    [Fact]
    public void IntCountAsc()
    {
        var x = Range.fromCount(2, 5, 2).ToLst();
        Assert.True(x == [2, 4, 6, 8, 10]);
    }

    [Fact]
    public void IntCountDesc()
    {
        var x = Range.fromCount(2, 5, -2).ToLst();
        Assert.True(x == [2, 0, -2, -4, -6]);
    }

    [Fact]
    public void CharCountAsc()
    {
        var x = Range.fromCount('a', (char)5).ToLst();
        Assert.True(x == ['a', 'b', 'c', 'd', 'e']);
    }

    [Fact]
    public void CharCountDesc()
    {
        var x = Range('e', 'a').ToLst();
        Assert.True(x == ['e', 'd', 'c', 'b', 'a']);
    }

    [Fact]
    public void CharRangeAsc()
    {
        var x = Range.fromMinMax('a', 'e').ToLst();
        Assert.True(x == ['a', 'b', 'c', 'd', 'e']);
    }

    [Fact]
    public void CharRangeDesc()
    {
        var x = Range.fromMinMax('e', 'a').ToLst();
        Assert.True(x == ['e', 'd', 'c', 'b', 'a']);
    }
}
