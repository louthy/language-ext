using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTEOF<E, S, T, M> : 
    ParsecT<E, S, T, M, Unit>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public static readonly ParsecT<E, S, T, M, Unit> Default = new ParsecTEOF<E, S, T, M>();
    
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, Unit, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, Unit, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        S.Take1(s.Input, out var x, out _)
            ? eerr(ParseError.Trivial<T, E>(s.Offset, ErrorItem.Token(x), ErrorItem.EndOfInput<T>()), s)
            : eok(unit, s, Hints<T>.Empty);
    
}
