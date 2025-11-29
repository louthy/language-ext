using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTApply<E, S, T, M, A, B>(ParsecT<E, S, T, M, Func<A, B>> FF, ParsecT<E, S, T, M, A> FA) : 
    ParsecT<E, S, T, M, B>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, C> Run<C>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, B, C> cok,
        ConsumedErr<E, S, T, M, C> cerr,
        EmptyOK<E, S, T, M, B, C> eok,
        EmptyErr<E, S, T, M, C> eerr)
    {
        return FF.Run(s, mcok, cerr, meok, eerr);
        
        K<M, C> mcok(Func<A, B> f, State<S, T, E> s1, Hints<T> hs) =>
            FA.Run(s1,
                   (x2, s2, hs2) => cok(f(x2), s2, hs2),
                   cerr,
                   (x2, s2, hs2) => cok(f(x2), s2, hs + hs2),
                   (e, s2) => e.WithHints<S, M, C>(hs, (ex, sx) => cerr(ex, sx))(s2));

        K<M, C> meok(Func<A, B> f, State<S, T, E> s1, Hints<T> hs) =>
            FA.Run(s1,
                   (x2, s2, hs2) => cok(f(x2), s2, hs2),
                   cerr,
                   (x2, s2, hs2) => eok(f(x2), s2, hs + hs2),
                   (e, s2) => e.WithHints<S, M, C>(hs, (ex, sx) => eerr(ex, sx))(s2));
        
    }
}

record ParsecTApplyLazy<E, S, T, M, A, B>(ParsecT<E, S, T, M, Func<A, B>> FF, Memo<ParsecT<E, S, T, M>, A> FA) : 
    ParsecT<E, S, T, M, B>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, C> Run<C>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, B, C> cok,
        ConsumedErr<E, S, T, M, C> cerr,
        EmptyOK<E, S, T, M, B, C> eok,
        EmptyErr<E, S, T, M, C> eerr)
    {
        return FF.Run(s, mcok, cerr, meok, eerr);

        K<M, C> mcok(Func<A, B> f, State<S, T, E> s1, Hints<T> hs) =>
            FA.Value.As().Run(s1,
                              (x2, s2, hs2) => cok(f(x2), s2, hs2),
                              cerr,
                              (x2, s2, hs2) => cok(f(x2), s2, hs + hs2),
                              (e, s2) => e.WithHints<S, M, C>(hs, (ex, sx) => cerr(ex, sx))(s2));

        K<M, C> meok(Func<A, B> f, State<S, T, E> s1, Hints<T> hs) =>
            FA.Value.As().Run(s1,
                              (x2, s2, hs2) => cok(f(x2), s2, hs2),
                              cerr,
                              (x2, s2, hs2) => eok(f(x2), s2, hs + hs2),
                              (e, s2) => e.WithHints<S, M, C>(hs, (ex, sx) => eerr(ex, sx))(s2));
        
    }
        
}
