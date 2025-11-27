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
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr)
    {
        return fa.Run(s, cok, cerr, eok, @catch);

        K<M, B> @catch(ParseError<T, E> err, State<S, T, E> ms) =>
            predicate(err)
                ? fail(err).As().Run(ms, cok, cerr, eok, eerr)
                : eerr(err, ms);
    }
}
