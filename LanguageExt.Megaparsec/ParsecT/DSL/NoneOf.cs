using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTNoneOf<E, S, T, M, EqT>(S tokens) : ParsecT<E, S, T, M, T>
    where S : TokenStream<S, T>
    where M : Monad<M>
    where EqT : Eq<T>
{
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, T, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, T, B> eok,
        EmptyErr<E, S, T, M, B> eerr)
    {
        var ts = S.ChunkToTokens(tokens);
        if (S.Take1(s.Input, out var head, out var tail))
        {
            foreach (var t in ts)
            {
                if(EqT.Equals(head, t))
                {
                    return eerr(unexpect(s.Offset, ErrorItem.Token(head), ts), s);
                }
            }
            return cok(head, s with { Input = tail, Offset = s.Offset + 1 }, Hints<T>.Empty);
        }
        else
        {
            return eerr(unexpect(s.Offset, ErrorItem.EndOfInput<T>(), ts), s);
        }
        
        ParseError<T, E> unexpect(int pos1, ErrorItem<T> u, ReadOnlySpan<T> tokens) =>
            ParseError.Trivial<T, E>(pos1, u, ErrorItem.Tokens(tokens));
    }
}
