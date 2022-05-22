#nullable enable
using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;

/// <summary>
/// Extension methods for Option
/// </summary>
public static class OptionExtensions
{
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
                #nullable disable
                yield return item.Value;
                #nullable enable
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
                    #nullable disable
                    yield return item.Value;
                    #nullable enable
                }
            }
        }
        return toSeq(ToSequence(self));
    }

    /// <summary>
    /// Add the bound values of x and y, uses an Add type-class to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="ADD">Add of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y added to x</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Add<NUM, A>(this Option<A> x, Option<A> y) where NUM : struct, Num<A> =>
        from a in x
        from b in y
        select plus<NUM, A>(a, b);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="DIFF">Subtract of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the subtract between x and y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Subtract<NUM, A>(this Option<A> x, Option<A> y) where NUM : struct, Num<A> =>
        from a in x
        from b in y
        select subtract<NUM, A>(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product type-class 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="PROD">Product of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the product of x and y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Product<NUM, A>(this Option<A> x, Option<A> y) where NUM : struct, Num<A> =>
        from a in x
        from b in y
        select product<NUM, A>(a, b);

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
    /// operation for type A.  For example x.Divide<TDouble,double>(y)
    /// </summary>
    /// <typeparam name="DIV">Divide of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option x / y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Divide<NUM, A>(this Option<A> x, Option<A> y) where NUM : struct, Num<A> =>
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
        ApplOption<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<B> Apply<A, B>(this Func<A, B> fab, Option<A> fa) =>
        ApplOption<A, B>.Inst.Apply(fab, fa);

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
        from x in fabc
        from y in ApplOption<A, B, C>.Inst.Apply(curry(x), fa, fb)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<C> Apply<A, B, C>(this Func<A, B, C> fabc, Option<A> fa, Option<B> fb) =>
        ApplOption<A, B, C>.Inst.Apply(curry(fabc), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<Func<B, C>> Apply<A, B, C>(this Option<Func<A, B, C>> fabc, Option<A> fa) =>
        from x in fabc
        from y in ApplOption<A, B, C>.Inst.Apply(curry(x), fa)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, Option<A> fa) =>
        ApplOption<A, B, C>.Inst.Apply(curry(fabc), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<Func<B, C>> Apply<A, B, C>(this Option<Func<A, Func<B, C>>> fabc, Option<A> fa) =>
        ApplOption<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, Option<A> fa) =>
        ApplOption<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<B> Action<A, B>(this Option<A> fa, Option<B> fb) =>
        ApplOption<A, B>.Inst.Action(fa, fb);

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
        Func<IEnumerable<R>> None
        ) =>
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

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="a">Option of int</param>
    /// <returns>The bound value or 0 if None</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sum(this Option<int> a) =>
        (int)a;

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="self">Option of A that is from the type-class NUM</param>
    /// <returns>The bound value or 0 if None</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A Sum<NUM, A>(this Option<A> self)
        where NUM : struct, Num<A> =>
        sum<NUM, MOption<A>, Option<A>, A>(self);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static U CheckNullReturn<U>(U value, string location) =>
        isnull(value)
            ? throw new ResultIsNullException($"'{location}' result is null.  Not allowed.")
            : value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static U CheckNullNoneReturn<U>(U value) =>
        CheckNullReturn(value, "None");

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static U CheckNullSomeReturn<U>(U value) =>
        CheckNullReturn(value, "Some");
}
