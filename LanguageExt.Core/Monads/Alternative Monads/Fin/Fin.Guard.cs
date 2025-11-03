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
            ? Fin.Succ(unit)
            : Fin.Fail<Unit>(ma.OnFalse());
    
    /// <summary>
    /// Monadic binding support for `Fin`
    /// </summary>
    public static Fin<B> Bind<B>(
        this Guard<Error, Unit> guard,
        Func<Unit, Fin<B>> f)  =>
        guard.Flag
            ? f(default).As()
            : Fin.Fail<B>(guard.OnFalse());
        
    /// <summary>
    /// Monadic binding support for `Fin`
    /// </summary>
    public static Fin<B> SelectMany<B>(this Guard<Error, Unit> ma, Func<Unit, Fin<B>> f) =>
        ma.Flag
            ? f(default)
            : Fin.Fail<B>(ma.OnFalse());

    /// <summary>
    /// Monadic binding support for `Fin`
    /// </summary>
    public static Fin<C> SelectMany<B, C>(
        this Guard<Error, Unit> ma, 
        Func<Unit, Fin<B>> bind, 
        Func<Unit, B, C> project) =>
        ma.Flag
            ? bind(default).Map(b => project(default, b))
            : Fin.Fail<C>(ma.OnFalse());
}
