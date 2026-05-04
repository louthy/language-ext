using DomainTypesExamples.ValueObjects.Scalars;

namespace DomainTypesExamples.ValueObjects.Spaces;

public sealed class HourOnly
    : DomainTypeFactory<HourOnly, (HourValue Hours, MinuteValue Minutes)>,
      Magnitude<HourOnly, HourScalar>
{
    public const int MinutesPerHour = 60;

    private readonly int _totalMinutes;

    private HourOnly(int totalMinutes) =>
        _totalMinutes = totalMinutes;

    public (HourValue Hours, MinuteValue Minutes) To() =>
    (
        HourValue.From(_totalMinutes / MinutesPerHour).ThrowIfFail(),
        MinuteValue.From(_totalMinutes % MinutesPerHour).ThrowIfFail()
    );

    public int CompareTo(HourOnly? other) =>
        other is null
            ? 1
            : _totalMinutes.CompareTo(other._totalMinutes);

    public bool Equals(HourOnly? other) =>
        _totalMinutes.Equals(other?._totalMinutes);

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is HourOnly other && Equals(other);

    public override int GetHashCode() => _totalMinutes;

    public override string ToString()
    {
        var sign = _totalMinutes < 0 ? "-" : "";
        var abs = Math.Abs(_totalMinutes);

        return $"{sign}{abs / 60}h {abs % 60}m";
    }

    public static HourOnly AdditiveIdentity =>
        new(0);

    public static Fin<HourOnly> From((HourValue Hours, MinuteValue Minutes) repr) =>
        new HourOnly(repr.Hours.To() * MinutesPerHour + repr.Minutes.To());

    public static Fin<HourOnly> From((int Hours, int Minutes) repr) =>
        from hours in HourValue.From(repr.Hours)
        from minutes in MinuteValue.From(repr.Minutes)
        from result in From((hours, minutes))
        select result;

    public static HourOnly FromTotalMinutes(int totalMinutes) =>
        new(totalMinutes);

    public static HourOnly FromHours(int hours) =>
        new(hours * 60);

    public static bool operator >(HourOnly left, HourOnly right) =>
        left._totalMinutes > right._totalMinutes;

    public static bool operator >=(HourOnly left, HourOnly right) =>
        left._totalMinutes >= right._totalMinutes;

    public static bool operator <(HourOnly left, HourOnly right) =>
        left._totalMinutes < right._totalMinutes;

    public static bool operator <=(HourOnly left, HourOnly right) =>
        left._totalMinutes <= right._totalMinutes;

    public static HourOnly operator +(HourOnly left, HourOnly right) =>
        new(left._totalMinutes + right._totalMinutes);

    public static HourOnly operator -(HourOnly left, HourOnly right) =>
        new(left._totalMinutes - right._totalMinutes);

    public static HourOnly operator -(HourOnly value) =>
        new(-value._totalMinutes);

    public static HourOnly operator *(HourOnly left, HourScalar right) =>
        new(left._totalMinutes * right.TotalMinutes);

    public static HourOnly operator /(HourOnly left, HourScalar right) =>
        new(left._totalMinutes / right.TotalMinutes);

    public static bool operator ==(HourOnly? left, HourOnly? right) => 
        left?._totalMinutes == right?._totalMinutes;

    public static bool operator !=(HourOnly? left, HourOnly? right) => 
        !(left == right);
}
