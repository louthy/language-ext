using System;

namespace LanguageExt;

public static class EitherGuardExtensions
{
    /// <summary>
    /// Natural transformation to `Either`
    /// </summary>
    public static Either<L, Unit> ToEither<L>(this Guard<L, Unit> guard) =>
        guard.Flag
            ? new Either<L, Unit>.Right(default)
            : new Either<L, Unit>.Left(guard.OnFalse());
 
    /// <summary>
    /// Monadic binding support for `Either`
    /// </summary>
    public static Either<L, B> Bind<L, B>(
        this Guard<L, Unit> guard,
        Func<Unit, Either<L, B>> f)  =>
        guard.Flag
            ? f(default).As()
            : new Either<L, B>.Left(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Either`
    /// </summary>
    public static Either<L, C> SelectMany<L, B, C>(
        this Guard<L, Unit> guard,
        Func<Unit, Either<L, B>> bind, 
        Func<Unit, B, C> project) =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : new Either<L, C>.Left(guard.OnFalse());

}
