using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Trait;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Extension methods for Option
/// </summary>
public static class OptionExtensions
{
    public static Option<A> As<A>(this K<Option, A> ma) =>
        (Option<A>)ma;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<F, A> ToValidation<F, A>(Option<A> ma, F defaultFailureValue)
        where F : Monoid<F>
        => ma.IsSome
               ? Validation<F, A>.Success(ma.Value!)
               : Validation<F, A>.Fail(defaultFailureValue);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Flatten<A>(this Option<Option<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Extracts from a list of `Option` all the `Some` elements.
    /// All the `Some` elements are extracted in order.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<A> Somes<A>(this IEnumerable<Option<A>> self)
    {
        foreach (var item in self)
        {
            if (item.IsSome)
            {
                yield return item.Value!;
            }
        }
    }

    /// <summary>
    /// Extracts from a list of `Option` all the `Some` elements.
    /// All the `Some` elements are extracted in order.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> Somes<A>(this Seq<Option<A>> self)
    {
        IEnumerable<A> ToSequence(Seq<Option<A>> items)
        {
            foreach (var item in items)
            {
                if (item.IsSome)
                {
                    yield return item.Value!;
                }
            }
        }
        return toSeq(ToSequence(self));
    }

    /// <summary>
    /// Add the bound values of x and y, uses an Add trait to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="ADD">Add of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y added to x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Add<ARITH, A>(this Option<A> x, Option<A> y) where ARITH : Arithmetic<A> =>
        from a in x
        from b in y
        select plus<ARITH, A>(a, b);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract trait
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="DIFF">Subtract of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the subtract between x and y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Subtract<ARITH, A>(this Option<A> x, Option<A> y) where ARITH : Arithmetic<A> =>
        from a in x
        from b in y
        select subtract<ARITH, A>(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product trait
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="PROD">Product of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the product of x and y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Product<ARITH, A>(this Option<A> x, Option<A> y) where ARITH : Arithmetic<A> =>
        from a in x
        from b in y
        select product<ARITH, A>(a, b);

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide trait to provide the divide
    /// operation for type A.  For example x.Divide<TDouble,double>(y)
    /// </summary>
    /// <typeparam name="DIV">Divide of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option x / y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Divide<NUM, A>(this Option<A> x, Option<A> y) where NUM : Num<A> =>
        from a in x
        from b in y
        select divide<NUM, A>(a, b);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<B> Apply<A, B>(this Option<Func<A, B>> fab, Option<A> fa) =>
        fab.Bind(fa.Map);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<C> Apply<A, B, C>(this Option<Func<A, B, C>> fabc, Option<A> fa, Option<B> fb) =>
        fabc.Bind(f => fa.Bind(a => fb.Map(b => f(a, b))));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<Func<B, C>> Apply<A, B, C>(this Option<Func<A, B, C>> fabc, Option<A> fa) =>
        fabc.Map(curry).Bind(fa.Map);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<Func<B, C>> Apply<A, B, C>(this Option<Func<A, Func<B, C>>> fabc, Option<A> fa) =>
        fabc.Bind(fa.Map);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<B> Action<A, B>(this Option<A> fa, Option<B> fb) =>
        fb;

    /// <summary>
    /// Convert the Option type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Option to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A? ToNullable<A>(this Option<A> ma) where A : struct =>
        ma.IsNone
            ? null
            : ma.Value;

    /// <summary>
    /// Match for an optional boolean
    /// </summary>
    /// <param name="ma">Optional boolean</param>
    /// <param name="True">Match for Some(true)</param>
    /// <param name="False">Match for Some(false)</param>
    /// <param name="None">Match for None</param>
    /// <typeparam name="R"></typeparam>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Match<R>(this Option<bool> ma, Func<R> True, Func<R> False, Func<R> None) =>
        ma.Match(Some: x => x ? True() : False(), None: None());

    /// <summary>
    /// Match over a list of options
    /// </summary>
    /// <typeparam name="T">Type of the bound values</typeparam>
    /// <typeparam name="R">Result type</typeparam>
    /// <param name="list">List of options to match against</param>
    /// <param name="Some">Operation to perform when an Option is in the Some state</param>
    /// <param name="None">Operation to perform when an Option is in the None state</param>
    /// <returns>An enumerable of results of the match operations</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<R> Match<T, R>(this IEnumerable<Option<T>> list,
        Func<T, IEnumerable<R>> Some,
        Func<IEnumerable<R>> None) =>
        match(list, Some, None);

    /// <summary>
    /// Match over a list of options
    /// </summary>
    /// <typeparam name="T">Type of the bound values</typeparam>
    /// <typeparam name="R">Result type</typeparam>
    /// <param name="list">List of options to match against</param>
    /// <param name="Some">Operation to perform when an Option is in the Some state</param>
    /// <param name="None">Default if the list is empty</param>
    /// <returns>An enumerable of results of the match operations</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<R> Match<T, R>(this IEnumerable<Option<T>> list,
        Func<T, IEnumerable<R>> Some,
        IEnumerable<R> None) =>
        match(list, Some, () => None);
}
