using System;
using System.Collections.Generic;
using System.Text;
using DomainTypesExamples.ValueObjects;
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
                0,
                (total, block) => total + block.Duration.ToBase().TotalMinutesValue())
        <= N720.Value;
}
