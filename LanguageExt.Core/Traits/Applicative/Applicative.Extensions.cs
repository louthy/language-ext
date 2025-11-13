using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Monad module
/// </summary>
public static partial class ApplicativeExtensions
{
    [Pure]
    public static K<M, B> ApplyM<M, A, B>(this K<M, Func<A, K<M, B>>> mf, K<M, A> ma)
        where M : Monad<M> =>
        M.Apply(mf, ma).Flatten();
    
    [Pure]
    public static K<AF, B> Apply<AF, A, B>(this K<AF, Func<A, B>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(mf, ma);
    
    [Pure]
    public static K<AF, B> Apply<AF, A, B>(this K<AF, Func<A, B>> mf, Func<K<AF, A>> ma)
        where AF : Applicative<AF> =>
        AF.ApplyLazy(mf, ma);

    [Pure]
    public static K<AF, Func<B, C>> Apply<AF, A, B, C>(this K<AF, Func<A, B, C>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, D>>> Apply<AF, A, B, C, D>(this K<AF, Func<A, B, C, D>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, E>>>> Apply<AF, A, B, C, D, E>(this K<AF, Func<A, B, C, D, E>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, F>>>>> Apply<AF, A, B, C, D, E, F>(this K<AF, Func<A, B, C, D, E, F>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply<AF, A, B, C, D, E, F, G>(this K<AF, Func<A, B, C, D, E, F, G>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply<AF, A, B, C, D, E, F, G, H>(this K<AF, Func<A, B, C, D, E, F, G, H>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply<AF, A, B, C, D, E, F, G, H, I>(this K<AF, Func<A, B, C, D, E, F, G, H, I>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Apply<AF, A, B, C, D, E, F, G, H, I, J>(this K<AF, Func<A, B, C, D, E, F, G, H, I, J>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Apply<AF, A, B, C, D, E, F, G, H, I, J, K>(this K<AF, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);
    
    [Pure]
    public static K<F, B> Action<F, A, B>(this K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Action(ma, mb);
    
    [Pure]
    public static K<F, A> Actions<F, A>(this IEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
    
    [Pure]
    public static K<F, A> Actions<F, A>(this IAsyncEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);

    [Pure]
    public static K<F, B> Lift<F, A, B>(this Func<A, B> f, K<F, A> fa)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa);

    [Pure]
    public static K<F, C> Lift<F, A, B, C>(this Func<A, B, C> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    [Pure]
    public static K<F, C> Lift<F, A, B, C>(this Func<A, Func<B, C>> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    [Pure]
    public static K<F, D> Lift<F, A, B, C, D>(this Func<A, B, C, D> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);

    [Pure]
    public static K<F, D> Lift<F, A, B, C, D>(this Func<A, Func<B, Func<C, D>>> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, R>(
        this (K<Fnctr, A>, K<Fnctr, B>) items,
        Func<A, B, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, R>(
        this (K<M, A>,
              K<M, B>) items,
        Func<A, B, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>) items, Func<A, B, C, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, R>(
        this (K<M, A>,
              K<M, B>,
              K<M, C>) items,
        Func<A, B, C, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>) items,
        Func<A, B, C, D, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, D, R>(
        this (K<M, A>, K<M, B>, K<M, C>, K<M, D>) items,
        Func<A, B, C, D, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>) items,
        Func<A, B, C, D, E, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, D, E, R>(
        this (K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>) items,
        Func<A, B, C, D, E, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>, K<Fnctr, F>) items,
        Func<A, B, C, D, E, F, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, D, E, F, R>(
        this (K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>) items,
        Func<A, B, C, D, E, F, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>, K<Fnctr, F>, K<Fnctr, G>) items,
        Func<A, B, C, D, E, F, G, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, D, E, F, G, R>(
        this (K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>) items,
        Func<A, B, C, D, E, F, G, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, H, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>, K<Fnctr, F>, K<Fnctr, G>, K<Fnctr, H>) items,
        Func<A, B, C, D, E, F, G, H, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7)
         .Apply(items.Item8);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, D, E, F, G, H, R>(
        this (K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>) items,
        Func<A, B, C, D, E, F, G, H, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, H, I, R>(
        this (K<Fnctr, A>,
              K<Fnctr, B>,
              K<Fnctr, C>,
              K<Fnctr, D>,
              K<Fnctr, E>,
              K<Fnctr, F>,
              K<Fnctr, G>,
              K<Fnctr, H>,
              K<Fnctr, I>) items,
        Func<A, B, C, D, E, F, G, H, I, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7)
         .Apply(items.Item8)
         .Apply(items.Item9);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, D, E, F, G, H, I, R>(
        this (K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>, K<M, I>) items,
        Func<A, B, C, D, E, F, G, H, I, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, H, I, J, R>(
        this (K<Fnctr, A>,
            K<Fnctr, B>,
            K<Fnctr, C>,
            K<Fnctr, D>,
            K<Fnctr, E>,
            K<Fnctr, F>,
            K<Fnctr, G>,
            K<Fnctr, H>,
            K<Fnctr, I>,
            K<Fnctr, J>) items,
        Func<A, B, C, D, E, F, G, H, I, J, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7)
         .Apply(items.Item8)
         .Apply(items.Item9)
         .Apply(items.Item10);

    [Pure]
    public static K<M, R> ApplyM<M, A, B, C, D, E, F, G, H, I, J, R>(
        this (K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>, K<M, I>, K<M, J>) items,
        Func<A, B, C, D, E, F, G, H, I, J, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    /// <summary>
    /// Sum the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Add<NumA, F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where NumA : Num<A> =>
        (fa, fb).Apply(NumA.Add);

    /// <summary>
    /// Sum the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Add<F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where A : IAdditionOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x + y);

    /// <summary>
    /// Subtract the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Subtract<NumA, F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where NumA : Arithmetic<A> =>
        (fa, fb).Apply(NumA.Subtract);

    /// <summary>
    /// Subtract the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Subtract<F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where A : ISubtractionOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x - y);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Multiply<NumA, F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where NumA : Arithmetic<A> =>
        (fa, fb).Apply(NumA.Multiply);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Multiply<F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where A : IMultiplyOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x * y);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Divide<NumA, F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where NumA : Num<A> =>
        (fa, fb).Apply(NumA.Divide);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Divide<F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where A : IDivisionOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x / y);
    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Zipping
    //

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second)> Zip<F, A, B>(
        this (K<F, A> First, K<F, B> Second) tuple)
        where F : Applicative<F> =>
        map((A a, B b) => (a, b), tuple.First).Apply(tuple.Second);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third)> Zip<F, A, B, C>(
        this (K<F, A> First, K<F, B> Second, K<F, C> Third) tuple)
        where F : Applicative<F> =>
        map((A a, B b, C c) => (a, b, c), tuple.First)
           .Apply(tuple.Second)
           .Apply(tuple.Third);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth)> Zip<F, A, B, C, D>(
        this (K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth) tuple)
        where F : Applicative<F> =>
        map((A a, B b, C c, D d) => (a, b, c, d), tuple.First)
           .Apply(tuple.Second)
           .Apply(tuple.Third)
           .Apply(tuple.Fourth);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth, E Fifth)> Zip<F, A, B, C, D, E>(
        this (K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth, K<F, E> Fifth) tuple)
        where F : Applicative<F> =>
        map((A a, B b, C c, D d, E e) => (a, b, c, d, e), tuple.First)
           .Apply(tuple.Second)
           .Apply(tuple.Third)
           .Apply(tuple.Fourth)
           .Apply(tuple.Fifth);
    

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="First">First applicative</param>
    /// <param name="Second">Second applicative</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second)> Zip<F, A, B>(
        this K<F, A> First, K<F, B> Second)
        where F : Applicative<F> =>
        map((A a, B b) => (a, b), First).Apply(Second);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="First">First applicative</param>
    /// <param name="Second">Second applicative</param>
    /// <param name="Third">Third applicative</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third)> Zip<F, A, B, C>(
        this K<F, A> First, K<F, B> Second, K<F, C> Third)
        where F : Applicative<F> =>
        map((A a, B b, C c) => (a, b, c), First)
           .Apply(Second)
           .Apply(Third);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="First">First applicative</param>
    /// <param name="Second">Second applicative</param>
    /// <param name="Third">Third applicative</param>
    /// <param name="Fourth">Fourth applicative</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth)> Zip<F, A, B, C, D>(
        this K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth)
        where F : Applicative<F> =>
        map((A a, B b, C c, D d) => (a, b, c, d), First)
           .Apply(Second)
           .Apply(Third)
           .Apply(Fourth);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="First">First applicative</param>
    /// <param name="Second">Second applicative</param>
    /// <param name="Third">Third applicative</param>
    /// <param name="Fourth">Fourth applicative</param>
    /// <param name="Fifth">Fifth applicative</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth, E Fifth)> Zip<F, A, B, C, D, E>(
        this K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth, K<F, E> Fifth)
        where F : Applicative<F> =>
        map((A a, B b, C c, D d, E e) => (a, b, c, d, e), First)
           .Apply(Second)
           .Apply(Third)
           .Apply(Fourth)
           .Apply(Fifth);      
}
