using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterTExtensions
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
    public static WriterT<W, M, B> Map<W, M, A, B>(this Func<A, B> f, K<WriterT<W, M>, A> ma) 
        where W : Monoid<W> 
        where M : Monad<M>, SemiAlternative<M> =>
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
    public static WriterT<W, M, B> Map<W, M, A, B>(this Func<A, B> f, WriterT<W, M, A> ma)
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> =>
        Functor.map(f, ma).As();
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static WriterT<W, M, B> Action<W, M, A, B>(this WriterT<W, M, A> ma, K<WriterT<W, M>, B> mb)
        where W : Monoid<W> 
        where M : Monad<M>, SemiAlternative<M> =>
        Applicative.action(ma, mb).As();
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static WriterT<W, M, B> Action<W, M, A, B>(this K<WriterT<W, M>, A> ma, K<WriterT<W, M>, B> mb)
        where W : Monoid<W> 
        where M : Monad<M>, SemiAlternative<M> =>
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
    public static WriterT<W, M, B> Apply<W, M, A, B>(this WriterT<W, M, Func<A, B>> mf, K<WriterT<W, M>, A> ma)
        where W : Monoid<W> 
        where M : Monad<M>, SemiAlternative<M> =>
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
    public static WriterT<W, M, B> Apply<W, M, A, B>(this K<WriterT<W, M>, Func<A, B>> mf, K<WriterT<W, M>, A> ma)
        where W : Monoid<W> 
        where M : Monad<M>, SemiAlternative<M> =>
        Applicative.apply(mf, ma).As();
}    
