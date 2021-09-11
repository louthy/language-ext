using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class AffGuards
    {
        public static Aff<B> SelectMany<B>(this Guard<Error> ma, Func<Unit, Aff<B>> f) =>
            ma.Flag
                ? f(default)
                : FailAff<B>(ma.OnFalse());

        public static Aff<RT, B> SelectMany<RT, B>(this Guard<Error> ma, Func<Unit, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Flag
                ? f(default)
                : FailAff<B>(ma.OnFalse());

        public static Aff<C> SelectMany<B, C>(this Guard<Error> ma, Func<Unit, Aff<B>> bind, Func<Unit, B, C> project) =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : FailAff<C>(ma.OnFalse());

        public static Aff<RT, C> SelectMany<RT, B, C>(this Guard<Error> ma, Func<Unit, Aff<RT, B>> bind, Func<Unit, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : FailAff<C>(ma.OnFalse());

        public static Aff<Unit> ToAff(this Guard<Error> ma) =>
            ma.Flag
                ? unitEff
                : FailAff<Unit>(ma.OnFalse());

        public static Aff<RT, Unit> ToAff<RT>(this Guard<Error> ma) where RT : struct, HasCancel<RT> =>
            ma.Flag
                ? unitEff
                : FailAff<Unit>(ma.OnFalse());        
        
        
        public static Aff<Unit> SelectMany<A>(this Aff<A> ma, Func<A, Guard<Error>> f) =>
            ma.Bind(a => f(a).ToAff());

        public static Aff<RT, Unit> SelectMany<RT, A>(this Aff<RT, A> ma, Func<A, Guard<Error>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).ToAff());

        public static Aff<C> SelectMany<A, C>(this Aff<A> ma, Func<A, Guard<Error>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToAff().Map(_ => project(a, default)));

        public static Aff<RT, C> SelectMany<RT, A, C>(this Aff<RT, A> ma, Func<A, Guard<Error>> bind, Func<A, Unit, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => bind(a).ToAff().Map(_ => project(a, default)));
    }
}
