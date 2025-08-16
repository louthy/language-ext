using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class FinGuardExtensions
{
    /// <summary>
    /// Natural transformation to `Fin`
    /// </summary>
    public static Fin<Unit> ToFin(this Guard<Error, Unit> ma) =>
        ma.Flag
            ? FinSucc(unit)
            : FinFail<Unit>(ma.OnFalse());
    
    /// <summary>
    /// Monadic binding support for `Fin`
    /// </summary>
    public static Fin<B> Bind<B>(
        this Guard<Error, Unit> guard,
        Func<Unit, Fin<B>> f)  =>
        guard.Flag
            ? f(default).As()
            : Fin<B>.Fail(guard.OnFalse());
        
    /// <summary>
    /// Monadic binding support for `Fin`
    /// </summary>
    public static Fin<B> SelectMany<B>(this Guard<Error, Unit> ma, Func<Unit, Fin<B>> f) =>
        ma.Flag
            ? f(default)
            : FinFail<B>(ma.OnFalse());

    /// <summary>
    /// Monadic binding support for `Fin`
    /// </summary>
    public static Fin<C> SelectMany<B, C>(
        this Guard<Error, Unit> ma, 
        Func<Unit, Fin<B>> bind, 
        Func<Unit, B, C> project) =>
        ma.Flag
            ? bind(default).Map(b => project(default, b))
            : FinFail<C>(ma.OnFalse());
}
