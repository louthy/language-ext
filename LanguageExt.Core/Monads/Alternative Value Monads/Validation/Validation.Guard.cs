using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationGuardExtensions
    {
        public static Validation<MonoidFail, FAIL, Unit> SelectMany<MonoidFail, FAIL, A>(
            this Validation<MonoidFail, FAIL, A> ma, 
            Func<A, Guard<FAIL, Unit>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
                ma.Bind(a => f(a).ToValidation<MonoidFail>());

        public static Validation<MonoidFail, FAIL, C> SelectMany<MonoidFail, FAIL, A, C>(
            this Validation<MonoidFail, FAIL, A> ma, 
            Func<A, Guard<FAIL, Unit>> bind, 
            Func<A, Unit, C> project)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
                ma.Bind(a => bind(a).ToValidation<MonoidFail>().Map(_ => project(a, default)));
    }
}
