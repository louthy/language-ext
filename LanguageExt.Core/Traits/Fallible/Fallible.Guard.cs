using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class FallibleGuardExtensions
{
    /// <summary>
    /// Monadic binding support for `Fallible`
    /// </summary>
    public static K<F, B> Bind<F, B>(
        this Guard<Error, Unit> guard,
        Func<Unit, K<F, B>> f) 
        where F : Fallible<F> =>
        guard.Flag 
            ? f(unit) 
            : error<F, B>(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Fallible`
    /// </summary>
    public static K<F, C> SelectMany<F, B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, K<F, B>> bind, 
        Func<Unit, B, C> project) 
        where F : Fallible<F>, Functor<F> =>
        guard.Flag
            ? bind(default).Map(b => project(default, b))
            : error<F, C>(guard.OnFalse());    
    
    /// <summary>
    /// Monadic binding support for `Fallible`
    /// </summary>
    public static K<F, B> Bind<E, F, B>(
        this Guard<E, Unit> guard,
        Func<Unit, K<F, B>> f) 
        where F : Fallible<E, F> =>
        guard.Flag 
            ? f(unit) 
            : fail<E, F, B>(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Fallible`
    /// </summary>
    public static K<F, C> SelectMany<E, F, B, C>(
        this Guard<E, Unit> guard,
        Func<Unit, K<F, B>> bind, 
        Func<Unit, B, C> project) 
        where F : Fallible<E, F>, Functor<F> =>
        guard.Flag
            ? bind(default).Map(b => project(default, b))
            : fail<E, F, C>(guard.OnFalse());    
}
