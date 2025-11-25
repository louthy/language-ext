using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTLabel<E, S, T, M, A>(string Name, ParsecT<E, S, T, M, A> P) : 
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
        return P.Run(s, cok1, cerr, eok1, eerr1);

        // Updated ConsumedOK
        K<M, B> cok1(A x, State<S, T, E> s1, Hints<T> hs) =>
            string.IsNullOrEmpty(Name)
                ? cok(x, s1, hs.Refresh(None))
                : cok(x, s1, hs);

        // Updated EmptyOK
        K<M, B> eok1(A x, State<S, T, E> s1, Hints<T> hs) => 
            eok(x, s1, hs.Refresh(ErrorItem.Label<T>(Name)));

        // Updated EmptyErr
        K<M, B> eerr1(ParseError<T, E> err, State<S, T, E> s1) =>
            err switch
            {
                ParseError<T, E>.Trivial(var pos, var us, _) => 
                    eerr(ParseError.Trivial<T, E>(pos, us, ErrorItem.Label<T>(Name)), s1),

                _ => eerr(err, s1)
            };        
    }
    
}
