using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTLookAhead<E, S, T, M, A>(ParsecT<E, S, T, M, A> P) : 
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr)
    {
        return P.Run(s, eok1, cerr, eok1, eerr);
                                       
        K<M, B> eok1(A x, State<S, T, E> _, Hints<T> __) => 
            eok(x, s, Hints<T>.Empty) ;
    }
}
