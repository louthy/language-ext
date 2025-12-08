using System;

namespace LanguageExt.Traits;

/// <summary>
/// Functor trait
/// </summary>
/// <remarks>
/// `Map` is used to apply a function of type `Func〈A, B〉` to a value of type `K〈F, A〉`
/// where `F` is a functor, to produce a value of type `K〈F, B〉`.
///
/// Note that for any type with more than one parameter (e.g., `Either`), only the
/// last type parameter can be modified with `Map` (e.g. `R` in `Either〈L, R〉`).
/// 
/// Some types two generic parameters or more have a `Bifunctor` instance that allows both
/// the last and the penultimate parameters to be mapped over.
/// </remarks>
/// <typeparam name="F">Self referring type</typeparam>
public interface Functor<F>  
    where F : Functor<F>
{
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="f">Mapping function</param>
    /// <param name="ma">Functor to map</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> ma);

    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="f">Mapping function</param>
    /// <param name="ma">Functor to map</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static virtual K<F, B> Map<A, B>(Func<A, B> f, Memo<F, A> ma) =>
        ma.Value.Map(f);
    
    /// <summary>
    /// Functor map operation with a constant value
    /// </summary>
    /// <param name="constantValue">Constant value used to override each bound value in the structure</param>
    /// <param name="ma">Functor to map</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static virtual K<F, A> ConstMap<A, B>(A constantValue, K<F, B> ma) =>
        ma.Map(_ => constantValue);
}
