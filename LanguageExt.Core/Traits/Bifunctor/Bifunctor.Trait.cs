using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Functor in `P` and `A`
/// </summary>
/// <typeparam name="F">Functor</typeparam>
public interface Bifunctor<F> 
    where F : Bifunctor<F>
{
    /// <summary>
    /// Functor bimap.  Maps all contained values of `P` to values of `Q` and every value of `A` to `B`
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Functor structure</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <returns>Mapped functor</returns>
    public static abstract K<F, Q, B> BiMap<P, A, Q, B>(Func<P, Q> first, Func<A, B> second, K<F, P, A> fab);

    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static virtual K<F, Q, A> First<P, A, Q>(Func<P, Q> first, K<F, P, A> fab) =>
        F.BiMap(first, identity, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static virtual K<F, P, B> Second<P, A, B>(Func<A, B> second, K<F, P, A> fab) =>
        F.BiMap(identity, second, fab);
}
