using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct an applicative structure from a pure value  
    /// </summary>
    /// <param name="value">Pure value to lift into the applicative structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Applicative structure</returns>
    [Pure]
    public static K<F, A> pure<F, A>(A value)
        where F : Applicative<F> =>
        F.Pure(value);    
    
    [Pure]
    public static K<M, B> applyM<M, A, B>(K<M, Func<A, K<M, B>>> mf, K<M, A> ma)
        where M : Monad<M> =>
        M.Apply(mf, ma).Flatten();
    
    [Pure]
    public static K<AF, B> apply<AF, A, B>(K<AF, Func<A, B>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(mf, ma);

    [Pure]
    public static K<AF, Func<B, C>> apply<AF, A, B, C>(K<AF, Func<A, B, C>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, D>>> apply<AF, A, B, C, D>(K<AF, Func<A, B, C, D>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, E>>>> apply<AF, A, B, C, D, E>(K<AF, Func<A, B, C, D, E>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, F>>>>> apply<AF, A, B, C, D, E, F>(K<AF, Func<A, B, C, D, E, F>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, G>>>>>> apply<AF, A, B, C, D, E, F, G>(K<AF, Func<A, B, C, D, E, F, G>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> apply<AF, A, B, C, D, E, F, G, H>(K<AF, Func<A, B, C, D, E, F, G, H>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I>(K<AF, Func<A, B, C, D, E, F, G, H, I>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I, J>(K<AF, Func<A, B, C, D, E, F, G, H, I, J>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I, J, K>(K<AF, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);
    
    [Pure]
    public static K<F, B> action<F, A, B>(K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Action(ma, mb);
    
    [Pure]
    public static K<F, A> actions<F, A>(IEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
    
    [Pure]
    public static K<F, A> actions<F, A>(IAsyncEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
    
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
    public static K<F, (A First, B Second)> zip<F, A, B>(
        (K<F, A> First, K<F, B> Second) tuple)
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
    public static K<F, (A First, B Second, C Third)> zip<F, A, B, C>(
        (K<F, A> First, K<F, B> Second, K<F, C> Third) tuple)
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
    public static K<F, (A First, B Second, C Third, D Fourth)> zip<F, A, B, C, D>(
        (K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth) tuple)
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
    public static K<F, (A First, B Second, C Third, D Fourth, E Fifth)> zip<F, A, B, C, D, E>(
        (K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth, K<F, E> Fifth) tuple)
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
    public static K<F, (A First, B Second)> zip<F, A, B>(
        K<F, A> First, K<F, B> Second)
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
    public static K<F, (A First, B Second, C Third)> zip<F, A, B, C>(
        K<F, A> First, K<F, B> Second, K<F, C> Third)
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
    public static K<F, (A First, B Second, C Third, D Fourth)> zip<F, A, B, C, D>(
        K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth)
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
    public static K<F, (A First, B Second, C Third, D Fourth, E Fifth)> zip<F, A, B, C, D, E>(
        K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth, K<F, E> Fifth)
        where F : Applicative<F> =>
        map((A a, B b, C c, D d, E e) => (a, b, c, d, e), First)
           .Apply(Second)
           .Apply(Third)
           .Apply(Fourth)
           .Apply(Fifth);    
}
