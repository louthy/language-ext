namespace LanguageExt.Megaparsec;

public static class Reply
{
    public static Reply<E, S, T, A> ConsumedOK<E, S, T, A>(A value, State<S, T, E> state, Hints<T> hints) =>
        new (state, true, Result.OK<T, E, A>(hints, value));
    
    public static Reply<E, S, T, A> EmptyOK<E, S, T, A>(A value, State<S, T, E> state, Hints<T> hints) =>
        new (state, false, Result.OK<T, E, A>(hints, value));    
    
    public static Reply<E, S, T, A> ConsumedError<E, S, T, A>(ParseError<T, E> error, State<S, T, E> state) =>
        new (state, true, Result.Error<T, E, A>(error));
    
    public static Reply<E, S, T, A> EmptyError<E, S, T, A>(ParseError<T, E> error, State<S, T, E> state) =>
        new (state, false, Result.Error<T, E, A>(error));
}
