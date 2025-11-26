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

    /// <summary>
    /// Run the parser
    /// </summary>
    /// <param name="initialState">Starting state</param>
    /// <returns>Result of the parse</returns>
    public K<M, Reply<E, S, T, A>> Run(State<S, T, E> initialState)
    {
        return Run(initialState, cok, cerr, eok, eerr);
        
        K<M, Reply<E, S, T, A>> cok(A x, State<S, T, E> s1, Hints<T> hs) =>
            M.Pure(Reply.ConsumedOK(x, s1, hs));
        
        K<M, Reply<E, S, T, A>> cerr(ParseError<T, E> e, State<S, T, E> s1) =>
            M.Pure(Reply.ConsumedError<E, S, T, A>(e, s1));        
        
        K<M, Reply<E, S, T, A>> eok(A x, State<S, T, E> s1, Hints<T> hs) =>
            M.Pure(Reply.EmptyOK(x, s1, hs));
        
        K<M, Reply<E, S, T, A>> eerr(ParseError<T, E> e, State<S, T, E> s1) =>
            M.Pure(Reply.EmptyError<E, S, T, A>(e, s1));        
    }
}
