using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Coproduct trait implementation for `Either〈L, R〉`
/// </summary>
public partial class Either :
    Coproduct<Either>,
    Bimonad<Either>
{
    static K<Either, L1, R1> Bifunctor<Either>.BiMap<L, R, L1, R1>(
        Func<L, L1> first,
        Func<R, R1> second,
        K<Either, L, R> fab) =>
        fab switch
        {
            Either<L, R>.Right(var value) => new Either<L1, R1>.Right(second(value)),
            Either<L, R>.Left(var value)  => new Either<L1, R1>.Left(first(value)),
            _                             => throw new NotSupportedException()
        };

    static K<Either, A, B> CoproductCons<Either>.Left<A, B>(A value) =>
        new Either<A, B>.Left(value);

    static K<Either, A, B> CoproductCons<Either>.Right<A, B>(B value) =>
        new Either<A, B>.Right(value);

    public static C Match<A, B, C>(
        Func<A, C> Left,
        Func<B, C> Right,
        K<Either, A, B> fab) =>
        fab switch
        {
            Either<A, B>.Right(var value) => Right(value),
            Either<A, B>.Left(var value)  => Left(value),
            _                             => throw new NotSupportedException()
        };

    public static K<Either, Y, A> BindFirst<X, Y, A>(K<Either, X, A> ma, Func<X, K<Either, Y, A>> f) =>
        ma switch
        {
            Either<X, A>.Right (var a) => new Either<Y, A>.Right(a),
            Either<X, A>.Left (var l)  => f(l),
            _                          => throw new NotSupportedException()
        };

    public static K<Either, X, B> BindSecond<X, A, B>(K<Either, X, A> ma, Func<A, K<Either, X, B>> f) =>
        ma switch
        {
            Either<X, A>.Right (var a) => f(a),
            Either<X, A>.Left (var l)  => new Either<X, B>.Left(l),
            _                          => throw new NotSupportedException()
        };
}
