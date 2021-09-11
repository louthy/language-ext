using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationGuardExtensions
    {
        public static Validation<MonoidFail, FAIL, Unit> ToValidation<MonoidFail, FAIL>(this Guard<FAIL> ma) 
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
                ma.Flag
                    ? Success<MonoidFail, FAIL, Unit>(unit)
                    : Fail<MonoidFail, FAIL, Unit>(ma.OnFalse());
        
        public static Validation<MonoidFail, FAIL, B> SelectMany<MonoidFail, FAIL, B>(
            this Guard<FAIL> ma, 
            Func<Unit, Validation<MonoidFail, FAIL, B>> f) 
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
                ma.Flag
                    ? f(default)
                    : Fail<MonoidFail, FAIL, B>(ma.OnFalse());

        public static Validation<MonoidFail, FAIL, C> SelectMany<MonoidFail, FAIL, B, C>(
            this Guard<FAIL> ma, 
            Func<Unit, Validation<MonoidFail, FAIL, B>> bind, 
            Func<Unit, B, C> project)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
                ma.Flag
                    ? bind(default).Map(b => project(default, b))
                    : Fail<MonoidFail, FAIL, C>(ma.OnFalse());

        public static Validation<MonoidFail, FAIL, Unit> SelectMany<MonoidFail, FAIL, A>(
            this Validation<MonoidFail, FAIL, A> ma, 
            Func<A, Guard<FAIL>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
                ma.Bind(a => f(a).ToValidation<MonoidFail, FAIL>());

        public static Validation<MonoidFail, FAIL, C> SelectMany<MonoidFail, FAIL, A, C>(
            this Validation<MonoidFail, FAIL, A> ma, 
            Func<A, Guard<FAIL>> bind, 
            Func<A, Unit, C> project)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
                ma.Bind(a => bind(a).ToValidation<MonoidFail, FAIL>().Map(_ => project(a, default)));
    }
}
