using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Functor in `A` and `B`
/// </summary>
/// <typeparam name="F">Functor</typeparam>
public interface Bifunctor<F> 
    where F : Bifunctor<F>
{
    /// <summary>
    /// Functor bimap.  Maps all contained values of `A` to values of `X` and every value of `B` to `Y`
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Functor structure</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <returns>Mapped functor</returns>
    public static abstract K<F, X, Y> BiMap<A, B, X, Y>(Func<A, X> first, Func<B, Y> second, K<F, A, B> fab);

    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static virtual K<F, X, B> First<A, B, X>(Func<A, X> first, K<F, A, B> fab) =>
        F.BiMap(first, identity, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static virtual K<F, A, Y> Second<A, B, Y>(Func<B, Y> second, K<F, A, B> fab) =>
        F.BiMap(identity, second, fab);
}
