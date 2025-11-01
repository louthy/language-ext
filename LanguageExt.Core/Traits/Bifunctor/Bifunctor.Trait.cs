using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Bi-functor
/// </summary>
/// <typeparam name="F">Bi-functor self-type</typeparam>
public interface Bifunctor<F> 
    where F : Bifunctor<F>
{
    /// <summary>
    /// Functor bimap.  Maps all contained values of `L` to values of `M` and every value of `A` to `B`
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Functor structure</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <returns>Mapped functor</returns>
    public static abstract K<F, M, B> BiMap<L, A, M, B>(Func<L, M> first, Func<A, B> second, K<F, L, A> fab);

    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static virtual K<F, M, A> MapFirst<L, A, M>(Func<L, M> first, K<F, L, A> fab) =>
        F.BiMap(first, identity, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static virtual K<F, L, B> MapSecond<L, A, B>(Func<A, B> second, K<F, L, A> fab) =>
        F.BiMap(identity, second, fab);
}
