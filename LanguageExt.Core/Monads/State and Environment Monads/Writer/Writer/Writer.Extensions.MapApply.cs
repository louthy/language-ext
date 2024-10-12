using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterExtensions
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
    public static Writer<W, B> Map<W, A, B>(this Func<A, B> f, K<Writer<W>, A> ma) 
        where W : Monoid<W> =>
        ma.As().Map(f);
    
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
    public static Writer<W, B> Map<W, A, B>(this Func<A, B> f, Writer<W, A> ma) 
        where W : Monoid<W> =>
        ma.Map(f);    
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Writer<W, B> Action<W, A, B>(this Writer<W, A> ma, K<Writer<W>, B> mb) 
        where W : Monoid<W> =>
        ma.Kind().Action(mb).As();    
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Writer<W, B> Action<W, A, B>(this K<Writer<W>, A> ma, K<Writer<W>, B> mb)
        where W : Monoid<W> =>
        ma.As().Action(mb);    

    /// <summary>
    /// Applicative functor apply operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the `ma` applicative-functor, passes it to the unwrapped function(s) within `mf`, and
    /// then takes the resulting value and wraps it back up into a new applicative-functor.
    /// </remarks>
    /// <param name="ma">Value(s) applicative functor</param>
    /// <param name="mf">Mapping function(s)</param>
    /// <returns>Mapped applicative functor</returns>
    public static Writer<W, B> Apply<W, A, B>(this Writer<W, Func<A, B>> mf, K<Writer<W>, A> ma)
        where W : Monoid<W> =>
        mf.Kind().Apply(ma).As();

    /// <summary>
    /// Applicative functor apply operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the `ma` applicative-functor, passes it to the unwrapped function(s) within `mf`, and
    /// then takes the resulting value and wraps it back up into a new applicative-functor.
    /// </remarks>
    /// <param name="ma">Value(s) applicative functor</param>
    /// <param name="mf">Mapping function(s)</param>
    /// <returns>Mapped applicative functor</returns>
    public static Writer<W, B> Apply<W, A, B>(this K<Writer<W>, Func<A, B>> mf, K<Writer<W>, A> ma) 
        where W : Monoid<W> =>
        mf.As().Apply(ma);
}    
