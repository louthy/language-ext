using DomainTypesExamples.Invariants;
using DomainTypesExamples.Literals;
using DomainTypesExamples.ValueObjects.Scalars;
using DomainTypesExamples.ValueObjects.Spaces;

namespace DomainTypesExamples.ValueObjects;

public sealed class WorkDuration :
    RefinedTypeFactory<WorkDuration, HourOnly, (HourValue Hours, MinuteValue Minutes)>,
    Magnitude<WorkDuration, HourScalar>
{
    private readonly HourOnly _value;
    
    private WorkDuration(HourOnly value) =>
        _value = value;

    public HourOnly ToBase() => _value;

    public (HourValue Hours, MinuteValue Minutes) To() =>
        _value.To();

    public bool Equals(WorkDuration? other) =>
        _value.Equals(other?._value);

    public int CompareTo(WorkDuration? other) => 
        _value.CompareTo(other?._value);

    public override string ToString() =>
        _value.ToString();

    public static WorkDuration AdditiveIdentity { get; } = new(HourOnly.AdditiveIdentity);

    public static Fin<WorkDuration> From(HourOnly repr) =>
        Between<N0, N720, int>
            .Validate(
                repr.TotalMinutesValue(),
                (_, value) => Error.New(
                    $"{nameof(WorkDuration)} must be greater than 0 minutes and lower or equal to 12 hours. Sent: {value} minutes"))
            .Map(_ => new WorkDuration(repr));

    public static Fin<WorkDuration> From((int Hours, int Minutes) repr) =>
        HourOnly.From(repr).Bind(From);

    public static bool operator >(WorkDuration left, WorkDuration right) =>
        left._value > right._value;

    public static bool operator >=(WorkDuration left, WorkDuration right) =>
        left._value >= right._value;

    public static bool operator <(WorkDuration left, WorkDuration right) =>
        left._value < right._value;

    public static bool operator <=(WorkDuration left, WorkDuration right) =>
        left._value <= right._value;

    public static WorkDuration operator +(WorkDuration left, WorkDuration right) =>
        From(left._value + right._value).ThrowIfFail();

    public static WorkDuration operator -(WorkDuration left, WorkDuration right) =>
        From(left._value - right._value).ThrowIfFail();

    public static WorkDuration operator -(WorkDuration value) =>
        From(-value._value).ThrowIfFail();

    public static WorkDuration operator *(WorkDuration left, HourScalar right) =>
        From(left._value * right).ThrowIfFail();

    public static WorkDuration operator /(WorkDuration left, HourScalar right) =>
        From(left._value / right).ThrowIfFail();

    public static bool operator ==(WorkDuration? left, WorkDuration? right) =>
        left?._value == right?._value;

    public static bool operator !=(WorkDuration? left, WorkDuration? right) =>
        !(left == right);
}

public static class HourOnlyWorkHourExtensions
{
    public static int TotalMinutesValue(this HourOnly value)
    {
        var (hours, minutes) = value.To();

        return hours.To() * 60 + minutes.To();
    }
}
