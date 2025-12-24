using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTMTransLift<E, S, T, M, A>(K<M, A> ma) : 
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        ma >> (a => eok(a, s, Hints<T>.Empty));
}

record ParsecTMTransLiftIO<E, S, T, M, A>(IO<A> ma) : 
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        M.LiftIOMaybe(ma) >> (a => eok(a, s, Hints<T>.Empty));
}
