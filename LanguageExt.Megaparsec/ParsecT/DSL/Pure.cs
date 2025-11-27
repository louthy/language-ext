using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTPure<E, S, T, M, A>(A Value) : 
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, C> Run<C>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, C> cok,
        ConsumedErr<E, S, T, M, C> cerr,
        EmptyOK<E, S, T, M, A, C> eok,
        EmptyErr<E, S, T, M, C> eerr) =>
        eok(Value, s, Hints<T>.Empty);
}
