using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTNotFollowedBy<E, S, T, M, A>(ParsecT<E, S, T, M, A> P) :
    ParsecT<E, S, T, M, Unit>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, Unit, B> _,
        ConsumedErr<E, S, T, M, B> __,
        EmptyOK<E, S, T, M, Unit, B> eok,
        EmptyErr<E, S, T, M, B> eerr)
    {
        return P.Run(s, eok1, cerr1, eok1, eerr1);

        // Updated ConsumedOK
        K<M, B> cok1(A x, State<S, T, E> s1, Hints<T> hs) =>
            eerr(unexpect(what()), s);

        // Updated EmptyErr
        K<M, B> cerr1(ParseError<T, E> err, State<S, T, E> s1) =>
            eok(unit, s, Hints<T>.Empty);

        // Updated EmptyOK
        K<M, B> eok1(A x, State<S, T, E> s1, Hints<T> hs) =>
            eerr(unexpect(what()), s);

        // Updated EmptyErr
        K<M, B> eerr1(ParseError<T, E> err, State<S, T, E> s1) =>
            eok(unit, s, Hints<T>.Empty);

        ParseError<T, E> unexpect(ErrorItem<T> u) =>
            ParseError.Trivial<T, E>(s.Offset, u);

        ErrorItem<T> what()
        {
            if (TokenStream.take1<S, T>(s.Input, out var t, out var _))
            {
                return ErrorItem.Token(t);
            }
            else
            {
                return ErrorItem.EndOfInput<T>();
            }
        }
    }
}
