using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTTakeWhile<E, S, T, M>(in Option<string> Name, Func<T, bool> Test) : 
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
        S.TakeWhile(Test, s.Input, out var ts, out var input1);
        var len = S.ChunkLength(ts);
        var hs = Name.IsSome
                     ? (string)Name switch
                       {
                           var n when string.IsNullOrEmpty(n) =>
                               Hints<T>.Empty,

                           var n => Hints.singleton(ErrorItem.Label<T>(n))
                       }
                     : Hints<T>.Empty;
        
        return len <= 0
                   ? eok(ts, s with { Input = input1, Offset = s.Offset + len }, hs)
                   : cok(ts, s with { Input = input1, Offset = s.Offset + len }, hs);
    }
}
