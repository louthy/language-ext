using DomainTypesExamples.ValueObjects;

namespace DomainTypesExamples.Roots;

public sealed record WorkDay :
    DomainTypeFactory<WorkDay, (NonFutureDate At, Seq<WorkBlock> Blocks)>
{
    private readonly NonFutureDate _day;
    private readonly Seq<WorkBlock> _blocks;

    public static readonly Time DefaultWorkDayDuration = 9 * hour;

    private WorkDay((NonFutureDate, Seq<WorkBlock>) values) =>
        (_day, _blocks) = values;

    public (NonFutureDate At, Seq<WorkBlock> Blocks) To() =>
        (_day, _blocks);

    public Time TrackedDuration =>
        _blocks.Fold(
            Time.AdditiveIdentity,
            (total, block) => total + block.Duration.ToBase());

    public Time EffectiveDuration =>
        _blocks.Fold(
            Time.AdditiveIdentity,
            (total, block) =>
                block is WorkBlock.Effective
                    ? total + block.Duration.ToBase()
                    : total);

    public Time Overtime
    {
        get
        {
            var overtime = TrackedDuration - DefaultWorkDayDuration;

            return overtime > Time.AdditiveIdentity
                ? overtime
                : Time.AdditiveIdentity;
        }
    }

    public static Fin<WorkDay> From((NonFutureDate At, Seq<WorkBlock> Blocks) repr) =>
        NonEmptyWorkBlocks
            .ValidateK(
                repr.Blocks,
                Error.New($"{nameof(WorkDay)} must contain at least one work block.")) >>
        DailyBlocksWithinTwelveHours
            .ValidateK(
                repr.Blocks,
                Error.New($"{nameof(WorkDay)} cannot exceed 12 tracked hours.")) *
        (WorkDay (_) => new WorkDay(repr));

    public Fin<WorkDay> AddBlock(WorkBlock block) =>
        From((_day, _blocks.Add(block)));
}
