using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<F, B> Map<F, A, B>(this Func<A, B> f, K<Validation<F>, A> ma) => 
        Functor.map(f, ma).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<F, B> Map<F, A, B>(this Func<A, B> f, Validation<F, A> ma) 
        where F : Monoid<F> =>
        Functor.map(f, ma).As();
}    
