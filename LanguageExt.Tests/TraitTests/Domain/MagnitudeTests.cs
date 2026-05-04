using System.Diagnostics;
using LanguageExt.ClassInstances;
using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests.TraitTests.Domain;

public sealed class MagnitudeDomainTypeTests
{
    [Fact]
    public void Operators()
    {
        var morningDelta = TemperatureDeltaCelsius.From(4.50m).SuccValue;

        var afternoonDelta = TemperatureDeltaCelsius.FromM(11.25m).Run().As().Value.SuccValue;

        Assert.True(morningDelta < afternoonDelta);
        Assert.True(afternoonDelta > morningDelta);
        Assert.True(morningDelta <= afternoonDelta);
        Assert.True(afternoonDelta >= morningDelta);
    }
}

file sealed class TemperatureDeltaCelsius :
    Magnitude<TemperatureDeltaCelsius, decimal>,
    DomainTypeFactory<TemperatureDeltaCelsius, decimal>,
    DomainTypeFactoryM<TemperatureDeltaCelsius, Identity, decimal>
{
    private readonly decimal degrees;

    private TemperatureDeltaCelsius(decimal degrees) =>
        this.degrees = degrees;

    public decimal To() =>
        degrees;

    public static Fin<TemperatureDeltaCelsius> From(decimal repr) =>
        Fin.Succ(new TemperatureDeltaCelsius(repr));

    public static FinT<Identity, TemperatureDeltaCelsius> FromM(decimal repr) =>
        From(repr).Match(
            Succ: static value => FinT.Succ<Identity, TemperatureDeltaCelsius>(value),
            Fail: static error => FinT.Fail<Identity, TemperatureDeltaCelsius>(error));

    public static TemperatureDeltaCelsius AdditiveIdentity { get; } =
        new(0m);

    public static TemperatureDeltaCelsius Origin =>
        AdditiveIdentity;

    public static TemperatureDeltaCelsius operator +(TemperatureDeltaCelsius left, TemperatureDeltaCelsius right) =>
        new(left.degrees + right.degrees);

    public static TemperatureDeltaCelsius operator -(TemperatureDeltaCelsius left, TemperatureDeltaCelsius right) =>
        new(left.degrees - right.degrees);

    public static TemperatureDeltaCelsius operator -(TemperatureDeltaCelsius value) =>
        new(-value.degrees);

    public static TemperatureDeltaCelsius operator *(TemperatureDeltaCelsius value, decimal scalar) =>
        new(value.degrees * scalar);

    public static TemperatureDeltaCelsius operator /(TemperatureDeltaCelsius value, decimal scalar) =>
        new(value.degrees / scalar);

    public int CompareTo(TemperatureDeltaCelsius? other) =>
        other is null ? 1 : degrees.CompareTo(other.degrees);

    public static bool operator <(TemperatureDeltaCelsius left, TemperatureDeltaCelsius right) =>
        left.CompareTo(right) < 0;

    public static bool operator >(TemperatureDeltaCelsius left, TemperatureDeltaCelsius right) =>
        left.CompareTo(right) > 0;

    public static bool operator <=(TemperatureDeltaCelsius left, TemperatureDeltaCelsius right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >=(TemperatureDeltaCelsius left, TemperatureDeltaCelsius right) =>
        left.CompareTo(right) >= 0;

    public bool Equals(TemperatureDeltaCelsius? other) =>
        other is not null && degrees == other.degrees;

    public override bool Equals(object? obj) =>
        obj is TemperatureDeltaCelsius other && Equals(other);

    public override int GetHashCode() =>
        degrees.GetHashCode();

    public static bool operator ==(TemperatureDeltaCelsius? left, TemperatureDeltaCelsius? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(TemperatureDeltaCelsius? left, TemperatureDeltaCelsius? right) =>
        !(left == right);
}
