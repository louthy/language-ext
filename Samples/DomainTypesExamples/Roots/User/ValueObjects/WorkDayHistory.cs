namespace DomainTypesExamples.Roots;

public sealed record WorkDayHistory
    : DomainType<WorkDayHistory, (int ForId, Seq<WorkDay> Histories)>
{
    private readonly UserId _forId;
    private readonly Seq<WorkDay> _histories;

    private WorkDayHistory(UserId ForId, Seq<WorkDay> Histories) =>
        (_forId, _histories) = (ForId, Histories);

    public (int ForId, Seq<WorkDay> Histories) To() =>
        (_forId.To(), _histories);
}
