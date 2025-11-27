using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTError<E, S, T, M, A>(ParseError<T, E> Error) : 
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        eerr(Error, s);
}
