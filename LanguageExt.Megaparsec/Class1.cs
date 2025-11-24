using System.Numerics;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;


//public delegate K<M, B> ConsumedOK<E, S, in M, in A, B>(A value, State<S, E> state, Hints<S> hints);
//public delegate K<M, B> ConsumedError<E, S, in M, B>(ParseError<S, E> error, State<S, E> state);
//public delegate K<M, B> EmptyOK<E, S, in M, in A, B>(A value, State<S, E> state, Hints<S> hints);
//public delegate K<M, B> EmptyError<E, S, in M, B>(ParseError<S, E> error, State<S, E> state);

public interface ConsumedOK<E, S, in M, in A>
{
    K<M, B> Invoke<B>(A value, State<S, E> state, Hints<S> hints);
}

public interface ConsumedError<E, S, in M>
{
    K<M, B> Invoke<B>(ParseError<S, E> error, State<S, E> state);
}

public interface EmptyOK<E, S, in M, in A>
{
    K<M, B> Invoke<B>(A value, State<S, E> state, Hints<S> hints);
}

public interface EmptyError<E, S, in M>
{
    K<M, B> Invoke<B>(ParseError<S, E> error, State<S, E> state);
}



