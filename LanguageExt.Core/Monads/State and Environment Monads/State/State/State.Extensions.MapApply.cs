using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StateExtensions
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
    public static State<S, B> Map<S, A, B>(this Func<A, B> f, K<State<S>, A> ma) =>
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
    public static State<S, B> Map<S, A, B>(this Func<A, B> f, State<S, A> ma) =>
        ma.Map(f);    
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static State<S, B> Action<S, A, B>(this State<S, A> ma, K<State<S>, B> mb) =>
        ma.Kind().Action(mb).As();    
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static State<S, B> Action<S, A, B>(this K<State<S>, A> ma, K<State<S>, B> mb) =>
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
    public static State<S, B> Apply<S, A, B>(this State<S, Func<A, B>> mf, K<State<S>, A> ma) =>
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
    public static State<S, B> Apply<S, A, B>(this K<State<S>, Func<A, B>> mf, K<State<S>, A> ma) =>
        mf.As().Apply(ma);
}    
