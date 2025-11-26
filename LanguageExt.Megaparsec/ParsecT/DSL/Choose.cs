using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTChoose<E, S, T, M, A>(ParsecT<E, S, T, M, A> m, ParsecT<E, S, T, M, A> n) : 
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
        return m.Run(s, cok, cerr, eok, merr);

        K<M, B> merr(ParseError<T, E> err, State<S, T, E> ms)
        {
          K<M, B> ncerr(ParseError<T, E> err1, State<S, T, E> s1) =>
              cerr(err1 + err, longestMatch(ms, s1));
          
          K<M, B> neok(A x, State<S, T, E> s1, Hints<T> hs) =>
              eok(x, s1, Hints.fromOffset(s1.Offset, err) + hs);
          
          K<M, B> neerr(ParseError<T, E> err1, State<S, T, E> s1) =>
              eerr(err1 + err, longestMatch(ms, s1));
          
          return n.Run(ms, cok, ncerr, neok, neerr);
        }
        
        State<S, T, E> longestMatch(State<S, T, E> s1, State<S, T, E> s2) =>
            s1.Offset > s2.Offset ? s1 : s2;
    }
}

record ParsecTChooseLazy<E, S, T, M, A>(ParsecT<E, S, T, M, A> m, Func<K<ParsecT<E, S, T, M>, A>> n) : 
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
        return m.Run(s, cok, cerr, eok, merr);

        K<M, B> merr(ParseError<T, E> err, State<S, T, E> ms)
        {
            K<M, B> ncerr(ParseError<T, E> err1, State<S, T, E> s1) =>
                cerr(err1 + err, longestMatch(ms, s1));
          
            K<M, B> neok(A x, State<S, T, E> s1, Hints<T> hs) =>
                eok(x, s1, Hints.fromOffset(s1.Offset, err) + hs);
          
            K<M, B> neerr(ParseError<T, E> err1, State<S, T, E> s1) =>
                eerr(err1 + err, longestMatch(ms, s1));
          
            return n().As().Run(ms, cok, ncerr, neok, neerr);
        }
        
        State<S, T, E> longestMatch(State<S, T, E> s1, State<S, T, E> s2) =>
            s1.Offset > s2.Offset ? s1 : s2;
    }
}
