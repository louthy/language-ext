using LanguageExt.Traits;
namespace LanguageExt.Megaparsec;

/// <summary>
/// Parser result
/// </summary>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public abstract record Result<T, E, A> : K<Result<T, E>, A>
{
    /// <summary>
    /// Parser succeeded (includes hints)
    /// </summary>
    /// <param name="Hints">Hints</param>
    /// <param name="Value">Parsed value</param>
    public record OK(Hints<T> Hints, A Value) : Result<T, E, A>;
    
    /// <summary>
    /// Parse failed
    /// </summary>
    /// <param name="Value">Error value</param>
    public record Error(ParseError<T, E> Value) : Result<T, E, A>;
}
