using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class ValidationTGuardExtensions
{
    /// <summary>
    /// Natural transformation to `ValidationT`
    /// </summary>
    public static ValidationT<F, M, Unit> ToValidationT<F, M>(this Guard<F, Unit> guard)
        where M : Monad<M>
        where F : Monoid<F> =>
        guard.Flag
            ? ValidationT.Success<F, M, Unit>(default)
            : ValidationT.Fail<F, M, Unit>(guard.OnFalse());
    /// <summary>
    /// Natural transformation to `ValidationT`
    /// </summary>
    internal static ValidationT<F, M, Unit> ToValidationTI<F, M>(this Guard<F, Unit> guard)
        where M : Monad<M> =>
        guard.Flag
            ? ValidationT.SuccessI<F, M, Unit>(default)
            : ValidationT.FailI<F, M, Unit>(guard.OnFalse());
 
    /// <summary>
    /// Monadic binding support for `ValidationT`
    /// </summary>
    public static ValidationT<F, M, B> Bind<F, M, B>(
        this Guard<F, Unit> guard,
        Func<Unit, ValidationT<F, M, B>> f) 
        where M : Monad<M>
        where F : Monoid<F> =>
        guard.Flag
            ? f(default).As()
            : ValidationT.Fail<F, M, B>(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public static ValidationT<F, M, C> SelectMany<F, M, B, C>(
        this Guard<F, Unit> guard,
        Func<Unit, ValidationT<F, M, B>> bind, 
        Func<Unit, B, C> project) 
        where M : Monad<M>
        where F : Monoid<F> =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : ValidationT.Fail<F, M, C>(guard.OnFalse());
}
