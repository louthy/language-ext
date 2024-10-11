using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// EitherT monad-transformer extensions
/// </summary>
public static partial class EitherTExtensions
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
    public static EitherT<L, M, B> Map<L, M, A, B>(this Func<A, B> f, K<EitherT<L, M>, A> ma)
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
    public static EitherT<L, M, Func<B, C>> Map<L, M, A, B, C>(
        this Func<A, B, C> f, K<EitherT<L, M>, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, D>>> Map<L, M, A, B, C, D>(
        this Func<A, B, C, D> f, K<EitherT<L, M>, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, E>>>> Map<L, M, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<EitherT<L, M>, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<L, M, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<EitherT<L, M>, A> ma) 
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<L, M, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<EitherT<L, M>, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<L, M, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<EitherT<L, M>, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<L, M, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<EitherT<L, M>, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<L, M, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<EitherT<L, M>, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<L, M, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<EitherT<L, M>, A> ma) 
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
    public static EitherT<L, M, B> Map<L, M, A, B>(this Func<A, B> f, EitherT<L, M, A> ma)
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
    public static EitherT<L, M, Func<B, C>> Map<L, M, A, B, C>(
        this Func<A, B, C> f, EitherT<L, M, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, D>>> Map<L, M, A, B, C, D>(
        this Func<A, B, C, D> f, EitherT<L, M, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, E>>>> Map<L, M, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, EitherT<L, M, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<L, M, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, EitherT<L, M, A> ma) 
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<L, M, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, EitherT<L, M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<L, M, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, EitherT<L, M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<L, M, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, EitherT<L, M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<L, M, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, EitherT<L, M, A> ma)
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
    public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<L, M, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, EitherT<L, M, A> ma) 
        where M : Monad<M> =>
        ma.Map(x => curry(f)(x)).As();    
}
