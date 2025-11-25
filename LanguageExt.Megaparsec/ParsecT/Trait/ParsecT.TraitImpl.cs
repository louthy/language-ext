using LanguageExt.Megaparsec.DSL;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public class ParsecT<E, S, T, M> : 
    MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Error<A>(
        ParseError<T, E> error) =>
        new ParsecTError<E, S, T, M, A>(error);

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Label<A>(
        string name,
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTLabel<E, S, T, M, A>(name, p.As());

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Try<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTTry<E, S, T, M, A>(p.As());

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.LookAhead<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTLookAhead<E, S, T, M, A>(p.As());

    static K<ParsecT<E, S, T, M>, Unit> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.NotFollowedBy<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTNotFollowedBy<E, S, T, M, A>(p.As());

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.WithRecovery<A>(
        Func<ParseError<T, E>, K<ParsecT<E, S, T, M>, A>> onError, 
        K<ParsecT<E, S, T, M>, A> p) => 
        new ParsecTWithRecovery<E, S, T, M, A>(onError, p.As());

    static K<ParsecT<E, S, T, M>, Unit> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.EOF => 
        ParsecTEOF<E, S, T, M>.Default;

    static K<ParsecT<E, S, T, M>, Either<ParseError<T, E>, A>> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Observing<A>(
        K<ParsecT<E, S, T, M>, A> p) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Token<A>(
        Func<T, Option<A>> test, 
        in Set<ErrorItem<T>> expected) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Tokens(
        Func<S, S, bool> test, 
        S chunk) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.TakeWhile(
        Option<string> name, 
        Func<T, bool> test) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.TakeWhile1(
        Option<string> name, 
        Func<T, bool> test) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Take(
        Option<string> name, 
        int n) => 
        new ParsecTTake<E, S, T, M>(name, n);

    static K<ParsecT<E, S, T, M>, State<S, T, E>> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.ParserState => 
        _parserState;

    static K<ParsecT<E, S, T, M>, Unit> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.UpdateParserState(
        Func<State<S, T, E>, State<S, T, E>> f) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Lift<A>(
        Func<State<S, T, E>, Reply<E, S, A>> f) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> Identifiable<ParsecT<E, S, T, M>, string>.Identify<A>(
        K<ParsecT<E, S, T, M>, A> fa, 
        Label<string> label) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, B> Functor<ParsecT<E, S, T, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ParsecT<E, S, T, M>, A> ma) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> Applicative<ParsecT<E, S, T, M>>.Pure<A>(
        A value) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, B> Applicative<ParsecT<E, S, T, M>>.Apply<A, B>(
        K<ParsecT<E, S, T, M>, Func<A, B>> mf, 
        K<ParsecT<E, S, T, M>, A> ma) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, B> Monad<ParsecT<E, S, T, M>>.Bind<A, B>(
        K<ParsecT<E, S, T, M>, A> ma, 
        Func<A, K<ParsecT<E, S, T, M>, B>> f) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> Choice<ParsecT<E, S, T, M>>.Choose<A>(
        K<ParsecT<E, S, T, M>, A> fa, 
        K<ParsecT<E, S, T, M>, A> fb) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> Choice<ParsecT<E, S, T, M>>.Choose<A>(
        K<ParsecT<E, S, T, M>, A> fa, 
        Func<K<ParsecT<E, S, T, M>, A>> fb) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> SemigroupK<ParsecT<E, S, T, M>>.Combine<A>(
        K<ParsecT<E, S, T, M>, A> lhs, 
        K<ParsecT<E, S, T, M>, A> rhs) => 
        throw new NotImplementedException();

    static K<ParsecT<E, S, T, M>, A> MonoidK<ParsecT<E, S, T, M>>.Empty<A>() => 
        throw new NotImplementedException();
}
