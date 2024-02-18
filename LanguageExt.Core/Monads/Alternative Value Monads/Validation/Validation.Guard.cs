using System;
using LanguageExt.TypeClasses;

namespace LanguageExt;

public static class ValidationGuardExtensions
{
    public static Validation<F, C> SelectMany<F, A, C>(
        this Validation<F, A> ma,
        Func<A, Guard<F, Unit>> f,
        Func<A, Unit, C> project)
        where F : Monoid<F> =>
        ma.Bind(a => f(a).ToValidation().Map(b => project(a, b)));
}
