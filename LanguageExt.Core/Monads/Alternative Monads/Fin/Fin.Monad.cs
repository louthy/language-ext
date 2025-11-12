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
            Fin<A>.Succ (var r) => f(r),
            Fin<A>.Fail (var l) => Fail<B>(l),
            _                   => throw new NotSupportedException()
        };

    static K<Fin, B> Functor<Fin>.Map<A, B>(Func<A, B> f, K<Fin, A> ma) =>
        ma switch
        {
            Fin<A>.Succ (var r) => Succ(f(r)),
            Fin<A>.Fail (var l) => Fail<B>(l),
            _                      => throw new NotSupportedException()
        };

    static K<Fin, A> Applicative<Fin>.Pure<A>(A value) =>
        Succ(value);

    static K<Fin, B> Applicative<Fin>.Apply<A, B>(K<Fin, Func<A, B>> mf, K<Fin, A> ma) =>
        (mf, ma) switch
        {
            (Fin<Func<A, B>>.Succ (var f), Fin<A>.Succ (var a))   => Succ(f(a)),
            (Fin<Func<A, B>>.Fail (var e1), Fin<A>.Fail (var e2)) => Fail<B>(e1 + e2),
            (Fin<Func<A, B>>.Fail (var e1), _)                    => Fail<B>(e1),
            (_, Fin<A>.Fail (var e2))                             => Fail<B>(e2),
            _                                                     => throw new NotSupportedException()
        };

    static K<Fin, B> Applicative<Fin>.Action<A, B>(K<Fin, A> ma, K<Fin, B> mb) =>
        (ma, mb) switch
        {
            (Fin<A>.Fail (var e1), Fin<B>.Fail (var e2)) => Fail<B>(e1 + e2),
            (Fin<A>.Fail (var e1), _)                    => Fail<B>(e1),
            var (_, r2)                                  => r2
        };

    static S Foldable<Fin>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Fin, A> ta) =>
        ta switch
        {
            Fin<A>.Succ (var r) => predicate((state, r)) ? f(r)(state) : state,
            _                   => state
        };

    static S Foldable<Fin>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Fin, A> ta) =>
        ta switch
        {
            Fin<A>.Succ (var r) => predicate((state, r)) ? f(state)(r) : state,
            _                   => state
        };

    static K<F, K<Fin, B>> Traversable<Fin>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Fin, A> ta) =>
        ta switch
        {
            Fin<A>.Succ (var r) => F.Map(ConsSucc, f(r)),
            Fin<A>.Fail (var l) => F.Pure(ConsFail<B>(l)),
            _                   => throw new NotSupportedException()
        };        

    static K<Fin, A> MonoidK<Fin>.Empty<A>() =>
        Fail<A>(Errors.None);

    static K<Fin, A> SemigroupK<Fin>.Combine<A>(K<Fin, A> ma, K<Fin, A> mb) =>
        ma switch
        {
            Fin<A>.Succ => ma,
            Fin<A>.Fail (var e1) => mb switch
                                    {
                                        Fin<A>.Succ          => mb,
                                        Fin<A>.Fail (var e2) => Fail<A>(e1 + e2),
                                        _                    => mb
                                    },
            _ => ma
        };

    static K<Fin, A> Choice<Fin>.Choose<A>(K<Fin, A> ma, K<Fin, A> mb) =>
        ma switch
        {
            Fin<A>.Succ => ma,
            _           => mb
        };

    static K<Fin, A> Choice<Fin>.Choose<A>(K<Fin, A> ma, Func<K<Fin, A>> mb) =>
        ma switch
        {
            Fin<A>.Succ => ma,
            _           => mb()
        };

    static K<Fin, A> ConsSucc<A>(A value) =>
        Succ(value);

    static K<Fin, A> ConsFail<A>(Error value) =>
        Fail<A>(value);

    static K<Fin, A> Fallible<Error, Fin>.Fail<A>(Error error) => 
        Fail<A>(error);

    static K<Fin, A> Fallible<Error, Fin>.Catch<A>(
        K<Fin, A> fa, Func<Error, bool> Predicate,
        Func<Error, K<Fin, A>> Fail) =>
        fa.As().BindFail(e => Predicate(e) ? Fail(e).As() : Fail<A>(e));

    static K<Either<Error>, A> Natural<Fin, Either<Error>>.Transform<A>(K<Fin, A> fa) =>
        fa switch
        {
            Fin<A>.Succ (var x) => Either.Right<Error, A>(x),
            Fin<A>.Fail (var e) => Either.Left<Error, A>(e),
            _                   => throw new NotSupportedException()
        };

    static K<Option, A> Natural<Fin, Option>.Transform<A>(K<Fin, A> fa) =>
        fa switch
        {
            Fin<A>.Succ (var x) => Option.Some(x),
            Fin<A>.Fail         => Option<A>.None,
            _                   => throw new NotSupportedException()
        };

    static K<Try, A> Natural<Fin, Try>.Transform<A>(K<Fin, A> fa) =>
        fa switch
        {
            Fin<A>.Succ (var x) => Try.Succ(x),
            Fin<A>.Fail (var e) => Try.Fail<A>(e),
            _                   => throw new NotSupportedException()
        };

    static K<Eff, A> Natural<Fin, Eff>.Transform<A>(K<Fin, A> fa) =>
        fa switch
        {
            Fin<A>.Succ (var x) => Eff<A>.Pure(x),
            Fin<A>.Fail (var e) => Eff<A>.Fail(e),
            _                   => throw new NotSupportedException()
        };

    static K<IO, A> Natural<Fin, IO>.Transform<A>(K<Fin, A> fa) =>
        fa switch
        {
            Fin<A>.Succ (var x) => IO.pure(x),
            Fin<A>.Fail (var e) => IO.fail<A>(e),
            _                   => throw new NotSupportedException()
        };
}
