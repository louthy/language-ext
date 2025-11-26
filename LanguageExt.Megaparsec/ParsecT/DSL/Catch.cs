using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTCatch<E, S, T, M, A>(
    ParsecT<E, S, T, M, A> fa,
    Func<ParseError<T, E>, bool> predicate, 
    Func<ParseError<T, E>, K<ParsecT<E, S, T, M>, A>> fail) : 
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
        return fa.Run(s, cok, mcerr, eok, meerr);

        K<M, B> mcerr(ParseError<T, E> err, State<S, T, E> ms)
        {
            K<M, B> ncerr(ParseError<T, E> err1, State<S, T, E> s1) =>
                cerr(err1 + err, longestMatch(ms, s1));
          
            K<M, B> neok(A x, State<S, T, E> s1, Hints<T> hs) =>
                eok(x, s1, Hints.fromOffset(s1.Offset, err) + hs);
          
            K<M, B> neerr(ParseError<T, E> err1, State<S, T, E> s1) =>
                eerr(err1 + err, longestMatch(ms, s1));
          
            return predicate(err)
                       ? fail(err).As().Run(ms, cok, ncerr, neok, neerr)
                       : eerr(err, ms);
        }        

        K<M, B> meerr(ParseError<T, E> err, State<S, T, E> ms)
        {
          K<M, B> ncerr(ParseError<T, E> err1, State<S, T, E> s1) =>
              cerr(err1 + err, longestMatch(ms, s1));
          
          K<M, B> neok(A x, State<S, T, E> s1, Hints<T> hs) =>
              eok(x, s1, Hints.fromOffset(s1.Offset, err) + hs);
          
          K<M, B> neerr(ParseError<T, E> err1, State<S, T, E> s1) =>
              eerr(err1 + err, longestMatch(ms, s1));
          
          return predicate(err)
                    ? fail(err).As().Run(ms, cok, ncerr, neok, neerr)
                    : eerr(err, ms);
        }
        
        State<S, T, E> longestMatch(State<S, T, E> s1, State<S, T, E> s2) =>
            s1.Offset > s2.Offset ? s1 : s2;
    }
}
