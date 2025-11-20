using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationTExtensions
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
    public static ValidationT<F, M, B> Map<F, M, A, B>(this Func<A, B> f, K<ValidationT<F, M>, A> ma)
        where F : Monoid<F> 
        where M : Monad<M> =>
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
    public static ValidationT<F, M, B> Map<F, M, A, B>(this Func<A, B> f, ValidationT<F, M, A> ma) 
        where F : Monoid<F> 
        where M : Monad<M> =>
        Functor.map(f, ma).As();
}    
