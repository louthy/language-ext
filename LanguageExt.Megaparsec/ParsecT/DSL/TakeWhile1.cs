using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTTakeWhile1<E, S, T, M>(in Option<string> Name, Func<T, bool> Test) : 
    ParsecT<E, S, T, M, S>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, S, B> cok,
        ConsumedErr<E, S, T, M, B> __,
        EmptyOK<E, S, T, M, S, B> ___,
        EmptyErr<E, S, T, M, B> eerr)
    {
        S.TakeWhile(Test, s.Input, out var ts, out var input1);
        var len = S.ChunkLength(ts);
        var el = Name.IsSome
                     ? Option.Some(ErrorItem.Label<T>((string)Name))
                     : Option.None;
        
        if (len <= 0)
        {
            if (S.Take1(s.Input, out var t, out _))
                return eerr(ParseError.Trivial<T, E>(s.Offset, ErrorItem.Token(t), el), s);
            else
                return eerr(ParseError.Trivial<T, E>(s.Offset, ErrorItem.EndOfInput<T>(), el), s);
        }
        else
        {
            var hs = Name.IsSome
                         ? Hints.singleton((ErrorItem<T>)el)
                         : Hints<T>.Empty;

            return cok(ts, s with { Input = input1, Offset = s.Offset + len }, hs);
        }
    }
}
