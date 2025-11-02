using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Fin : 
    Monad<Fin>, 
    Fallible<Fin>,
    Traversable<Fin>, 
    Alternative<Fin>,
    Natural<Fin, Either<Error>>,
    Natural<Fin, Option>,
    Natural<Fin, Eff>,
    Natural<Fin, Try>,
    Natural<Fin, IO>
{
    static K<Fin, B> Monad<Fin>.Bind<A, B>(K<Fin, A> ma, Func<A, K<Fin, B>> f) =>
        ma switch
        {
            Succ<A> (var r) => f(r),
            Fail<A> (var l) => Fin<B>.Fail(l),
            _               => throw new NotSupportedException()
        };

    static K<Fin, B> Functor<Fin>.Map<A, B>(Func<A, B> f, K<Fin, A> ma) =>
        ma switch
        {
            Succ<A> (var r) => Fin<B>.Succ(f(r)),
            Fail<A> (var l) => Fin<B>.Fail(l),
            _               => throw new NotSupportedException()
        };

    static K<Fin, A> Applicative<Fin>.Pure<A>(A value) =>
        Fin<A>.Succ(value);

    static K<Fin, B> Applicative<Fin>.Apply<A, B>(K<Fin, Func<A, B>> mf, K<Fin, A> ma) =>
        (mf, ma) switch
        {
            (Succ<Func<A, B>> (var f), Succ<A> (var a))   => Fin<B>.Succ(f(a)),
            (Fail<Func<A, B>> (var e1), Fail<A> (var e2)) => Fin<B>.Fail(e1 + e2),
            (Fail<Func<A, B>> (var e1), _)                => Fin<B>.Fail(e1),
            (_, Fail<A> (var e2))                         => Fin<B>.Fail(e2),
            _                                             => throw new NotSupportedException()
        };

    static K<Fin, B> Applicative<Fin>.Action<A, B>(K<Fin, A> ma, K<Fin, B> mb) =>
        (ma, mb) switch
        {
            (Fail<A> (var e1), Fail<B> (var e2)) => Fin<B>.Fail(e1 + e2),
            (Fail<A> (var e1), _)                => Fin<B>.Fail(e1),
            var (_, r2)                          => r2
        };

    static S Foldable<Fin>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state,
        K<Fin, A> ta) =>
        ta switch
        {
            Succ<A> (var r) => predicate((state, r)) ? f(r)(state) : state,
            _               => state
        };

    static S Foldable<Fin>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state,
        K<Fin, A> ta) =>
        ta switch
        {
            Succ<A> (var r) => predicate((state, r)) ? f(state)(r) : state,
            _               => state
        };

    static K<F, K<Fin, B>> Traversable<Fin>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Fin, A> ta) =>
        ta switch
        {
            Succ<A> (var r) => F.Map(ConsSucc, f(r)),
            Fail<A> (var l) => F.Pure(ConsFail<B>(l)),
            _               => throw new NotSupportedException()
        };        

    static K<Fin, A> MonoidK<Fin>.Empty<A>() =>
        Fin<A>.Fail(Errors.None);

    static K<Fin, A> SemigroupK<Fin>.Combine<A>(K<Fin, A> ma, K<Fin, A> mb) =>
        ma switch
        {
            Succ<A> => ma,
            Fail<A> (var e1) => mb switch
                                {
                                    Succ<A>          => mb,
                                    Fail<A> (var e2) => Fin<A>.Fail(e1 + e2),
                                    _                => mb
                                },
            _                => ma
        };

    static K<Fin, A> Choice<Fin>.Choose<A>(K<Fin, A> ma, K<Fin, A> mb) =>
        ma switch
        {
            Succ<A> => ma,
            _       => mb
        };

    static K<Fin, A> Choice<Fin>.Choose<A>(K<Fin, A> ma, Func<K<Fin, A>> mb) => 
        ma switch
        {
            Succ<A> => ma,
            _       => mb()
        };

    static K<Fin, A> ConsSucc<A>(A value) =>
        Fin<A>.Succ(value);

    static K<Fin, A> ConsFail<A>(Error value) =>
        Fin<A>.Fail(value);

    static K<Fin, A> Fallible<Error, Fin>.Fail<A>(Error error) => 
        Fin<A>.Fail(error);

    static K<Fin, A> Fallible<Error, Fin>.Catch<A>(
        K<Fin, A> fa, Func<Error, bool> Predicate,
        Func<Error, K<Fin, A>> Fail) =>
        fa.As().BindFail(e => Predicate(e) ? Fail(e).As() : Fin<A>.Fail(e));

    static K<Either<Error>, A> Natural<Fin, Either<Error>>.Transform<A>(K<Fin, A> fa) =>
        fa switch
        {
            Succ<A> (var x) => Either.Right<Error, A>(x),
            Fail<A> (var e) => Either.Left<Error, A>(e),
            _               => throw new NotSupportedException()
        };

    static K<Option, A> Natural<Fin, Option>.Transform<A>(K<Fin, A> fa) =>
        fa switch
        {
            Succ<A> (var x) => Option<A>.Some(x),
            Fail<A>         => Option<A>.None,
            _               => throw new NotSupportedException()
        };

    static K<Try, A> Natural<Fin, Try>.Transform<A>(K<Fin, A> fa) => 
        fa switch
        {
            Succ<A> (var x) => Try<A>.Succ(x),
            Fail<A> (var e) => Try<A>.Fail(e),
            _               => throw new NotSupportedException()
        };

    static K<Eff, A> Natural<Fin, Eff>.Transform<A>(K<Fin, A> fa) => 
        fa switch
        {
            Succ<A> (var x) => Eff<A>.Pure(x),
            Fail<A> (var e) => Eff<A>.Fail(e),
            _               => throw new NotSupportedException()
        };    

    static K<IO, A> Natural<Fin, IO>.Transform<A>(K<Fin, A> fa) => 
        fa switch
        {
            Succ<A> (var x) => IO<A>.Pure(x),
            Fail<A> (var e) => IO<A>.Fail(e),
            _               => throw new NotSupportedException()
        };    
}
