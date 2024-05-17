using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FunctorExtensions
{
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M<C></returns>
    public static K<F, C> SelectMany<F, A, C>(
        this K<F, A> ma,
        Func<A, Guard<Error, Unit>> bind,
        Func<A, Unit, C> project)
        where F : Functor<F> =>
        F.Map(a => bind(a) switch
                   {
                       { Flag: true } => project(a, default),
                       var guard      => guard.OnFalse().Throw<C>()
                   }, ma);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M<C></returns>
    public static K<F, C> SelectMany<F, A, C>(
        this K<F, A> ma,
        Func<A, Guard<Fail<Error>, Unit>> bind,
        Func<A, Unit, C> project)
        where F : Functor<F> =>
        F.Map(a => bind(a) switch
                   {
                       { Flag: true } => project(a, default),
                       var guard      => guard.OnFalse().Value.Throw<C>()
                   }, ma);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M<C></returns>
    public static K<F, C> SelectMany<F, B, C>(
        this Guard<Error, Unit> ma,
        Func<Unit, K<F, B>> bind,
        Func<Unit, B, C> project)
        where F : Functor<F> =>
        ma switch
        {
            { Flag: true } => F.Map(b => project(default, b), bind(default)),
            var guard      => guard.OnFalse().Throw<K<F, C>>()
        };

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M<C></returns>
    public static K<F, C> SelectMany<F, B, C>(
        this Guard<Fail<Error>, Unit> ma,
        Func<Unit, K<F, B>> bind,
        Func<Unit, B, C> project)
        where F : Functor<F> =>
        ma switch
        {
            { Flag: true } => F.Map(b => project(default, b), bind(default)),
            var guard      => guard.OnFalse().Value.Throw<K<F, C>>()
        };
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, B> Map<Fnctr, A, B>(
        this K<Fnctr, A> ma, Func<A, B> f) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(f, ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, B> Select<Fnctr, A, B>(
        this K<Fnctr, A> ma, Func<A, B> f) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(f, ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, B> Map<Fnctr, A, B>(
        this Func<A, B> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(f, ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, C>> Map<Fnctr, A, B, C>(
        this Func<A, B, C> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, D>>> Map<Fnctr, A, B, C, D>(
        this Func<A, B, C, D> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, Func<D, E>>>> Map<Fnctr, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<Fnctr, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<Fnctr, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <remarks>
    /// This variant takes the passed function and partially applies it, so the result is a
    /// functor within the value being the partially applied function.  If `Fnctr` is also an
    /// `Applicative`, which is often the case, you can provide further arguments to the
    /// partially applied function by calling `.Apply()` on the resulting functor.
    ///
    /// You can continue this until all arguments have been provided and then you'll have
    /// a functor within the result of the function wrapped up inside.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
}
