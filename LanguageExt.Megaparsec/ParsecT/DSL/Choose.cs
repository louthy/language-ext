using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTChoose<E, S, T, M, A>(ParsecT<E, S, T, M, A> MA, ParsecT<E, S, T, M, A> MB) : 
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
        return MA.Run(s, cok, cerr, eok, merr);
        K<M, B> merr(ParseError<T, E> e, State<S, T, E> s1) =>
            MB.Run(s1, cok, cerr, eok, eerr);
    }
}
