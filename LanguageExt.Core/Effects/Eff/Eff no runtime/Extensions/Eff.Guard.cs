using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffGuardExtensions
{
    /// <summary>
    /// Natural transformation to `Eff`
    /// </summary>
    public static Eff<Unit> ToEff(this Guard<Error, Unit> guard) =>
        guard.Flag
            ? Pure(unit)
            : Fail(guard.OnFalse());

    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<B> Bind<B>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<B>> f) =>
        guard.Flag
            ? f(default).As()
            : Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<C> SelectMany<B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<B>> bind, 
        Func<Unit, B, C> project) =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : Fail(guard.OnFalse());    
}
