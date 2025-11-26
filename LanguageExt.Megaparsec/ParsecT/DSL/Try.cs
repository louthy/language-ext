using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTTry<E, S, T, M, A>(ParsecT<E, S, T, M, A> P) : 
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
        return P.Run(s, cok, eerr1, eok, eerr1);

        // Resets the state to before the error
        K<M, B> eerr1(ParseError<T, E> err, State<S, T, E> _) =>
            eerr(err, s);        
    }
}
