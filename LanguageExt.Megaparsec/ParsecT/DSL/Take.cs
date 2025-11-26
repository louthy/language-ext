using LanguageExt.Traits;
using LanguageExt.UnsafeValueAccess;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTTake<E, S, T, M>(in Option<string> Name, int Amount) : 
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
        var n = Math.Max(0, Amount);
        if (S.Take(n, s.Input, out var ts, out var input1))
        {
            var len = S.ChunkLength(ts);
            if (len == n)
            {
                // Happy path
                return cok(ts, s with { Input = input1, Offset = s.Offset + len }, Hints<T>.Empty);
            }
            else
            {
                // Didn't read the desired number of tokens
                var el = (ErrorItem.Label<T>) * Name;
                var ps = el.IsNone ? [] : Set.singleton(el.ValueUnsafe()!);
                return eerr(ParseError.Trivial<T, E>(s.Offset + len, ErrorItem.EndOfInput<T>(), ps), s);
            }
        }
        else
        {
            // End of the input stream
            var el = (ErrorItem.Label<T>) * Name;
            var ps = el.IsNone ? [] : Set.singleton(el.ValueUnsafe()!);
            return eerr(ParseError.Trivial<T, E>(s.Offset, ErrorItem.EndOfInput<T>(), ps), s);
        }
    }
    
}
