using System;

namespace LanguageExt.Traits;

/// <summary>
/// Functor module
/// </summary>
public static class BifunctorExtensions
{
    /// <summary>
    /// Functor bimap.  Maps all contained values of `A` to values of `X` and every value of `B` to `Y`
    /// </summary>
    /// <param name="First">Mapping function</param>
    /// <param name="Second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, X, Y> BiMap<F, A, B, X, Y>(this K<F, A, B> fab, Func<A, X> First, Func<B, Y> Second) 
        where F : Bifunctor<F> =>
        F.BiMap(First, Second, fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, X, B> First<F, A, B, X>(this K<F, A, B> fab, Func<A, X> first) 
        where F : Bifunctor<F> =>
        F.First(first, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, A, Y> Second<F, A, B, Y>(this K<F, A, B> fab, Func<B, Y> second) 
        where F : Bifunctor<F> =>
        F.Second(second, fab);
}
