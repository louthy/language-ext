using System;
using System.Linq;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for OptionAsync
/// </summary>
public static partial class OptionAsyncExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static OptionAsync<A> Flatten<A>(this OptionAsync<OptionAsync<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Extracts from a list of `Option` all the `Some` elements.
    /// All the `Some` elements are extracted in order.
    /// </summary>
    [Pure]
    public static async Task<IEnumerable<A>> Somes<A>(this IEnumerable<OptionAsync<A>> self)
    {
        var res = await self.Map(o => o.Data)
                            .WindowMap(identity)
                            .ConfigureAwait(false);
        return res.Filter(x => x.IsSome).Map(x => x.Value).ToArray();
    }
    

    /// <summary>
    /// Extracts from a list of `OptionAsync` all the `Some` elements.
    /// All the `Some` elements are extracted in order.
    /// </summary>
    [Pure]
    public static async Task<Seq<A>> Somes<A>(this Seq<OptionAsync<A>> self)
    {
        var res = await self.Map(o => o.Data)
                            .WindowMap(identity)
                            .ConfigureAwait(false);
        return res.Filter(x => x.IsSome).Map(x => x.Value).ToSeq();
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
    public static OptionAsync<A> Add<NUM, A>(this OptionAsync<A> x, OptionAsync<A> y) where NUM : struct, Num<A> =>
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
    public static OptionAsync<A> Subtract<NUM, A>(this OptionAsync<A> x, OptionAsync<A> y) where NUM : struct, Num<A> =>
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
    public static OptionAsync<A> Product<NUM, A>(this OptionAsync<A> x, OptionAsync<A> y) where NUM : struct, Num<A> =>
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
    public static OptionAsync<A> Divide<NUM, A>(this OptionAsync<A> x, OptionAsync<A> y) where NUM : struct, Num<A> =>
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
    public static OptionAsync<B> Apply<A, B>(this OptionAsync<Func<A, B>> fab, OptionAsync<A> fa) =>
        ApplOptionAsync<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static OptionAsync<B> Apply<A, B>(this Func<A, B> fab, OptionAsync<A> fa) =>
        ApplOptionAsync<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static OptionAsync<C> Apply<A, B, C>(this OptionAsync<Func<A, B, C>> fabc, OptionAsync<A> fa, OptionAsync<B> fb) =>
        from x in fabc
        from y in ApplOptionAsync<A, B, C>.Inst.Apply(curry(x), fa, fb)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static OptionAsync<C> Apply<A, B, C>(this Func<A, B, C> fabc, OptionAsync<A> fa, OptionAsync<B> fb) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(curry(fabc), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> Apply<A, B, C>(this OptionAsync<Func<A, B, C>> fabc, OptionAsync<A> fa) =>
        from x in fabc
        from y in ApplOptionAsync<A, B, C>.Inst.Apply(curry(x), fa)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, OptionAsync<A> fa) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(curry(fabc), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> Apply<A, B, C>(this OptionAsync<Func<A, Func<B, C>>> fabc, OptionAsync<A> fa) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, OptionAsync<A> fa) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type OptionAsync<B></returns>
    [Pure]
    public static OptionAsync<B> Action<A, B>(this OptionAsync<A> fa, OptionAsync<B> fb) =>
        ApplOptionAsync<A, B>.Inst.Action(fa, fb);

    /// <summary>
    /// Convert the Option type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Option to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static async Task<A?> ToNullable<A>(this OptionAsync<A> ma) where A : struct =>
        (await ma.IsNone.ConfigureAwait(false))
            ? (A?)null
            : await ma.Value.ConfigureAwait(false);

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
    public static Task<IEnumerable<R>> Match<T, R>(this IEnumerable<OptionAsync<T>> list,
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
    public static Task<IEnumerable<R>> Match<T, R>(this IEnumerable<OptionAsync<T>> list,
        Func<T, IEnumerable<R>> Some,
        IEnumerable<R> None) =>
        match(list, Some, () => None);

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="a">Option of int</param>
    /// <returns>The bound value or 0 if None</returns>
    public static Task<int> Sum(this OptionAsync<int> a) =>
        a.IfNone(0);

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="self">Option of A that is from the type-class NUM</param>
    /// <returns>The bound value or 0 if None</returns>
    public static Task<A> Sum<NUM, A>(this OptionAsync<A> self)
        where NUM : struct, Num<A> =>
        sumAsync<NUM, MOptionAsync<A>, OptionAsync<A>, A>(self);
}
