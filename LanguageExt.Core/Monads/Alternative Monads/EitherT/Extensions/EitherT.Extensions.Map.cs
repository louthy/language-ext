using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherTExtensions
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
    public static EitherT<L, M, B> Map<L, M, A, B>(this Func<A, B> f, K<EitherT<L, M>, A> ma) 
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
    public static EitherT<L, M, B> Map<L, M, A, B>(this Func<A, B> f, EitherT<L, M, A> ma)
        where M : Monad<M> =>
        Functor.map(f, ma).As();
}    
