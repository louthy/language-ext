using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using LanguageExt.Common;

namespace LanguageExt;

/// <summary>
/// Extension methods for Fin
/// </summary>
public static partial class FinExtensions
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
    public static Fin<B> Map<A, B>(this Func<A, B> f, K<Fin, A> ma) =>
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
    public static Fin<Func<B, C>> Map<A, B, C>(
        this Func<A, B, C> f, K<Fin, A> ma) =>
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
    public static Fin<Func<B, Func<C, D>>> Map<A, B, C, D>(
        this Func<A, B, C, D> f, K<Fin, A> ma) =>
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
    public static Fin<Func<B, Func<C, Func<D, E>>>> Map<A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<Fin, A> ma) =>
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
    public static Fin<Func<B, Func<C, Func<D, Func<E, F>>>>> Map<A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<Fin, A> ma) => 
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
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<Fin, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<Fin, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<Fin, A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<Fin, A> ma) => 
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
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<Fin, A> ma) => 
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
    public static Fin<B> Map<A, B>(this Func<A, B> f, Fin<A> ma) =>
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
    public static Fin<Func<B, C>> Map<A, B, C>(this Func<A, B, C> f, Fin<A> ma) =>
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
    public static Fin<Func<B, Func<C, D>>> Map<A, B, C, D>(
        this Func<A, B, C, D> f, Fin<A> ma) =>
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
    public static Fin<Func<B, Func<C, Func<D, E>>>> Map<A, B, C, D, E>(
        this Func<A, B, C, D, E> f, Fin<A> ma) =>
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
    public static Fin<Func<B, Func<C, Func<D, Func<E, F>>>>> Map<A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, Fin<A> ma) => 
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
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, Fin<A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, Fin<A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, Fin<A> ma) => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, Fin<A> ma) => 
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
    public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, Fin<A> ma) => 
        ma.Map(x => curry(f)(x)).As();    
}
