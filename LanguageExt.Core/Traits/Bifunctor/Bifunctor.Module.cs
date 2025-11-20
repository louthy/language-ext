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
    public static K<F, Q, B> bimap<F, P, A, Q, B>(Func<P, Q> first, Func<A, B> second, K<F, P, A> fab) 
        where F : Bifunctor<F> =>
        F.BiMap(first, second, fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Q, A> first<F, P, A, Q>(Func<P, Q> first, K<F, P, A> fab) 
        where F : Bifunctor<F> =>
        F.MapFirst(first, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, P, B> second<F, P, A, B>(Func<A, B> second, K<F, P, A> fab) 
        where F : Bifunctor<F> =>
        F.MapSecond(second, fab);
}
