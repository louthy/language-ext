#nullable enable

using FluentAssertions;
using Xunit;

namespace LanguageExt.Tests.ScheduleTest;

public static class PositiveDurationTests
{
    [Fact]
    public static void EqualsTest() =>
        (new Duration(2) == new Duration(2)).Should().BeTrue();

    [Fact]
    public static void NotEqualsTest() =>
        (new Duration(2) != new Duration(4)).Should().BeTrue();

    [Fact]
    public static void GreaterThanTest() =>
        (new Duration(4) > new Duration(2)).Should().BeTrue();

    [Fact]
    public static void GreaterThanEqualToTest() =>
        (new Duration(4) >= new Duration(4)).Should().BeTrue();

    [Fact]
    public static void GreaterThanEqualToTest2() =>
        (new Duration(2) >= new Duration(4)).Should().BeFalse();

    [Fact]
    public static void LessThanTest() =>
        (new Duration(2) < new Duration(4)).Should().BeTrue();

    [Fact]
    public static void GreaterLessThanOrEqualToTest() =>
        (new Duration(2) <= new Duration(2)).Should().BeTrue();

    [Fact]
    public static void GreaterLessThanOrEqualToTest2() =>
        (new Duration(5) <= new Duration(2)).Should().BeFalse();
}
