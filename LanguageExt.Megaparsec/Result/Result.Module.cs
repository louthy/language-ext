namespace LanguageExt.Megaparsec;

public static class Result
{
    /// <summary>
    /// Parser succeeded (includes hints)
    /// </summary>
    /// <param name="hints">Hints</param>
    /// <param name="value">Parsed value</param>
    /// <typeparam name="T">Token type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Result</returns>
    public static Result<T, E, A> OK<T, E, A>(Hints<T> hints, A value) => 
        new Result<T, E, A>.OK(hints, value);
    
    /// <summary>
    /// Parse failed
    /// </summary>
    /// <param name="error">Error value</param>
    /// <typeparam name="T">Token type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Result</returns>
    public static Result<T, E, A> Error<T, E, A>(ParseError<T, E> error) => 
        new Result<T, E, A>.Error(error);
}
