namespace DomainTypesExamples.Roots;

public sealed class WorkMoment :
    DomainType<WorkMoment, double>,
    DomainFactory<WorkMoment, Time>,
    AffineSpace<WorkMoment, Time, double>
{
    private readonly Time _offset;

    private WorkMoment(Time offset) =>
        _offset = offset;

    public Time ToBase() =>
        _offset;

    public double To() =>
        _offset.To();

    public static Fin<WorkMoment> From(Time repr) =>
        new WorkMoment(repr);

    public static WorkMoment operator +(WorkMoment moment, Time distance) =>
        new(moment._offset + distance);

    public static Time operator -(WorkMoment left, WorkMoment right) =>
        left._offset - right._offset;

    public bool Equals(WorkMoment? other) =>
        other is not null && _offset.Equals(other._offset);

    public override bool Equals(object? obj) =>
        obj is WorkMoment other && Equals(other);

    public override int GetHashCode() =>
        _offset.GetHashCode();

    public override string ToString()
    {
        var hours = Math.Floor(_offset.Hours);
        var minutes = _offset.Minutes - hours * 60;

        return $"{hours:0}h {minutes:00}m";
    }

    public static bool operator ==(WorkMoment? left, WorkMoment? right) =>
        Equals(left, right);

    public static bool operator !=(WorkMoment? left, WorkMoment? right) =>
        !(left == right);
}
