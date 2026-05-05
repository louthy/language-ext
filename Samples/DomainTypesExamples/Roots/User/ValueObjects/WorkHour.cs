using DomainTypesExamples.Invariants;

namespace DomainTypesExamples.Roots;

public sealed class WorkDuration :
    RefinedType<WorkDuration, Time, double>,
    DomainFactory<WorkDuration, Time>
{
    private readonly Time _value;

    private WorkDuration(Time value) =>
        _value = value;

    public Time ToBase() =>
        _value;

    public double To() =>
        _value.To();

    public double TotalHours =>
        _value.Hours;

    public double TotalMinutes =>
        _value.Minutes;

    public static Fin<WorkDuration> From(Time repr) =>
        WorkDurationWithinDay
            .Validate(
                repr,
                (_, value) => Error.New(
                    $"{nameof(WorkDuration)} must be greater than 0 minutes and lower or equal to 12 hours. Sent: {value.Minutes:0.##} minutes"))
            .Map(value => new WorkDuration(value));

    public static Fin<WorkDuration> FromHours(double value) =>
        From(value * hour);

    public static Fin<WorkDuration> FromMinutes(double value) =>
        From(value * minute);

    public bool Equals(WorkDuration? other) =>
        other is not null && _value.Equals(other._value);

    public override bool Equals(object? obj) =>
        obj is WorkDuration other && Equals(other);

    public override int GetHashCode() =>
        _value.GetHashCode();

    public override string ToString()
    {
        var hours = Math.Floor(_value.Hours);
        var minutes = _value.Minutes - hours * 60;

        return $"{hours:0}h {minutes:00}m";
    }

    public static bool operator ==(WorkDuration? left, WorkDuration? right) =>
        Equals(left, right);

    public static bool operator !=(WorkDuration? left, WorkDuration? right) =>
        !(left == right);
}
