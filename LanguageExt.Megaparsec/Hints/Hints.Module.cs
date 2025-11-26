namespace LanguageExt.Megaparsec;

public static class Hints 
{
    /// <summary>
    /// No hints
    /// </summary>
    public static Hints<T> empty<T>() => 
        Hints<T>.Empty;
    
    /// <summary>
    /// No hints
    /// </summary>
    public static Hints<T> singleton<T>(ErrorItem<T> value) => 
        new ([value]);
    
    /// <summary>
    /// Convert a `ParseError` record into 'Hints'.
    /// </summary>
    /// <param name="streamPos"></param>
    /// <param name="error"></param>
    /// <typeparam name="T">Token type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <returns>Hints</returns>
    public static Hints<T> fromOffset<T, E>(int streamPos, ParseError<T, E> error) =>
        error switch
        {
            ParseError<T, E>.Trivial(var errOffset, _, var ps) =>
                streamPos == errOffset
                    ? ps.IsEmpty
                          ? Hints<T>.Empty
                          : new Hints<T>(ps)
                    : Hints<T>.Empty,

            _ => Hints<T>.Empty
        };
}
