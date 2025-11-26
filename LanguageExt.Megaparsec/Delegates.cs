using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public delegate K<M, B> ConsumedOK<E, S, T, in M, in A, B>(A value, State<S, T, E> state, Hints<T> hints);
public delegate K<M, B> ConsumedErr<E, S, T, in M, B>(ParseError<T, E> error, State<S, T, E> state);
public delegate K<M, B> EmptyOK<E, S, T, in M, in A, B>(A value, State<S, T, E> state, Hints<T> hints);
public delegate K<M, B> EmptyErr<E, S, T, in M, B>(ParseError<T, E> error, State<S, T, E> state);
