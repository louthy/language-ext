﻿using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinTExtensions
{
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static FinT<M, B> Action<M, A, B>(this FinT<M, A> ma, FinT<M, B> mb) 
        where M : Monad<M> =>
        ma.Kind().Action(mb).As();    

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
    public static FinT<M, B> Apply<M, A, B>(this FinT<M, Func<A, B>> mf, K<FinT<M>, A> ma) 
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
    public static FinT<M, B> Apply<M, A, B>(this K<FinT<M>, Func<A, B>> mf, K<FinT<M>, A> ma)
        where M : Monad<M> =>
        Applicative.apply(mf, ma).As();
}    
