using LanguageExt.Megaparsec.DSL;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public class ParsecT<E, S, T, M> : 
    MonoidK<ParsecT<E, S, T, M>>, 
    MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>,
    MonadIO<ParsecT<E, S, T, M>>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Error<A>(
        ParseError<T, E> error) =>
        DSL<E, S, T, M>.error<A>(error);

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Label<A>(
        string name,
        K<ParsecT<E, S, T, M>, A> p) =>
        DSL<E, S, T, M>.label(name, p);

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Try<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        DSL<E, S, T, M>.@try(p);

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.LookAhead<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        DSL<E, S, T, M>.lookAhead(p);

    static K<ParsecT<E, S, T, M>, Unit> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.NotFollowedBy<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        DSL<E, S, T, M>.notFollowedBy(p);

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.WithRecovery<A>(
        Func<ParseError<T, E>, K<ParsecT<E, S, T, M>, A>> onError, 
        K<ParsecT<E, S, T, M>, A> p) => 
        DSL<E, S, T, M>.withRecovery(onError, p);

    static K<ParsecT<E, S, T, M>, Unit> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.EOF => 
        DSL<E, S, T, M>.eof;

    static K<ParsecT<E, S, T, M>, Either<ParseError<T, E>, A>> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Observing<A>(
        K<ParsecT<E, S, T, M>, A> p) => 
        DSL<E, S, T, M>.observing(p);

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Token<A>(
        Func<T, Option<A>> test, 
        in Set<ErrorItem<T>> expected) => 
        DSL<E, S, T, M>.token(test, expected);

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Tokens(
        Func<S, S, bool> test, 
        in S chunk) => 
        DSL<E, S, T, M>.tokens(test, chunk);

    static K<ParsecT<E, S, T, M>, T> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.OneOf<EqT>(
        S tokens)  =>
        DSL<E, S, T, M>.oneOf<EqT>(tokens);

    static K<ParsecT<E, S, T, M>, T> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.NoneOf<EqT>(
        S tokens) => 
        DSL<E, S, T, M>.noneOf<EqT>(tokens);

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.TakeWhile(
        Func<T, bool> test,
        in Option<string> name) => 
        DSL<E, S, T, M>.takeWhile(test, name);

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.TakeWhile1(
        Func<T, bool> test,
        in Option<string> name) => 
        DSL<E, S, T, M>.takeWhile1(test, name);

    static K<ParsecT<E, S, T, M>, S> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Take(
        int n,
        in Option<string> name) =>
        DSL<E, S, T, M>.take(name, n);

    static K<ParsecT<E, S, T, M>, A> MonadParsecT<ParsecT<E, S, T, M>, E, S, T, M>.Lift<A>(
        Func<State<S, T, E>, Reply<E, S, T, A>> f) =>
        DSL<E, S, T, M>.lift(f);

    static K<ParsecT<E, S, T, M>, A> Identifiable<ParsecT<E, S, T, M>, string>.Identify<A>(
        K<ParsecT<E, S, T, M>, A> fa,
        Label<string> label) =>
        DSL<E, S, T, M>.label(label.Value, fa);

    static K<ParsecT<E, S, T, M>, B> Functor<ParsecT<E, S, T, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ParsecT<E, S, T, M>, A> ma) => 
        DSL<E, S, T, M>.map(ma.As(), f);

    static K<ParsecT<E, S, T, M>, A> Applicative<ParsecT<E, S, T, M>>.Pure<A>(A value) => 
        DSL<E, S, T, M>.pure(value);

    static K<ParsecT<E, S, T, M>, B> Applicative<ParsecT<E, S, T, M>>.Apply<A, B>(
        K<ParsecT<E, S, T, M>, Func<A, B>> mf,
        K<ParsecT<E, S, T, M>, A> ma) =>
        DSL<E, S, T, M>.apply(mf.As(), ma.As());

    static K<ParsecT<E, S, T, M>, B> Applicative<ParsecT<E, S, T, M>>.Apply<A, B>(
        K<ParsecT<E, S, T, M>, Func<A, B>> mf,
        Memo<ParsecT<E, S, T, M>, A> ma) =>
        DSL<E, S, T, M>.apply(mf.As(), ma);

    static K<ParsecT<E, S, T, M>, B> Monad<ParsecT<E, S, T, M>>.Bind<A, B>(
        K<ParsecT<E, S, T, M>, A> ma, 
        Func<A, K<ParsecT<E, S, T, M>, B>> f) => 
        DSL<E, S, T, M>.bind(ma, f);

    static K<ParsecT<E, S, T, M>, A> Choice<ParsecT<E, S, T, M>>.Choose<A>(
        K<ParsecT<E, S, T, M>, A> fa, 
        K<ParsecT<E, S, T, M>, A> fb) => 
        DSL<E, S, T, M>.choose(fa, fb);

    static K<ParsecT<E, S, T, M>, A> Choice<ParsecT<E, S, T, M>>.Choose<A>(
        K<ParsecT<E, S, T, M>, A> fa,
        Memo<ParsecT<E, S, T, M>, A> fb) =>
        DSL<E, S, T, M>.choose(fa, fb);

    static K<ParsecT<E, S, T, M>, A> SemigroupK<ParsecT<E, S, T, M>>.Combine<A>(
        K<ParsecT<E, S, T, M>, A> fa,
        K<ParsecT<E, S, T, M>, A> fb) =>
        SemigroupInstance<A>.Instance switch
        {
            { IsSome: true, Case: SemigroupInstance<A> semi } =>
                ((A x, A y) => semi.Combine(x, y)) * fa * fb,

            _ => DSL<E, S, T, M>.choose(fa, fb)
        };

    static K<ParsecT<E, S, T, M>, A> MonoidK<ParsecT<E, S, T, M>>.Empty<A>() =>
        DSL<E, S, T, M>.empty<A>();

    static K<ParsecT<E, S, T, M>, A> Alternative<ParsecT<E, S, T, M>>.Empty<A>() =>
        DSL<E, S, T, M>.empty<A>();

    static K<ParsecT<E, S, T, M>, A> Fallible<ParseError<T, E>, ParsecT<E, S, T, M>>.Fail<A>(ParseError<T, E> error) =>
        DSL<E, S, T, M>.error<A>(error);

    static K<ParsecT<E, S, T, M>, A> Fallible<ParseError<T, E>, ParsecT<E, S, T, M>>.Catch<A>(
        K<ParsecT<E, S, T, M>, A> fa,
        Func<ParseError<T, E>, bool> predicate,
        Func<ParseError<T, E>, K<ParsecT<E, S, T, M>, A>> fail) =>
        DSL<E, S, T, M>.@catch(fa, predicate, fail);

    static K<ParsecT<E, S, T, M>, A> MonadIO<ParsecT<E, S, T, M>>.LiftIO<A>(IO<A> ma) =>
        DSL<E, S, T, M>.liftIO(ma);

    static K<ParsecT<E, S, T, M>, A> MonadT<ParsecT<E, S, T, M>, M>.Lift<A>(K<M, A> ma) =>
        DSL<E, S, T, M>.liftM(ma);

    static K<ParsecT<E, S, T, M>, A> Readable<ParsecT<E, S, T, M>, State<S, T, E>>.Asks<A>(
        Func<State<S, T, E>, A> f) => 
        DSL<E, S, T, M>.asks(f);

    static K<ParsecT<E, S, T, M>, A> Readable<ParsecT<E, S, T, M>, State<S, T, E>>.Local<A>(
        Func<State<S, T, E>, State<S, T, E>> f, 
        K<ParsecT<E, S, T, M>, A> ma) => 
        DSL<E, S, T, M>.local(f, ma.As());

    static K<ParsecT<E, S, T, M>, Unit> Stateful<ParsecT<E, S, T, M>, State<S, T, E>>.Put(
        State<S, T, E> value) => 
        DSL<E, S, T, M>.put(value);

    static K<ParsecT<E, S, T, M>, Unit> Stateful<ParsecT<E, S, T, M>, State<S, T, E>>.Modify(
        Func<State<S, T, E>, State<S, T, E>> f) => 
        DSL<E, S, T, M>.modify(f);
    
    static K<ParsecT<E, S, T, M>, A> Stateful<ParsecT<E, S, T, M>, State<S, T, E>>.Gets<A>(
        Func<State<S, T, E>, A> f) =>
        DSL<E, S, T, M>.asks(f);
}
