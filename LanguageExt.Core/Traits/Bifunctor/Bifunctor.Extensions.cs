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
    public static K<F, Q, B> BiMap<F, P, A, Q, B>(this K<F, P, A> fab, Func<P, Q> First, Func<A, B> Second) 
        where F : Bifunctor<F> =>
        F.BiMap(First, Second, fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Q, A> First<F, P, A, Q>(this K<F, P, A> fab, Func<P, Q> first) 
        where F : Bifunctor<F> =>
        F.First(first, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, P, B> Second<F, P, A, B>(this K<F, P, A> fab, Func<A, B> second) 
        where F : Bifunctor<F> =>
        F.Second(second, fab);
}
