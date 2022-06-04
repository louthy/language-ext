using FluentAssertions;
using Xunit;

namespace LanguageExt.Tests.ScheduleTest;

public static class PositiveDurationTests
{
    [Fact]
    public static void EqualsTest() =>
        (new PositiveDuration(2) == new PositiveDuration(2)).Should().BeTrue();

    [Fact]
    public static void NotEqualsTest() =>
        (new PositiveDuration(2) != new PositiveDuration(4)).Should().BeTrue();

    [Fact]
    public static void GreaterThanTest() =>
        (new PositiveDuration(4) > new PositiveDuration(2)).Should().BeTrue();

    [Fact]
    public static void GreaterThanEqualToTest() =>
        (new PositiveDuration(4) >= new PositiveDuration(4)).Should().BeTrue();

    [Fact]
    public static void GreaterThanEqualToTest2() =>
        (new PositiveDuration(2) >= new PositiveDuration(4)).Should().BeFalse();

    [Fact]
    public static void LessThanTest() =>
        (new PositiveDuration(2) < new PositiveDuration(4)).Should().BeTrue();

    [Fact]
    public static void GreaterLessThanOrEqualToTest() =>
        (new PositiveDuration(2) <= new PositiveDuration(2)).Should().BeTrue();
    
    [Fact]
    public static void GreaterLessThanOrEqualToTest2() =>
        (new PositiveDuration(5) <= new PositiveDuration(2)).Should().BeFalse();
}
