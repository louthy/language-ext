using LanguageExt;
using System;

public static class EitherRightLeftExt
{
    public static Either<L, C> SelectMany<L, R, B, C>(this Either<L, R> ma, Func<R, EitherRight<B>> bind, Func<R, B, C> project) =>
        ma.Match(
            r => Prelude.Right<L, C>(project(r, bind(r).Value)),
            l => Prelude.Left<L, C>(l));

    public static Either<L, C> SelectMany<L, R, C>(this Either<L, R> ma, Func<R, EitherRight<L>> bind, Func<R, Unit, C> project) =>
        ma.Match(
            r => Prelude.Left<L, C>(bind(r).Value),
            l => Prelude.Left<L, C>(l));
}
