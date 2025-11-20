using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static ValidationT<F, M, B> Action<F, M, A, B>(this ValidationT<F, M, A> ma, K<ValidationT<F, M>, B> mb)
        where F : Monoid<F> 
        where M : Monad<M> =>
        Applicative.action(ma, mb).As();
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static ValidationT<F, M, B> Action<F, M, A, B>(this K<ValidationT<F, M>, A> ma, K<ValidationT<F, M>, B> mb)
        where F : Monoid<F> 
        where M : Monad<M> =>
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
    public static ValidationT<F, M, B> Apply<F, M, A, B>(this ValidationT<F, M, Func<A, B>> mf, K<ValidationT<F, M>, A> ma)
        where F : Monoid<F> 
        where M : Monad<M> =>
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
    public static ValidationT<F, M, B> Apply<F, M, A, B>(this K<ValidationT<F, M>, Func<A, B>> mf, K<ValidationT<F, M>, A> ma)
        where F : Monoid<F> 
        where M : Monad<M> =>
        Applicative.apply(mf, ma).As();
}    
