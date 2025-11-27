using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTEmpty<E, S, T, M, A> : 
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public static readonly ParsecT<E, S, T, M, A> Default = new ParsecTEmpty<E, S, T, M, A>();
    
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        eerr(ParseError.Trivial<T, E>(s.Offset, None, []), s);
}
