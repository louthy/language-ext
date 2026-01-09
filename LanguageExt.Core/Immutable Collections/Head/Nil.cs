namespace LanguageExt;

/// <summary>
/// Non-existent value
/// </summary>
/// <typeparam name="A">Value type</typeparam>
public sealed record Nil<A> : Head<A>
{
    public static readonly Head<A> Default = new Nil<A>();
}
