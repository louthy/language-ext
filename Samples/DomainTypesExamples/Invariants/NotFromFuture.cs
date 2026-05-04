using DomainTypesExamples.Capabilities;
using LanguageExt.Traits;

namespace DomainTypesExamples.Invariants;

public sealed class DateNotFromFuture<RT> : RuleM<DateNotFromFuture<RT>, Eff<RT>, DateOnly>
    where RT : HasTime<RT>
{
    public Eff<RT, DateTimeOffset> Now => 
        getNow<RT>();

    public static K<Eff<RT>, bool> Check(DateOnly v) =>
        getNow<RT>()
            .Map(n => n.ToDateOnly())
            .Map(d => d <= v);

}

public sealed class DateTimeOffsetNotFromFuture<RT> : RuleM<DateTimeOffsetNotFromFuture<RT>, Eff<RT>, DateTimeOffset>
    where RT : HasTime<RT>
{
    public Eff<RT, DateTimeOffset> Now =>
        getNow<RT>();

    public static K<Eff<RT>, bool> Check(DateTimeOffset v) =>
        getNow<RT>().Map(d => d <= v);

}
