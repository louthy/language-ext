using LanguageExt.Traits;

namespace DomainTypesExamples.Roots;

public sealed class NonEmptyWorkBlocks
    : RuleK<NonEmptyWorkBlocks, Seq, WorkBlock>
{
    public static bool Check(K<Seq, WorkBlock> value) =>
        value.As().Count > 0;
}

public sealed class DailyBlocksWithinTwelveHours
    : RuleK<DailyBlocksWithinTwelveHours, Seq, WorkBlock>
{
    public static bool Check(K<Seq, WorkBlock> value) =>
        value.As()
            .Fold(
                Time.AdditiveIdentity,
                (total, block) => total + block.Duration.ToBase())
        <= 12 * hour;
}
