using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
///  A data type that is used to represent “unexpected or expected” items in
/// 'ParseError'. It is parametrised over the token type `T`.
/// </summary>
/// <typeparam name="T">Token type</typeparam>
public abstract record ErrorItem<T> : K<ErrorItem, T>
{
    public ErrorItem<U> Map<U>(Func<T, U> f) =>
        this.Kind().Map(f).As();
    
    public ErrorItem<U> Select<U>(Func<T, U> f) =>
        this.Kind().Map(f).As();
    
    /// <summary>
    /// Non-empty stream of tokens
    /// </summary>
    /// <param name="Tokens">Tokens</param>
    /// <typeparam name="T">Token type</typeparam>
    public record Tokens(Seq<T> Items) : ErrorItem<T>;

    /// <summary>
    /// Label (should not be empty)
    /// </summary>
    /// <param name="Value">Label value</param>
    /// <typeparam name="T">Token type</typeparam>
    public record Label(string Value) : ErrorItem<T>;

    /// <summary>
    /// End of input
    /// </summary>
    /// <typeparam name="T">Token type</typeparam>
    public record EndfOfInput : ErrorItem<T>;
}
