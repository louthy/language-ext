using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class EitherTGuardExtensions
{
    /// <summary>
    /// Natural transformation to `EitherT`
    /// </summary>
    public static EitherT<L, M, Unit> ToEitherT<L, M>(this Guard<L, Unit> guard)
        where M : Monad<M> =>
        guard.Flag
            ? EitherT<L, M, Unit>.Right(default)
            : EitherT<L, M, Unit>.Left(guard.OnFalse());

    /// <summary>
    /// Monadic binding support for `EitherT`
    /// </summary>
    public static EitherT<L, M, B> Bind<L, M, B>(
        this Guard<L, Unit> guard,
        Func<Unit, EitherT<L, M, B>> f)
        where M : Monad<M> =>
        guard.Flag
            ? f(default).As()
            : EitherT<L, M, B>.Left(guard.OnFalse());

    /// <summary>
    /// Monadic binding support for `EitherT`
    /// </summary>
    public static EitherT<L, M, C> SelectMany<L, M, B, C>(
        this Guard<L, Unit> guard,
        Func<Unit, EitherT<L, M, B>> bind,
        Func<Unit, B, C> project)
        where M : Monad<M> =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : EitherT<L, M, C>.Left(guard.OnFalse());

}
