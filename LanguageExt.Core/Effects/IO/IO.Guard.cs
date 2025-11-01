using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class IOGuardExtensions
{
    /// <summary>
    /// Monadic binding support for `IO`
    /// </summary>
    public static IO<B> Bind<B>(
        this Guard<Error, Unit> guard,
        Func<Unit, IO<B>> f) =>
        guard.Flag 
            ? f(unit).As() 
            : Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `IO`
    /// </summary>
    public static IO<C> SelectMany<B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, IO<B>> bind, 
        Func<Unit, B, C> project) =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : Fail(guard.OnFalse());    
    
    /// <summary>
    /// Natural transformation to `IO`
    /// </summary>
    public static IO<Unit> ToIO(this Guard<Error, Unit> guard) =>
        IO.lift(() => guard.Flag
                          ? unit
                          : guard.OnFalse().Throw());    
}
