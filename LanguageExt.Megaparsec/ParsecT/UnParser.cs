using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// UnParser
/// </summary>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Stream type</typeparam>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public interface UnParser<E, S, T, M, A>
    where S : TokenStream<S, T>
{
    (Func<A, State<S, E>, Hints<S>, K<M, B>> ConsumedOK, 
     Func<ParseError<T, E>, State<S, E>, Hints<S>, K<M, B>> ConsumedError, 
     Func<A, State<S, E>, Hints<S>, K<M, B>> EmptyOK, 
     Func<ParseError<T, E>, State<S, E>, Hints<S>, K<M, B>> EmptyError) Invoke<B>(State<S, E> state);
}
