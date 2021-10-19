using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EitherAsyncGuardExtensions
    {
        public static EitherAsync<L, Unit> ToEitherAsync<L>(this Guard<L> ma) =>
            ma.Flag
                ? RightAsync<L, Unit>(unit)
                : LeftAsync<L, Unit>(ma.OnFalse());
        
        public static EitherAsync<L, B> SelectMany<L, B>(this Guard<L> ma, Func<Unit, EitherAsync<L, B>> f) =>
            ma.Flag
                ? f(default)
                : LeftAsync<L, B>(ma.OnFalse());

        public static EitherAsync<L, C> SelectMany<L, B, C>(this Guard<L> ma, Func<Unit, EitherAsync<L, B>> bind, Func<Unit, B, C> project) =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : LeftAsync<L, C>(ma.OnFalse());

        public static EitherAsync<L, Unit> SelectMany<L, A>(this EitherAsync<L, A> ma, Func<A, Guard<L>> f) =>
            ma.Bind(a => f(a).ToEitherAsync());

        public static EitherAsync<L, C> SelectMany<L, A, C>(this EitherAsync<L, A> ma, Func<A, Guard<L>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEitherAsync().Map(_ => project(a, default)));
    }
}
