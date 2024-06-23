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
}

/// <summary>
/// Non-empty stream of tokens
/// </summary>
/// <param name="Tokens">Tokens</param>
/// <typeparam name="T">Token type</typeparam>
public record EITokens<T>(Seq<T> Tokens) : ErrorItem<T>;

/// <summary>
/// Label (should not be empty)
/// </summary>
/// <param name="Value">Label value</param>
/// <typeparam name="T">Token type</typeparam>
public record EILabel<T>(string Value) : ErrorItem<T>;

/// <summary>
/// End of input
/// </summary>
/// <typeparam name="T">Token type</typeparam>
public record EIEndfOfInput<T> : ErrorItem<T>;

public static class ErrorItemExtensions
{
    public static ErrorItem<T> As<T>(this K<ErrorItem, T> ea) =>
        (ErrorItem<T>)ea;
}

public class ErrorItem : Functor<ErrorItem>
{
    public static K<ErrorItem, B> Map<A, B>(Func<A, B> f, K<ErrorItem, A> ma) => 
        ma switch
        {
            EITokens<A> (var tokens) => new EITokens<B>(tokens.Map(f)),
            EILabel<A> (var label)   => new EILabel<B>(label),
            EIEndfOfInput<A>         => new EIEndfOfInput<B>()
        };
}

