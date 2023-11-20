using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EitherUnsafeGuardExtensions
    {
        public static EitherUnsafe<L, Unit> SelectMany<L, A>(this EitherUnsafe<L, A> ma, Func<A, Guard<L, Unit>> f) =>
            ma.Bind(a => f(a).ToEitherUnsafe());

        public static EitherUnsafe<L, C> SelectMany<L, A, C>(this EitherUnsafe<L, A> ma, Func<A, Guard<L, Unit>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEitherUnsafe().Map(_ => project(a, default)));
    }
}
