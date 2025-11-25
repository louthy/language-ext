using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/*
public delegate K<M, A> ConsumedOK<E, S, T, in M, A>(A value, State<S, T, E> state, Hints<T> hints);
public delegate K<M, A> ConsumedErr<E, S, T, in M, A>(ParseError<T, E> error, State<S, T, E> state);
public delegate K<M, A> EmptyOK<E, S, T, in M, A>(A value, State<S, T, E> state, Hints<T> hints);
public delegate K<M, A> EmptyErr<E, S, T, in M, A>(ParseError<T, E> error, State<S, T, E> state);

/// <summary>
/// Parser combinator delegate
/// </summary>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Token stream type</typeparam>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="M">Monad to lift</typeparam>
/// <typeparam name="A">Parse value type</typeparam>
public delegate K<M, A> UnParser<E, S, T, M, A>(
   State<S, T, E> initialState,
   ConsumedOK<E, S, T, M, A> consumedOK,
   ConsumedErr<E, S, T, M, A> consumedError,
   EmptyOK<E, S, T, M, A> emptyOK,
   EmptyErr<E, S, T, M, A> emptyError);
   */

public delegate K<M, B> ConsumedOK<E, S, T, in M, A, B>(A value, State<S, T, E> state, Hints<T> hints);
public delegate K<M, B> ConsumedErr<E, S, T, in M, B>(ParseError<T, E> error, State<S, T, E> state);
public delegate K<M, B> EmptyOK<E, S, T, in M, A, B>(A value, State<S, T, E> state, Hints<T> hints);
public delegate K<M, B> EmptyErr<E, S, T, in M, B>(ParseError<T, E> error, State<S, T, E> state);

/// <summary>
/// Parser combinator delegate
/// </summary>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Token stream type</typeparam>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="M">Monad to lift</typeparam>
/// <typeparam name="A">Parse input value type</typeparam>
/// <typeparam name="B">Mapped output value type</typeparam>
/*
public delegate K<M, B> ParserF<E, S, T, M, A, B>(
    State<S, T, E> initialState,
    ConsumedOK<E, S, T, M, A, B> consumedOK,
    ConsumedErr<E, S, T, M, B> consumedError,
    EmptyOK<E, S, T, M, A, B> emptyOK,
    EmptyErr<E, S, T, M, B> emptyError);
    */

public delegate K<M, B> ParserF<E, S, T, M, A, B>(
    State<S, T, E> initialState,
    ConsumedOK<E, S, T, M, A, B> consumedOK,
    ConsumedErr<E, S, T, M, B> consumedError,
    EmptyOK<E, S, T, M, A, B> emptyOK,
    EmptyErr<E, S, T, M, B> emptyError);

public abstract record UnParser<E, S, T, M, A>
{
    public abstract K<M, B> Run<B>(
        State<S, T, E> initialState,
        ConsumedOK<E, S, T, M, A, B> consumedOK,
        ConsumedErr<E, S, T, M, B> consumedError,
        EmptyOK<E, S, T, M, A, B> emptyOK,
        EmptyErr<E, S, T, M, B> emptyError);
}

record UnParserError<E, S, T, M, A>(ParseError<T, E> error) : UnParser<E, S, T, M, A>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        eerr(error, s);
}
