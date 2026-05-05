namespace DomainTypesExamples.Invariants;

public sealed class WorkDurationWithinDay :
    Rule<WorkDurationWithinDay, Time>
{
    public static bool Check(Time value) =>
        value > 0 * minute &&
        value <= 12 * hour;
}
