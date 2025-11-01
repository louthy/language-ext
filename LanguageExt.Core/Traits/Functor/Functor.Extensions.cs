using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FunctorExtensions
{
    /// <summary>
    /// Ignores the bound value result and instead maps it to `Unit`
    /// </summary>
    /// <param name="fa">Functor that returns a bound value that should be ignored</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Functor with unit bound value</returns>
    public static K<F, Unit> IgnoreF<F, A>(this K<F, A> fa)
        where F : Functor<F> =>
        fa.Map(_ => default(Unit));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<F, C> SelectMany<E, F, B, C>(
        this Guard<E, Unit> ma,
        Func<Unit, K<F, B>> bind,
        Func<Unit, B, C> project)
        where F : Functor<F>, Fallible<E, F> =>
        ma switch
        {
            { Flag: true } => F.Map(b => project(default, b), bind(default)),
            var guard      => F.Fail<C>(guard.OnFalse())
        };

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<F, C> SelectMany<E, F, B, C>(
        this Guard<Fail<E>, Unit> ma,
        Func<Unit, K<F, B>> bind,
        Func<Unit, B, C> project)
        where F : Functor<F>, Fallible<E, F> =>
        ma switch
        {
            { Flag: true } => F.Map(b => project(default, b), bind(default)),
            var guard      => F.Fail<C>(guard.OnFalse().Value)
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
    /// Invokes the functor, extracting its bound value, which it then ignores and
    /// then maps to the default `value` provided.  This is useful for side-effecting
    /// monads that have an effect on the world, which the result value can be ignored,
    /// to then return a default.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="value">Ignore the bound value and map to this</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<Fnctr, B> Map<Fnctr, A, B>(
        this K<Fnctr, A> ma, B value) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(_ => value, ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
    
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
        Fnctr.Map(curry(f), ma);
}
