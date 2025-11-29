using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Ignores the bound value result and instead maps it to `Unit`
    /// </summary>
    /// <param name="fa">Functor that returns a bound value that should be ignored</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Functor with unit bound value</returns>
    public static K<F, Unit> ignore<F, A>(K<F, A> fa)
        where F : Functor<F> =>
        map(_ => default(Unit), fa);
    
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
    public static K<Fnctr, B> map<Fnctr, A, B>(
        Func<A, B> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, C>> map<Fnctr, A, B, C>(
        Func<A, B, C> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, D>>> map<Fnctr, A, B, C, D>(
        Func<A, B, C, D> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, E>>>> map<Fnctr, A, B, C, D, E>(
        Func<A, B, C, D, E> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, F>>>>> map<Fnctr, A, B, C, D, E, F>(
        Func<A, B, C, D, E, F> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> map<Fnctr, A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H, I>(
        Func<A, B, C, D, E, F, G, H, I> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H, I, J>(
        Func<A, B, C, D, E, F, G, H, I, J> f, K<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H, I, J, K>(
        Func<A, B, C, D, E, F, G, H, I, J, K> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);    
    
    
    
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
    public static K<Fnctr, B> map<Fnctr, A, B>(
        Func<A, B> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, C>> map<Fnctr, A, B, C>(
        Func<A, B, C> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, D>>> map<Fnctr, A, B, C, D>(
        Func<A, B, C, D> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, E>>>> map<Fnctr, A, B, C, D, E>(
        Func<A, B, C, D, E> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, F>>>>> map<Fnctr, A, B, C, D, E, F>(
        Func<A, B, C, D, E, F> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> map<Fnctr, A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H, I>(
        Func<A, B, C, D, E, F, G, H, I> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H, I, J>(
        Func<A, B, C, D, E, F, G, H, I, J> f, Memo<Fnctr, A> ma) 
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
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> map<Fnctr, A, B, C, D, E, F, G, H, I, J, K>(
        Func<A, B, C, D, E, F, G, H, I, J, K> f, Memo<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);    
}
