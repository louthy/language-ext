using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class ValidationGuardExtensions
{
    /// <summary>
    /// Natural transformation to `Validation`
    /// </summary>
    public static Validation<F, Unit> ToValidation<F>(this Guard<F, Unit> guard) 
        where F : Monoid<F> =>
        guard.Flag
            ? Validation.Success<F, Unit>(default)
            : Validation.Fail<F, Unit>(guard.OnFalse());
 
    /// <summary>
    /// Natural transformation to `Validation`
    /// </summary>
    internal static Validation<F, Unit> ToValidationI<F>(this Guard<F, Unit> guard) =>
        guard.Flag
            ? Validation.SuccessI<F, Unit>(default)
            : Validation.FailI<F, Unit>(guard.OnFalse());
 
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public static Validation<F, B> Bind<F, B>(
        this Guard<F, Unit> guard,
        Func<Unit, Validation<F, B>> f) 
        where F : Monoid<F> =>
        guard.Flag
            ? f(default).As()
            : Validation.Fail<F, B>(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public static Validation<F, C> SelectMany<F, B, C>(
        this Guard<F, Unit> guard,
        Func<Unit, Validation<F, B>> bind, 
        Func<Unit, B, C> project) 
        where F : Monoid<F> =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : Validation.Fail<F, C>(guard.OnFalse());    
}
