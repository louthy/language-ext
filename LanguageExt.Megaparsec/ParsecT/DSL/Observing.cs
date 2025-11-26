using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTObserving<E, S, T, M, A>(ParsecT<E, S, T, M, A> P) : 
    ParsecT<E, S, T, M, Either<ParseError<T, E>, A>>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, Either<ParseError<T, E>, A>, B> cok,
        ConsumedErr<E, S, T, M, B> _,
        EmptyOK<E, S, T, M, Either<ParseError<T, E>, A>, B> eok,
        EmptyErr<E, S, T, M, B> __)
    {
        return P.Run(s, cok1, cerr1, eok1, eerr1);

        K<M, B> cok1(A x, State<S, T, E> s1, Hints<T> hs) =>
            cok(Right(x), s1, hs);
        
        K<M, B> eok1(A x, State<S, T, E> s1, Hints<T> hs) =>
            eok(Right(x), s1, hs);
        
        K<M, B> cerr1(ParseError<T, E> err, State<S, T, E> s1) =>
            cok(Left(err), s1, Hints<T>.Empty);
        
        K<M, B> eerr1(ParseError<T, E> err, State<S, T, E> s1) =>
            eok(Left(err), s1, Hints.fromOffset(s1.Offset, err));
    }
}
