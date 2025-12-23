using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FunctorExtensions
{
    extension<E>(Guard<E, Unit> ma)
    {
        /// <summary>
        /// Monad bind operation
        /// </summary>
        /// <param name="bind">Monadic bind function</param>
        /// <param name="project">Projection function</param>
        /// <typeparam name="F">Functor trait</typeparam>
        /// <typeparam name="B">Intermediate bound value type</typeparam>
        /// <typeparam name="C">Target bound value type</typeparam>
        /// <returns>M〈C〉</returns>
        public K<F, C> SelectMany<F, B, C>(Func<Unit, K<F, B>> bind, Func<Unit, B, C> project)
            where F : Functor<F>, Fallible<E, F> =>
            ma switch
            {
                { Flag: true } => F.Map(b => project(default, b), bind(default)),
                var guard      => F.Fail<C>(guard.OnFalse())
            };
    }

    extension<E>(Guard<Fail<E>, Unit> ma)
    {
        /// <summary>
        /// Monad bind operation
        /// </summary>
        /// <param name="bind">Monadic bind function</param>
        /// <param name="project">Projection function</param>
        /// <typeparam name="F">Functor trait</typeparam>
        /// <typeparam name="B">Intermediate bound value type</typeparam>
        /// <typeparam name="C">Target bound value type</typeparam>
        /// <returns>M〈C〉</returns>
        public K<F, C> SelectMany<F, B, C>(Func<Unit, K<F, B>> bind, Func<Unit, B, C> project)
            where F : Functor<F>, Fallible<E, F> =>
            ma switch
            {
                { Flag: true } => F.Map(b => project(default, b), bind(default)),
                var guard      => F.Fail<C>(guard.OnFalse().Value)
            };
    }

    extension<Fnctr, A>(K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr>
    {
        /// <summary>
        /// Ignores the bound value result and instead maps it to `Unit`
        /// </summary>
        /// <returns>Functor with unit bound value</returns>
        public K<Fnctr, Unit> IgnoreF() =>
            ma.Map(_ => default(Unit));
        
        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <remarks>
        /// Unwraps the value within the functor, passes it to the map function `f` provided, and
        /// then takes the mapped value and wraps it back up into a new functor.
        /// </remarks>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped functor</returns>
        public K<Fnctr, B> Map<B>(Func<A, B> f) =>
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
        /// <param name="value">Ignore the bound value and map to this</param>
        /// <returns>Mapped functor</returns>
        public K<Fnctr, B> ConstMap<B>(B value) =>
            Fnctr.ConstMap(value, ma);

        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <remarks>
        /// Unwraps the value within the functor, passes it to the map function `f` provided, and
        /// then takes the mapped value and wraps it back up into a new functor.
        /// </remarks>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped functor</returns>
        public K<Fnctr, B> Select<B>(Func<A, B> f) =>
            Fnctr.Map(f, ma);
    }

    extension<Fnctr, A, B>(Func<A, B> f) where Fnctr : Functor<Fnctr>
    {
        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <remarks>
        /// Unwraps the value within the functor, passes it to the map function `f` provided, and
        /// then takes the mapped value and wraps it back up into a new functor.
        /// </remarks>
        /// <param name="ma">Functor to map</param>
        /// <returns>Mapped functor</returns>
        public K<Fnctr, B> Map(K<Fnctr, A> ma) =>
            Fnctr.Map(f, ma);
    }

    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    extension<Fnctr, A, B, C>(Func<A, B, C> f) where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, C>> Map(K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    extension<Fnctr, A, B, C, D>(Func<A, B, C, D> f) where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, D>>> Map(K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    extension<Fnctr, A, B, C, D, E>(Func<A, B, C, D, E> f) 
        where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, Func<D, E>>>> Map(K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    /// <param name="f">Mapping function</param>
    /// <typeparam name="Fnctr">Trait of the functor</typeparam>
    extension<Fnctr, A, B, C, D, E, F>(Func<A, B, C, D, E, F> f) 
        where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, Func<D, Func<E, F>>>>> Map(
            K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    extension<Fnctr, A, B, C, D, E, F, G>(Func<A, B, C, D, E, F, G> f) 
        where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map(
            K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    extension<Fnctr, A, B, C, D, E, F, G, H>(Func<A, B, C, D, E, F, G, H> f) 
        where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map(
            K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    extension<Fnctr, A, B, C, D, E, F, G, H, I>(Func<A, B, C, D, E, F, G, H, I> f) 
        where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map(
            K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    extension<Fnctr, A, B, C, D, E, F, G, H, I, J>(Func<A, B, C, D, E, F, G, H, I, J> f) 
        where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map(
            K<Fnctr, A> ma) =>
            curry(f) * ma;
    }

    extension<Fnctr, A, B, C, D, E, F, G, H, I, J, K>(Func<A, B, C, D, E, F, G, H, I, J, K> f) 
        where Fnctr : Functor<Fnctr>
    {
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
        /// <returns>Mapped functor</returns>
        public K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map(
            K<Fnctr, A> ma) =>
            curry(f) * ma;
    }
}
