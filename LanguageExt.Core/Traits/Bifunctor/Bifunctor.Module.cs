using System;

namespace LanguageExt.Traits;

/// <summary>
/// Functor module
/// </summary>
public static class Bifunctor
{
    /// <summary>
    /// Functor bimap.  Maps all contained values of `A` to values of `X` and every value of `B` to `Y`
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, X, Y> bimap<F, A, B, X, Y>(Func<A, X> first, Func<B, Y> second, K<F, A, B> fab) 
        where F : Bifunctor<F> =>
        F.BiMap(first, second, fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, X, B> first<F, A, B, X>(Func<A, X> first, K<F, A, B> fab) 
        where F : Bifunctor<F> =>
        F.First(first, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, A, Y> second<F, A, B, Y>(Func<B, Y> second, K<F, A, B> fab) 
        where F : Bifunctor<F> =>
        F.Second(second, fab);
}
