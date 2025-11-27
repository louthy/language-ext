using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTMap<E, S, T, M, A, B>(ParsecT<E, S, T, M, A> P, Func<A, B> F) : 
    ParsecT<E, S, T, M, B>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, C> Run<C>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, B, C> cok,
        ConsumedErr<E, S, T, M, C> cerr,
        EmptyOK<E, S, T, M, B, C> eok,
        EmptyErr<E, S, T, M, C> eerr) =>
        P.Run(s, (x, s1, hs) => cok(F(x), s1, hs), cerr, (x, s1, hs) => eok(F(x), s1, hs), eerr);
}
