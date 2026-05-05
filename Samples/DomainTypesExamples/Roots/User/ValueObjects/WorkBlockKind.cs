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

    public sealed override string ToString() =>
        Value;
}
