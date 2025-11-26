using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// All information available after parsing. This includes consumption of input, success (with the returned value) or
/// failure (with the parse error), and parser state at the end of parsing. 'Reply' can also be used to resume parsing.
/// </summary>
/// <param name="NewState">Updated state</param>
/// <param name="Consumed">Consumption flag</param>
/// <param name="Result">Parsed value</param>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Stream type</typeparam>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public readonly record struct Reply<E, S, T, A>(State<S, T, E> NewState, bool Consumed, Result<S, E, A> Result)
    : K<Reply<E, S, T>, A>
{
    public Reply(State<S, T, E> NewState, bool Consumed, K<Result<S, E>, A> Result) :
        this(NewState, Consumed, +Result)
    { }
}
