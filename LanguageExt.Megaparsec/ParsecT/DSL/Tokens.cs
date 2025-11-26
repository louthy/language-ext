using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTTokens<E, S, T, M>(Func<S, S, bool> Test, in S Chunk) : 
    ParsecT<E, S, T, M, S>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, S, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, S, B> eok,
        EmptyErr<E, S, T, M, B> eerr)
    {
        var len  = S.ChunkLength(Chunk);
        if (S.Take(len, s.Input, out var tts1, out var input1))
        {
            if (Test(Chunk, tts1))
            {
                var st = s with { Input = input1, Offset = s.Offset + len };
                return len <= 0
                           ? eok(tts1, st, Hints<T>.Empty)
                           : cok(tts1, st, Hints<T>.Empty);
            }
            else
            {
                return eerr(unexpect(s.Offset, ErrorItem.Tokens<S, T>(tts1)), s);
            }
        }
        else
        {
            return eerr(unexpect(s.Offset, ErrorItem.EndOfInput<T>()), s);
        }
        
        ParseError<T, E> unexpect(int pos1, ErrorItem<T> u) =>
            ParseError.Trivial<T, E>(pos1, u, ErrorItem.Tokens<S, T>(Chunk));
    }
}
