using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class ValidationExtensions
{
    public static Validation<F, A> As<F, A>(this K<Validation<F>, A> ma) 
        where F : Monoid<F> =>
        (Validation<F, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Validation<L, A> Flatten<L, A>(this Validation<L, Validation<L, A>> mma)
        where L : Monoid<L> =>
        mma.Bind(x => x);

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static Validation<L, B> Apply<L, A, B>(this Validation<L, Func<A, B>> mf, Validation<L, A> ma)
        where L : Monoid<L> =>
        mf switch
        {
            Validation.Success<L, Func<A, B>> (var f) =>
                ma switch
                {
                    Validation.Success<L, A> (var a) =>
                        Validation<L, B>.Success(f(a)),

                    Validation.Fail<L, A> (var e) =>
                        Validation<L, B>.Fail(e),

                    _ =>
                        Validation<L, B>.Fail(L.Empty)
                },

            Validation.Fail<L, Func<A, B>> (var e1) =>
                ma switch
                {
                    Validation.Fail<L, A> (var e2) =>
                        Validation<L, B>.Fail(e1 + e2),

                    _ =>
                        Validation<L, B>.Fail(e1)

                },
            _ => Validation<L, B>.Fail(L.Empty)
        };

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static Validation<L, C> Apply<L, A, B, C>(
        this Validation<L, Func<A, B, C>> mf, 
        Validation<L, A> ma,
        Validation<L, B> mb)
        where L : Monoid<L> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static Validation<L, Func<B, C>> Apply<L, A, B, C>(
        this Validation<L, Func<A, B, C>> mf, 
        Validation<L, A> ma)
        where L : Monoid<L> =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static Validation<L, B> ApplyM<L, A, B>(
        this Validation<L, Func<A, Validation<L, B>>> mf,
        Validation<L, A> ma)
        where L : Monoid<L> =>
        mf.Apply(ma).Flatten();

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static Validation<L, C> ApplyM<L, A, B, C>(
        this Validation<L, Func<A, B, Validation<L, C>>> mf, 
        Validation<L, A> ma,
        Validation<L, B> mb)
        where L : Monoid<L> =>
        mf.Map(curry).Apply(ma).Apply(mb).Flatten();

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static Validation<L, Func<B, Validation<L, C>>> ApplyM<L, A, B, C>(
        this Validation<L, Func<A, B, Validation<L, C>>> mf, 
        Validation<L, A> ma)
        where L : Monoid<L> =>
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure]
    public static Validation<L, B> Action<L, A, B>(this Validation<L, A> ma, Validation<L, B> mb)
        where L : Monoid<L> =>
        fun((A _, B b) => b).Map(ma).Apply(mb).As();

    /// <summary>
    /// Extract only the successes 
    /// </summary>
    /// <param name="vs">Enumerable of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of successes</returns>
    [Pure]
    public static IEnumerable<S> Successes<F, S>(this IEnumerable<Validation<F, S>> vs)
        where F : Monoid<F>
    {
        foreach (var v in vs)
        {
            if (v.IsSuccess) yield return (S)v;
        }
    }

    /// <summary>
    /// Extract only the failures 
    /// </summary>
    /// <param name="vs">Enumerable of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of failures</returns>
    [Pure]
    public static IEnumerable<F> Fails<F, S>(this IEnumerable<Validation<F, S>> vs)
        where F : Monoid<F>
    {
        foreach (var v in vs)
        {
            if (v.IsFail) yield return (F)v;
        }
    }
    
    /// <summary>
    /// Extract only the successes 
    /// </summary>
    /// <param name="vs">Seq of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of successes</returns>
    [Pure]
    public static Seq<S> Successes<F, S>(this Seq<Validation<F, S>> vs)
        where F : Monoid<F> =>
        toSeq(Successes(vs.AsEnumerable()));
    
    /// <summary>
    /// Extract only the failures 
    /// </summary>
    /// <param name="vs">Seq of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of failures</returns>
    [Pure]
    public static Seq<F> Fails<F, S>(this Seq<Validation<F, S>> vs)
        where F : Monoid<F> => 
        toSeq(Fails(vs.AsEnumerable()));

    /// <summary>
    /// Convert `Validation` type to `Fin` type.
    /// </summary>
    [Pure]
    public static Fin<A> ToFin<A>(this Validation<Error, A> ma) =>
        ma switch
        {
            Validation.Success<Error, A> (var x) => new Fin.Succ<A>(x),
            Validation.Fail<Error, A> (var x)    => new Fin.Fail<A>(x),
            _                                    => throw new NotSupportedException()
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
}
