using Xunit;

namespace LanguageExt.Tests.ScheduleTest;

public static class PositiveDurationTests
{
    [Fact]
    public static void EqualsTest() =>
        Assert.True(new Duration(2) == new Duration(2));

    [Fact]
    public static void NotEqualsTest() =>
        Assert.True(new Duration(2) != new Duration(4));

    [Fact]
    public static void GreaterThanTest() =>
        Assert.True(new Duration(4) > new Duration(2));

    [Fact]
    public static void GreaterThanEqualToTest() =>
        Assert.True(new Duration(4) >= new Duration(4));

    [Fact]
    public static void GreaterThanEqualToTest2() =>
        Assert.False(new Duration(2) >= new Duration(4));

    [Fact]
    public static void LessThanTest() =>
        Assert.True(new Duration(2) < new Duration(4));

    [Fact]
    public static void GreaterLessThanOrEqualToTest() =>
        Assert.True(new Duration(2) <= new Duration(2));

    [Fact]
    public static void GreaterLessThanOrEqualToTest2() =>
        Assert.False(new Duration(5) <= new Duration(2));
}
