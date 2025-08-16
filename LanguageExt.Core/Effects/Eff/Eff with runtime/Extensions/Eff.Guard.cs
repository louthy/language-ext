using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffGuardExtensions
{
    /// <summary>
    /// Natural transformation to `Eff`
    /// </summary>
    public static Eff<RT, Unit> ToEff<RT>(this Guard<Error, Unit> guard) =>
        guard.Flag
            ? Pure(unit)
            : Fail(guard.OnFalse());

    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<RT, B> Bind<RT, B>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<RT, B>> f) =>
        guard.Flag
            ? f(default).As()
            : Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<RT, C> SelectMany<RT, B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<RT, B>> bind, 
        Func<Unit, B, C> project) =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : Fail(guard.OnFalse());
}
