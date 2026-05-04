using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests.TraitTests.Domain;

public sealed class AffineSpaceDomainTypeTests
{
    [Fact]
    public void Operators()
    {
        var origin = MapCoordinateMeters.From(100m).SuccValue;

        var offset = MeterOffset.FromM(37.5m).Run().As().Value.SuccValue;

        var target = origin + offset;

        Assert.Equal(137.5m, target.To());
        Assert.Equal(37.5m, (target - origin).To());
        Assert.Equal(-37.5m, (origin - target).To());
    }
}

file sealed class MapCoordinateMeters :
    AffineSpace<MapCoordinateMeters, MeterOffset, decimal>,
    DomainTypeFactory<MapCoordinateMeters, decimal>,
    DomainTypeFactoryM<MapCoordinateMeters, Identity, decimal>
{
    private readonly decimal meters;

    private MapCoordinateMeters(decimal meters) =>
        this.meters = meters;

    public static MapCoordinateMeters Zero { get; } =
        new(0m);

    public decimal To() =>
        meters;

    public static Fin<MapCoordinateMeters> From(decimal repr) =>
        Fin.Succ(new MapCoordinateMeters(repr));

    public static FinT<Identity, MapCoordinateMeters> FromM(decimal repr) =>
        From(repr).Match(
            Succ: static value => FinT.Succ<Identity, MapCoordinateMeters>(value),
            Fail: static error => FinT.Fail<Identity, MapCoordinateMeters>(error));

    public static MapCoordinateMeters operator +(MapCoordinateMeters point, MeterOffset distance) =>
        new(point.meters + distance.To());

    public static MeterOffset operator -(MapCoordinateMeters left, MapCoordinateMeters right) =>
        MeterOffset.FromTrusted(left.meters - right.meters);

    public bool Equals(MapCoordinateMeters? other) =>
        other is not null && meters == other.meters;

    public override bool Equals(object? obj) =>
        obj is MapCoordinateMeters other && Equals(other);

    public override int GetHashCode() =>
        meters.GetHashCode();

    public static bool operator ==(MapCoordinateMeters? left, MapCoordinateMeters? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(MapCoordinateMeters? left, MapCoordinateMeters? right) =>
        !(left == right);
}

file sealed class MeterOffset :
    Magnitude<MeterOffset, decimal>,
    DomainTypeFactory<MeterOffset, decimal>,
    DomainTypeFactoryM<MeterOffset, Identity, decimal>
{
    private readonly decimal meters;

    private MeterOffset(decimal meters) =>
        this.meters = meters;

    public decimal To() =>
        meters;

    public static MeterOffset FromTrusted(decimal meters) =>
        new(meters);

    public static Fin<MeterOffset> From(decimal repr) =>
        Fin.Succ(new MeterOffset(repr));

    public static FinT<Identity, MeterOffset> FromM(decimal repr) =>
        From(repr).Match(
            Succ: static value => FinT.Succ<Identity, MeterOffset>(value),
            Fail: static error => FinT.Fail<Identity, MeterOffset>(error));

    public static MeterOffset AdditiveIdentity { get; } =
        new(0m);

    public static MeterOffset Origin =>
        AdditiveIdentity;

    public static MeterOffset operator +(MeterOffset left, MeterOffset right) =>
        new(left.meters + right.meters);

    public static MeterOffset operator -(MeterOffset left, MeterOffset right) =>
        new(left.meters - right.meters);

    public static MeterOffset operator -(MeterOffset value) =>
        new(-value.meters);

    public static MeterOffset operator *(MeterOffset value, decimal scalar) =>
        new(value.meters * scalar);

    public static MeterOffset operator /(MeterOffset value, decimal scalar) =>
        new(value.meters / scalar);

    public int CompareTo(MeterOffset? other) =>
        other is null ? 1 : meters.CompareTo(other.meters);

    public static bool operator <(MeterOffset left, MeterOffset right) =>
        left.CompareTo(right) < 0;

    public static bool operator >(MeterOffset left, MeterOffset right) =>
        left.CompareTo(right) > 0;

    public static bool operator <=(MeterOffset left, MeterOffset right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >=(MeterOffset left, MeterOffset right) =>
        left.CompareTo(right) >= 0;

    public bool Equals(MeterOffset? other) =>
        other is not null && meters == other.meters;

    public override bool Equals(object? obj) =>
        obj is MeterOffset other && Equals(other);

    public override int GetHashCode() =>
        meters.GetHashCode();

    public static bool operator ==(MeterOffset? left, MeterOffset? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(MeterOffset? left, MeterOffset? right) =>
        !(left == right);
}
