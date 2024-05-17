using System;

namespace LanguageExt;

public static class EitherGuardExtensions
{
    public static Either<L, Unit> SelectMany<L, A>(this Either<L, A> ma, Func<A, Guard<L, Unit>> f) =>
        ma.Bind(a => f(a).ToEither());

    public static Either<L, C> SelectMany<L, A, C>(this Either<L, A> ma, Func<A, Guard<L, Unit>> bind, Func<A, Unit, C> project) =>
        ma.Bind(a => bind(a).ToEither().Map(_ => project(a, default)));
}
