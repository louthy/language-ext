using LanguageExt.Traits;
namespace LanguageExt.Megaparsec.DSL;

record ParsecTWithRecovery<E, S, T, M, A>(
    Func<ParseError<T, E>, K<ParsecT<E, S, T, M>, A>> OnError, 
    ParsecT<E, S, T, M, A> P) : 
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr)
    {
        return P.Run(s, cok, mcerr, eok, meerr);
        
        K<M, B> mcerr(ParseError<T, E> err, State<S, T, E> ms)
        {
            return OnError(err).As().Run(ms, rcok, rcerr, reok, reerr);
            K<M, B> rcok(A x, State<S, T, E> s1, Hints<T> _) => cok(x, s1, Hints<T>.Empty);
            K<M, B> rcerr(ParseError<T, E> _, State<S, T, E> __) => cerr(err, ms);
            K<M, B> reok(A x, State<S, T, E> s1, Hints<T> _) => eok(x, s1, Hints.fromOffset(s1.Offset, err));
            K<M, B> reerr(ParseError<T, E> _, State<S, T, E> s1) => cerr(err, ms);
        } 
        K<M, B> meerr(ParseError<T, E> err, State<S, T, E> ms)
        {
            return OnError(err).As().Run(ms, rcok, rcerr, reok, reerr);
            K<M, B> rcok(A x, State<S, T, E> s1, Hints<T> _) => cok(x, s1, Hints.fromOffset(s1.Offset, err));
            K<M, B> rcerr(ParseError<T, E> _, State<S, T, E> __) => eerr(err, ms);
            K<M, B> reok(A x, State<S, T, E> s1, Hints<T> _) => eok(x, s1, Hints.fromOffset(s1.Offset, err));
            K<M, B> reerr(ParseError<T, E> _, State<S, T, E> s1) => eerr(err, ms);
        } 
    }
}

