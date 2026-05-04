using DomainTypesExamples.ValueObjects;
using DomainTypesExamples.ValueObjects.Spaces;
using LanguageExt.Traits;

namespace DomainTypesExamples.Roots;

public abstract record WorkBlockKind(string Value)
    : DomainType<WorkBlockKind, string>,
        DomainSet<WorkBlockKind>
{
    public string To() => Value;

    public sealed record EffectiveKind() : WorkBlockKind("Effective");

    public sealed record LunchKind() : WorkBlockKind("Lunch");

    public sealed record RestKind() : WorkBlockKind("Rest");

    public static WorkBlockKind Effective { get; } =
        new EffectiveKind();

    public static WorkBlockKind Lunch { get; } =
        new LunchKind();

    public static WorkBlockKind Rest { get; } =
        new RestKind();

    public override string ToString() =>
        Value;
}

public sealed record WorkDay : DomainTypeFactory<WorkDay, (NonFutureDate At, Seq<WorkBlock> Blocks)>
{
    private readonly NonFutureDate _day;
    private readonly Seq<WorkBlock> _blocks;

    private WorkDay((NonFutureDate, Seq<WorkBlock>) values) =>
        (_day, _blocks) = values;

    public (NonFutureDate At, Seq<WorkBlock> Blocks) To() => (_day, _blocks);

    public HourOnly TrackedDuration =>
        HourOnly.FromTotalMinutes(
            _blocks.Fold(
                0,
                (total, block) => total + block.Duration.ToBase().TotalMinutesValue()));

    public HourOnly EffectiveDuration =>
        HourOnly.FromTotalMinutes(_blocks.Fold(
            0,
            (total, block) =>
                block is WorkBlock.Effective
                    ? total + block.Duration.ToBase().TotalMinutesValue()
                    : total));

    public HourOnly Overtime =>
        HourOnly.FromTotalMinutes(
            Math.Max(0, EffectiveDuration.TotalMinutesValue() - N480.Value));

    public static Fin<WorkDay> From((NonFutureDate At, Seq<WorkBlock> Blocks) repr) =>
        NonEmptyWorkBlocks
            .ValidateK(repr.Blocks,
                       Error.New($"{nameof(WorkDay)} must contain at least one work block.")) >>
        DailyBlocksWithinTwelveHours
            .ValidateK(repr.Blocks,
                       Error.New($"{nameof(WorkDay)} cannot exceed 12 tracked hours.")) *
        (WorkDay (_) => new WorkDay(repr));

    public Fin<WorkDay> AddBlock(WorkBlock block) =>
        From((_day, _blocks.Add(block)));
}
