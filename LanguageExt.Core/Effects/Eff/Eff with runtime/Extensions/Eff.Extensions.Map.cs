using System;
using static LanguageExt.Prelude;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Extension methods for Fin
/// </summary>
public static partial class EffExtensions
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
    public static Eff<RT, B> Map<RT, A, B>(this Func<A, B> f, K<Eff<RT>, A> ma) =>
        ma.Map(f).As();
    
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
    public static Eff<RT, Func<B, C>> Map<RT, A, B, C>(
        this Func<A, B, C> f, K<Eff<RT>, A> ma) =>
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, D>>> Map<RT, A, B, C, D>(
        this Func<A, B, C, D> f, K<Eff<RT>, A> ma) =>
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> Map<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<Eff<RT>, A> ma) =>
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<RT, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<Eff<RT>, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<RT, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<Eff<RT>, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<RT, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<Eff<RT>, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<RT, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<Eff<RT>, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<RT, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<Eff<RT>, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<RT, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<Eff<RT>, A> ma) => 
        ma.Map(x => curry(f)(x)).As();    
    
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
    public static Eff<RT, B> Map<RT, A, B>(this Func<A, B> f, Eff<RT, A> ma) =>
        ma.Map(f).As();
    
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
    public static Eff<RT, Func<B, C>> Map<RT, A, B, C>(this Func<A, B, C> f, Eff<RT, A> ma) =>
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, D>>> Map<RT, A, B, C, D>(
        this Func<A, B, C, D> f, Eff<RT, A> ma) =>
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> Map<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, Eff<RT, A> ma) =>
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<RT, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, Eff<RT, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<RT, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, Eff<RT, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<RT, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, Eff<RT, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<RT, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, Eff<RT, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<RT, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, Eff<RT, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
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
    public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<RT, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, Eff<RT, A> ma) => 
        ma.Map(x => curry(f)(x)).As();    
}
