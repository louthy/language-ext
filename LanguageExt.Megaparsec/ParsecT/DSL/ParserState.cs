using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTParserState<E, S, T, M> : 
    ParsecT<E, S, T, M, State<S, T, E>>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public static readonly ParsecT<E, S, T, M, State<S, T, E>> Default = new ParsecTParserState<E, S, T, M>();
    
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, State<S, T, E>, B> _,
        ConsumedErr<E, S, T, M, B> __,
        EmptyOK<E, S, T, M, State<S, T, E>, B> eok,
        EmptyErr<E, S, T, M, B> ___) =>
        eok(s, s, Hints<T>.Empty);
    
}
