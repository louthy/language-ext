using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTToken<E, S, T, M, A>(Func<T, Option<A>> Test, in Set<ErrorItem<T>> Expected) : 
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
        if (S.Take1(s.Input, out var c, out var cs))
        {
            switch (Test(c))
            {
                case { IsSome: true } o:
                    var value = (A)o;
                    return cok(value, s with { Offset = s.Offset + 1, Input = cs }, Hints<T>.Empty);
                
                default:
                    return eerr(ParseError.Trivial<T, E>(s.Offset, ErrorItem.Token(c), Expected), s);
            }
        }
        else
        {
            return eerr(ParseError.Trivial<T, E>(s.Offset, ErrorItem.EndOfInput<T>(), Expected), s);
        }
    }
}
