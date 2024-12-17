using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Bifunctor trait implementation for `Either〈L, R〉`
/// </summary>
public partial class Either :
    Bifunctor<Either>
{
    static K<Either, Q, B> Bifunctor<Either>.BiMap<P, A, Q, B>(
        Func<P, Q> first,
        Func<A, B> second,
        K<Either, P, A> fab) =>
        fab switch
        {
            Right<P, A>(P value) => Either<Q, B>.Left(first(value)),
            Left<P, A>(A value)  => Either<Q, B>.Right(second(value)),
            _                    => throw new NotSupportedException()
        };
}

/// <summary>
/// Monad trait implementation for `Either〈L, R〉`
/// </summary>
/// <typeparam name="L">Left type parameter</typeparam>
public class Either<L> : Either,
    Monad<Either<L>>, 
    Fallible<L, Either<L>>,
    Traversable<Either<L>>, 
    Choice<Either<L>>
{
    static K<Either<L>, B> Applicative<Either<L>>.Apply<A, B>(
        K<Either<L>, Func<A, B>> mf,
        K<Either<L>, A> ma) =>
        (mf, ma) switch
        {
            (Right<L, Func<A, B>> (var f), Right<L, A> (var a)) => Right(f(a)),
            (Left<L, Func<A, B>> (var e1), _)                   => Left<B>(e1),
            (_, Left<L, A> (var e2))                            => Left<B>(e2),
            _                                                   => throw new NotSupportedException()
        };

    static K<Either<L>, B> Applicative<Either<L>>.Action<A, B>(
        K<Either<L>, A> ma, 
        K<Either<L>, B> mb) => 
        mb;

    static K<Either<L>, B> Monad<Either<L>>.Bind<A, B>(K<Either<L>, A> ma, Func<A, K<Either<L>, B>> f) =>
        ma switch
        {
            Right<L, A> (var r) => f(r),
            Left<L, A> (var l)  => Left<B>(l),
            _                   => throw new NotSupportedException()
        };

    static K<Either<L>, A> Applicative<Either<L>>.Pure<A>(A value) => 
        Either<L, A>.Right(value);

    static K<F, K<Either<L>, B>> Traversable<Either<L>>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Either<L>, A> ta) =>
        ta switch
        {
            Right<L, A> (var r) => F.Map(Right, f(r)),
            Left<L, A> (var l)  => F.Pure(Left<B>(l)),
            _                   => throw new NotSupportedException()
        };

    static S Foldable<Either<L>>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Either<L>, A> ta) =>
        ta switch
        {
            Right<L, A> (var r) => predicate((state, r)) ? f(r)(state) : state,
            _                   => state
        };

    static S Foldable<Either<L>>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Either<L>, A> ta) =>
        ta switch
        {
            Right<L, A> (var r) => predicate((state, r)) ? f(state)(r) : state,
            _                   => state
        };

    static K<Either<L>, B> Functor<Either<L>>.Map<A, B>(Func<A, B> f, K<Either<L>, A> ma) =>
        ma switch
        {
            Right<L, A> (var r) => Right(f(r)),
            Left<L, A> (var l)  => Left<B>(l),
            _                   => throw new NotSupportedException()
        };

    static K<Either<L>, A> Right<A>(A value) =>
        Either<L, A>.Right(value);

    static K<Either<L>, A> Left<A>(L value) =>
        Either<L, A>.Left(value);

    static K<Either<L>, A> Choice<Either<L>>.Choose<A>(K<Either<L>, A> ma, K<Either<L>, A> mb) =>
        ma is Right<L, A> ? ma : mb;

    static K<Either<L>, A> Fallible<L, Either<L>>.Fail<A>(L error) => 
        Either<L, A>.Left(error);

    static K<Either<L>, A> Fallible<L, Either<L>>.Catch<A>(
        K<Either<L>, A> fa, Func<L, bool> Predicate,
        Func<L, K<Either<L>, A>> Fail) =>
        fa.As().BindLeft(l => Predicate(l) ? Fail(l).As() : Either<L, A>.Left(l));
}
