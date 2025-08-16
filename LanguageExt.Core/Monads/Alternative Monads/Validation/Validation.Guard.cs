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
            ? Validation<F, Unit>.Success(default)
            : Validation<F, Unit>.Fail(guard.OnFalse());
 
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public static Validation<F, B> Bind<F, B>(
        this Guard<F, Unit> guard,
        Func<Unit, Validation<F, B>> f) 
        where F : Monoid<F> =>
        guard.Flag
            ? f(default).As()
            : Validation<F, B>.Fail(guard.OnFalse());
       
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
            : Validation<F, C>.Fail(guard.OnFalse());    
}
