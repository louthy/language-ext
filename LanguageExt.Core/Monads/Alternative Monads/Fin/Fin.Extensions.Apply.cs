using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinExtensions
{
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Fin<B> Action<A, B>(this Fin<A> ma, K<Fin, B> mb) =>
        Applicative.action(ma, mb).As();
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Fin<B> Action<A, B>(this K<Fin, A> ma, K<Fin, B> mb) =>
        Applicative.action(ma, mb).As();

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
    public static Fin<B> Apply<A, B>(this Fin<Func<A, B>> mf, K<Fin, A> ma) =>
        Applicative.apply(mf, ma).As();

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
    public static Fin<B> Apply<A, B>(this K<Fin, Func<A, B>> mf, K<Fin, A> ma) =>
        Applicative.apply(mf, ma).As();
}    
