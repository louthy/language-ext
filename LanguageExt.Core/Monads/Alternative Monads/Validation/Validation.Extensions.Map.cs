using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationExtensions
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
    public static Validation<FAIL, B> Map<FAIL, A, B>(this Func<A, B> f, K<Validation<FAIL>, A> ma) 
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, C>> Map<FAIL, A, B, C>(
        this Func<A, B, C> f, K<Validation<FAIL>, A> ma) 
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, D>>> Map<FAIL, A, B, C, D>(
        this Func<A, B, C, D> f, K<Validation<FAIL>, A> ma)
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, E>>>> Map<FAIL, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<Validation<FAIL>, A> ma)
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<FAIL, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<Validation<FAIL>, A> ma) 
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<FAIL, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<Validation<FAIL>, A> ma)  
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<Validation<FAIL>, A> ma)  
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<Validation<FAIL>, A> ma)  
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<Validation<FAIL>, A> ma)  
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<Validation<FAIL>, A> ma)  
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, B> Map<FAIL, A, B>(this Func<A, B> f, Validation<FAIL, A> ma) 
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, C>> Map<FAIL, A, B, C>(
        this Func<A, B, C> f, Validation<FAIL, A> ma) 
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, D>>> Map<FAIL, A, B, C, D>(
        this Func<A, B, C, D> f, Validation<FAIL, A> ma)
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, E>>>> Map<FAIL, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, Validation<FAIL, A> ma)
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<FAIL, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, Validation<FAIL, A> ma) 
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<FAIL, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, Validation<FAIL, A> ma)  
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, Validation<FAIL, A> ma)  
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, Validation<FAIL, A> ma)  
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, Validation<FAIL, A> ma)  
        where FAIL : Monoid<FAIL> =>
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
    public static Validation<FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<FAIL, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, Validation<FAIL, A> ma)  
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();    
}
