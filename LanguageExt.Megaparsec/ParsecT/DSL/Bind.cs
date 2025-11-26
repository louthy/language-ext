using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTBind<E, S, T, M, A, B>(ParsecT<E, S, T, M, A> FA, Func<A, K<ParsecT<E, S, T, M>, B>> F) : 
    ParsecT<E, S, T, M, B>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, C> Run<C>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, B, C> cok,
        ConsumedErr<E, S, T, M, C> cerr,
        EmptyOK<E, S, T, M, B, C> eok,
        EmptyErr<E, S, T, M, C> eerr)
    {
        return FA.Run(s, mcok, cerr, meok, eerr);

        K<M, C> mcok(A x, State<S, T, E> s1, Hints<T> hs) =>
            F(x).As().Run(s1,
                          cok,
                          cerr,
                          (x2, s2, hs2) => cok(x2, s2, hs + hs2),
                          (e, s2) => e.WithHints<S, M, C>(hs, (ex, sx) => cerr(ex, sx))(s2));

        K<M, C> meok(A x, State<S, T, E> s1, Hints<T> hs) =>
            F(x).As().Run(s1,
                          cok,
                          cerr,
                          (x2, s2, hs2) => eok(x2, s2, hs + hs2),
                          (e, s2) => e.WithHints<S, M, C>(hs, (ex, sx) => eerr(ex, sx))(s2));
    }
}
