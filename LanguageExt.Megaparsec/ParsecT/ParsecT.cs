using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// Parser combinator transformer
/// </summary>
/// <param name="runParser"></param>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Token stream type</typeparam>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="M">Lifted monad</typeparam>
/// <typeparam name="A">Parse value type</typeparam>
public abstract record ParsecT<E, S, T, M, A> :
    K<ParsecT<E, S, T, M>, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    /// <summary>
    /// Run the parser
    /// </summary>
    /// <param name="initialState">Starting state</param>
    /// <param name="consumedOK">Called when a value has successfully parsed</param>
    /// <param name="consumedError">Called when a value has partially parsed before failure</param>
    /// <param name="emptyOK">Called when the parsing process was successful but didn't yield a value</param>
    /// <param name="emptyError">Called when the parsing process was unsuccessful and didn't parse anything at all</param>
    /// <returns>Result of the parse being passed to one of the provided functions</returns>
    public abstract K<M, B> Run<B>(
        State<S, T, E> initialState,
        ConsumedOK<E, S, T, M, A, B> consumedOK,
        ConsumedErr<E, S, T, M, B> consumedError,
        EmptyOK<E, S, T, M, A, B> emptyOK,
        EmptyErr<E, S, T, M, B> emptyError);
}
