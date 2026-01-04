using Xunit;

namespace LanguageExt.Tests;

public class RangeTests
{
    [Fact]
    public void IntRangeAsc()
    {
        var x = Range.fromMinMax(2, 5).ToLst();
        Assert.True(x == List(2, 3, 4, 5));
    }

    [Fact]
    public void IntRangeAscReversed()
    {
        var x = Range.fromMinMax(2, 5).Reverse().ToLst();
        Assert.True(x == List(5, 4, 3, 2));
    }

    [Fact]
    public void IntRangeDesc()
    {
        var x = Range.fromMinMax(5, 2).ToLst();
        Assert.True(x == List(5, 4, 3, 2));
    }

    [Fact]
    public void IntRangeDescReversed()
    {
        var x = Range.fromMinMax(5, 2).Reverse().ToLst();
        Assert.True(x == List(2, 3, 4, 5));
    }

    [Fact]
    public void IntCountAsc()
    {
        var x = Range.fromCount(2, 5, 2).ToLst();
        Assert.True(x == List(2, 4, 6, 8, 10));
    }

    [Fact]
    public void IntCountAscReversed()
    {
        var x = Range.fromCount(2, 5, 2).Reverse().ToLst();
        Assert.True(x == List(10, 8, 6, 4, 2));
    }

    [Fact]
    public void IntCountDesc()
    {
        var x = Range.fromCount(2, 5, -2).ToLst();
        Assert.True(x == List(2, 0, -2, -4, -6));
    }

    [Fact]
    public void IntCountDescReversed()
    {
        var x = Range.fromCount(2, 5, -2).Reverse().ToLst();
        Assert.True(x == List(-6, -4, -2, 0, 2));
    }

    [Fact]
    public void CharCountAsc()
    {
        var x = Range.fromCount('a', (char)5).ToLst();
        Assert.True(x == List('a', 'b', 'c', 'd', 'e'));
    }

    [Fact]
    public void CharCountAscReversed()
    {
        var x = Range.fromCount('a', (char)5).Reverse().ToLst();
        Assert.True(x == List('e', 'd', 'c', 'b', 'a'));
    }

    [Fact]
    public void CharCountDesc()
    {
        var x = Range('e', 'a').ToLst();
        Assert.True(x == List('e', 'd', 'c', 'b', 'a'));
    }

    [Fact]
    public void CharCountDescReversed()
    {
        var x = Range('e', 'a').Reverse().ToLst();
        Assert.True(x == List('a', 'b', 'c', 'd', 'e'));
    }

    [Fact]
    public void CharRangeAsc()
    {
        var x = Range.fromMinMax('a', 'e').ToLst();
        Assert.True(x == List('a', 'b', 'c', 'd', 'e'));
    }

    [Fact]
    public void CharRangeAscReversed()
    {
        var x = Range.fromMinMax('a', 'e').Reverse().ToLst();
        Assert.True(x == List('e', 'd', 'c', 'b', 'a'));
    }

    [Fact]
    public void CharRangeDesc()
    {
        var x = Range.fromMinMax('e', 'a').ToLst();
        Assert.True(x == List('e', 'd', 'c', 'b', 'a'));
    }

    [Fact]
    public void CharRangeDescReversed()
    {
        var x = Range.fromMinMax('e', 'a').Reverse().ToLst();
        Assert.True(x == List('a', 'b', 'c', 'd', 'e'));
    }
}
