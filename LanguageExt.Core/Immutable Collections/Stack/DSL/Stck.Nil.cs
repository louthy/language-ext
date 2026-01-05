namespace LanguageExt;

public abstract partial record Stck<A>
{
    /// <summary>
    /// Terminating/empty stack
    /// </summary>
    public sealed record Nil : Stck<A>;

    /// <summary>
    /// Value on top of the stack that has a reference to the rest of the stack
    /// </summary>
    /// <param name="Value">Value on the top of the stack</param>
    /// <param name="Rest">The rest of the stack</param>
    public sealed record Top(A Value, Stck<A> Rest) : Stck<A>;
}
