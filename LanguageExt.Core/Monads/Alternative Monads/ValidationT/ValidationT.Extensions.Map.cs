using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// ValidationT monad-transformer extensions
/// </summary>
public static partial class ValidationTExtensions
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
    public static ValidationT<FAIL, M, B> Map<FAIL, M, A, B>(this Func<A, B> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
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
    public static ValidationT<FAIL, M, Func<B, C>> Map<FAIL, M, A, B, C>(
        this Func<A, B, C> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
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
    public static ValidationT<FAIL, M, Func<B, Func<C, D>>> Map<FAIL, M, A, B, C, D>(
        this Func<A, B, C, D> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
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
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, E>>>> Map<FAIL, M, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
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
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<FAIL, M, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
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
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<FAIL, M, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
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
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();    
}
