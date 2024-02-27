using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
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
        where F : Monoid<F>, Eq<F>
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
        where F : Monoid<F>, Eq<F>
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
        where F : Monoid<F>, Eq<F> =>
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
        where F : Monoid<F>, Eq<F> => 
        toSeq(Fails(vs.AsEnumerable()));
    
    [Pure]
    public static Validation<F, R> Apply<F, A, R>(
        this ValueTuple<Validation<F, A>> items, Func<A, R> f)
        where F : Monoid<F>, Eq<F> =>
        items.Item1.Match(Succ: s => f(s), Fail: Validation<F, R>.Fail);

    [Pure]
    public static Validation<F, R> Apply<F, A, B, R>(
        this ValueTuple<
            Validation<F, A>,
            Validation<F, B>> items,
        Func<A, B, R> f)
        where F : Monoid<F>, Eq<F> =>
        fun(f).Map(items.Item1).Apply(items.Item2).As();

    [Pure]
    public static Validation<F, R> Apply<F, A, B, C, R>(
        this ValueTuple<
            Validation<F, A>,
            Validation<F, B>,
            Validation<F, C>
        > items,
        Func<A, B, C, R> f)
        where F : Monoid<F>, Eq<F> =>
        fun(f).Map(items.Item1)
              .Apply(items.Item2)
              .Apply(items.Item3)
              .As();

    [Pure]
    public static Validation<F, R> Apply<F, A, B, C, D, R>(
        this ValueTuple<
            Validation<F, A>,
            Validation<F, B>,
            Validation<F, C>,
            Validation<F, D>
            > items,
        Func<A, B, C, D, R> f)
        where F : Monoid<F>, Eq<F> =>
        fun(f).Map(items.Item1)
              .Apply(items.Item2)
              .Apply(items.Item3)
              .Apply(items.Item4)
              .As();
    
    [Pure]
    public static Validation<F, R> Apply<F, A, B, C, D, E, R>(
        this ValueTuple<
            Validation<F, A>,
            Validation<F, B>,
            Validation<F, C>,
            Validation<F, D>,
            Validation<F, E>
            > items,
        Func<A, B, C, D, E, R> f)
        where F : Monoid<F>, Eq<F> =>
        fun(f).Map(items.Item1)
              .Apply(items.Item2)
              .Apply(items.Item3)
              .Apply(items.Item4)
              .Apply(items.Item5)
              .As();
    
    [Pure]
    public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>,
            Validation<FAIL, C>,
            Validation<FAIL, D>,
            Validation<FAIL, E>,
            Validation<FAIL, F>
            > items,
        Func<A, B, C, D, E, F, R> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        fun(f).Map(items.Item1)
              .Apply(items.Item2)
              .Apply(items.Item3)
              .Apply(items.Item4)
              .Apply(items.Item5)
              .Apply(items.Item6)
              .As();
    
    [Pure]
    public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>,
            Validation<FAIL, C>,
            Validation<FAIL, D>,
            Validation<FAIL, E>,
            Validation<FAIL, F>,
            Validation<FAIL, G>
            > items,
        Func<A, B, C, D, E, F, G, R> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        fun(f).Map(items.Item1)
              .Apply(items.Item2)
              .Apply(items.Item3)
              .Apply(items.Item4)
              .Apply(items.Item5)
              .Apply(items.Item6)
              .Apply(items.Item7)
              .As();

    [Pure]
    public static Validation<FAIL, R> ApplyM<FAIL, A, R>(this ValueTuple<Validation<FAIL, A>> items, Func<A, Validation<FAIL, R>> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        items.Apply(f).Flatten();
    
    [Pure]
    public static Validation<FAIL, R> ApplyM<FAIL, A, B, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>> items,
        Func<A, B, Validation<FAIL, R>> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        items.Apply(f).Flatten();
    
    [Pure]
    public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>,
            Validation<FAIL, C>
            > items,
        Func<A, B, C, Validation<FAIL, R>> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        items.Apply(f).Flatten();
    
    [Pure]
    public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>,
            Validation<FAIL, C>,
            Validation<FAIL, D>
            > items,
        Func<A, B, C, D, Validation<FAIL, R>> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        items.Apply(f).Flatten();
    
    [Pure]
    public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>,
            Validation<FAIL, C>,
            Validation<FAIL, D>,
            Validation<FAIL, E>
            > items,
        Func<A, B, C, D, E, Validation<FAIL, R>> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        items.Apply(f).Flatten();
    
    [Pure]
    public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>,
            Validation<FAIL, C>,
            Validation<FAIL, D>,
            Validation<FAIL, E>,
            Validation<FAIL, F>
            > items,
        Func<A, B, C, D, E, F, Validation<FAIL, R>> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        items.Apply(f).Flatten();
    
    [Pure]
    public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, R>(
        this ValueTuple<
            Validation<FAIL, A>,
            Validation<FAIL, B>,
            Validation<FAIL, C>,
            Validation<FAIL, D>,
            Validation<FAIL, E>,
            Validation<FAIL, F>,
            Validation<FAIL, G>
            > items,
        Func<A, B, C, D, E, F, G, Validation<FAIL, R>> f)
        where FAIL : Monoid<FAIL>, Eq<FAIL> =>
        items.Apply(f).Flatten();
}
