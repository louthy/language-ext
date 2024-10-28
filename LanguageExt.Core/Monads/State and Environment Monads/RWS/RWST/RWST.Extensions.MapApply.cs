using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RWSTExtensions
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
    public static RWST<R, W, S, M, B> Map<R, W, S, M, A, B>(this Func<A, B> f, K<RWST<R, W, S, M>, A> ma)
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
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
    public static RWST<R, W, S, M, B> Map<R, W, S, M, A, B>(this Func<A, B> f, RWST<R, W, S, M, A> ma) 
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
        Functor.map(f, ma).As();
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static RWST<R, W, S, M, B> Action<R, W, S, M, A, B>(this RWST<R, W, S, M, A> ma, K<RWST<R, W, S, M>, B> mb) 
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
        Applicative.action(ma, mb).As();

    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static RWST<R, W, S, M, B> Action<R, W, S, M, A, B>(this K<RWST<R, W, S, M>, A> ma, K<RWST<R, W, S, M>, B> mb) 
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
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
    public static RWST<R, W, S, M, B> Apply<R, W, S, M, A, B>(this RWST<R, W, S, M, Func<A, B>> mf, K<RWST<R, W, S, M>, A> ma) 
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
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
    public static RWST<R, W, S, M, B> Apply<R, W, S, M, A, B>(this K<RWST<R, W, S, M>, Func<A, B>> mf, K<RWST<R, W, S, M>, A> ma)
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
        Applicative.apply(mf, ma).As();
}    
