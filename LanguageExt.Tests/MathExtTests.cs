#nullable enable

using FluentAssertions;
using Xunit;

namespace LanguageExt.Tests;

public static class MathExtTests
{
    [Theory(DisplayName = "NextAfter will return a double that is greater than zero in the direction of positive infinity")]
    [InlineData(0.0d, 5E-324)]
    [InlineData(1.0d, 1.0000000000000002)]
    public static void Case1(double value, double result) =>
        MathExt.NextAfter(value, double.MaxValue).Should().BeGreaterThan(0).And.Be(result);

    [Theory(DisplayName = "NextAfter will return a double that is less than zero in the direction of negative infinity")]
    [InlineData(0.0d, -5E-324)]
    [InlineData(1.0d, 0.9999999999999999)]
    public static void Case2(double value, double result) =>
        MathExt.NextAfter(value, double.MinValue).Should().Be(result);

    [Fact(DisplayName = "NextAfter double will return a its starting point when called twice in both directions")]
    public static void Case3() =>
        MathExt.NextAfter(MathExt.NextAfter(0.0d, double.MaxValue), double.MinValue).Should().Be(0.0d);

    [Theory(DisplayName = "NextAfter double edge cases are handled gracefully")]
    [InlineData(double.NaN, double.NaN, double.NaN)]
    [InlineData(0.0d, double.NaN, double.NaN)]
    [InlineData(double.NaN, 0.0d, double.NaN)]
    [InlineData(0.0d, 0.0d, 0.0d)]
    [InlineData(double.Epsilon, double.Epsilon, double.Epsilon)]
    [InlineData(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity)]
    public static void Case4(double value, double next, double result) =>
        MathExt.NextAfter(value, next).Should().Be(result);

    [Theory(DisplayName = "NextAfter will return a float that is greater than zero in the direction of positive infinity")]    
    [InlineData(0.0f, 1E-45F)]
    [InlineData(1.0f, 1.0000001F)]
    public static void Case5(float value, float result) =>
        MathExt.NextAfter(value, float.MaxValue).Should().BeGreaterThan(0).And.Be(result);

    [Theory(DisplayName = "NextAfter will return a float that is less than zero in the direction of negative infinity")]
    [InlineData(0.0f, -1E-45F)]
    [InlineData(1.0f, 0.99999994F)]
    public static void Case6(float value, float result) =>
        MathExt.NextAfter(value, float.MinValue).Should().Be(result);

    [Fact(DisplayName = "NextAfter float will return a its starting point when called twice in both directions")]
    public static void Case7() =>
        MathExt.NextAfter(MathExt.NextAfter(0.0f, float.MaxValue), float.MinValue).Should().Be(0.0f);

    [Theory(DisplayName = "NextAfter float edge cases are handled gracefully")]
    [InlineData(float.NaN, float.NaN, float.NaN)]
    [InlineData(0.0f, float.NaN, float.NaN)]
    [InlineData(float.NaN, 0.0f, float.NaN)]
    [InlineData(0.0f, 0.0f, 0.0f)]
    [InlineData(float.Epsilon, float.Epsilon, float.Epsilon)]
    [InlineData(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity)]
    public static void Case8(float value, float next, float result) =>
        MathExt.NextAfter(value, next).Should().Be(result);
}
