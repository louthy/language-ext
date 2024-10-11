using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Extension methods for Fin
/// </summary>
public static partial class StreamTExtensions
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
    public static StreamT<M, B> Map<M, A, B>(this Func<A, B> f, K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, C>> Map<M, A, B, C>(
        this Func<A, B, C> f, K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, D>>> Map<M, A, B, C, D>(
        this Func<A, B, C, D> f, K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, E>>>> Map<M, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<StreamT<M>, A> ma)
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<M, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<M, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<M, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<StreamT<M>, A> ma)  
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<M, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<M, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<M, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<StreamT<M>, A> ma)  
        where M : Monad<M> =>
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
    public static StreamT<M, B> Map<M, A, B>(this Func<A, B> f, StreamT<M, A> ma)
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, C>> Map<M, A, B, C>(this Func<A, B, C> f, StreamT<M, A> ma)
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, D>>> Map<M, A, B, C, D>(
        this Func<A, B, C, D> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, E>>>> Map<M, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<M, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<M, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<M, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<M, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<M, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
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
    public static StreamT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<M, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, StreamT<M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();    
}
