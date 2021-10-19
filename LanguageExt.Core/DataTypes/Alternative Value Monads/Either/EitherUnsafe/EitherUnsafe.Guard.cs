using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EitherUnsafeGuardExtensions
    {
        public static EitherUnsafe<L, Unit> ToEitherUnsafe<L>(this Guard<L> ma) =>
            ma.Flag
                ? Right(unit)
                : Left(ma.OnFalse());
        
        public static EitherUnsafe<L, B> SelectMany<L, B>(this Guard<L> ma, Func<Unit, EitherUnsafe<L, B>> f) =>
            ma.Flag
                ? f(default)
                : Left(ma.OnFalse());

        public static EitherUnsafe<L, C> SelectMany<L, B, C>(this Guard<L> ma, Func<Unit, EitherUnsafe<L, B>> bind, Func<Unit, B, C> project) =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : Left(ma.OnFalse());

        public static EitherUnsafe<L, Unit> SelectMany<L, A>(this EitherUnsafe<L, A> ma, Func<A, Guard<L>> f) =>
            ma.Bind(a => f(a).ToEitherUnsafe());

        public static EitherUnsafe<L, C> SelectMany<L, A, C>(this EitherUnsafe<L, A> ma, Func<A, Guard<L>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEitherUnsafe().Map(_ => project(a, default)));
    }
}
