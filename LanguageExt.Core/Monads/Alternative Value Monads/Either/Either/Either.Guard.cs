using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EitherGuardExtensions
    {
        public static Either<L, Unit> ToEither<L>(this Guard<L> ma) =>
            ma.Flag
                ? Right(unit)
                : Left(ma.OnFalse());
        
        public static Either<L, B> SelectMany<L, B>(this Guard<L> ma, Func<Unit, Either<L, B>> f) =>
            ma.Flag
                ? f(default)
                : Left(ma.OnFalse());

        public static Either<L, C> SelectMany<L, B, C>(this Guard<L> ma, Func<Unit, Either<L, B>> bind, Func<Unit, B, C> project) =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : Left(ma.OnFalse());

        public static Either<L, Unit> SelectMany<L, A>(this Either<L, A> ma, Func<A, Guard<L>> f) =>
            ma.Bind(a => f(a).ToEither());

        public static Either<L, C> SelectMany<L, A, C>(this Either<L, A> ma, Func<A, Guard<L>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEither().Map(_ => project(a, default)));
    }
}
