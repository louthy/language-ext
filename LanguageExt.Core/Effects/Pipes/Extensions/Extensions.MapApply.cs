using System;
using LanguageExt.Traits;
using LanguageExt.Pipes;

namespace LanguageExt;

public static partial class ProxyExtensions
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
    public static Proxy<UOut, UIn, DIn, DOut, M, B> Map<UOut, UIn, DIn, DOut, M, A, B>(
        this Func<A, B> f, 
        K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) 
        where M : Monad<M> =>
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
    public static Proxy<UOut, UIn, DIn, DOut, M, B> Map<UOut, UIn, DIn, DOut, M, A, B>(
        this Func<A, B> f, 
        Proxy<UOut, UIn, DIn, DOut, M, A> ma) 
        where M : Monad<M> =>
        ma.Map(f);    
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Proxy<UOut, UIn, DIn, DOut, M, B> Action<UOut, UIn, DIn, DOut, M, A, B>(
        this Proxy<UOut, UIn, DIn, DOut, M, A> ma, 
        K<Proxy<UOut, UIn, DIn, DOut, M>, B> mb) 
        where M : Monad<M> =>
        ma.Kind().Action(mb).As();    
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Proxy<UOut, UIn, DIn, DOut, M, B> Action<UOut, UIn, DIn, DOut, M, A, B>(
        this K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma, 
        K<Proxy<UOut, UIn, DIn, DOut, M>, B> mb) 
        where M : Monad<M> =>
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
    public static Proxy<UOut, UIn, DIn, DOut, M, B> Apply<UOut, UIn, DIn, DOut, M, A, B>(
        this Proxy<UOut, UIn, DIn, DOut, M, Func<A, B>> mf, 
        K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) 
        where M : Monad<M> =>
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
    public static Proxy<UOut, UIn, DIn, DOut, M, B> Apply<UOut, UIn, DIn, DOut, M, A, B>(
        this K<Proxy<UOut, UIn, DIn, DOut, M>, Func<A, B>> mf, 
        K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) 
        where M : Monad<M> =>
        mf.As().Apply(ma);
}    
