using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for Either
/// </summary>
public static class EitherAsyncExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static EitherAsync<L, R> Flatten<L, R>(this EitherAsync<L, EitherAsync<L, R>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Add the bound values of x and y, uses an Add type-class to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y added to x</returns>
    [Pure]
    public static EitherAsync<L, R> Plus<NUM, L, R>(this EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Plus(a, b);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the subtract between x and y</returns>
    [Pure]
    public static EitherAsync<L, R> Subtract<NUM, L, R>(this EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Subtract(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product type-class 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the product of x and y</returns>
    [Pure]
    public static EitherAsync<L, R> Product<NUM, L, R>(this EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Product(a, b);

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
    /// operation for type A.  For example x.Divide<TDouble,double>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option x / y</returns>
    [Pure]
    public static EitherAsync<L, R> Divide<NUM, L, R>(this EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Divide(a, b);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static EitherAsync<L, B> Apply<L, A, B>(this EitherAsync<L, Func<A, B>> fab, EitherAsync<L, A> fa) =>
        default(ApplEitherAsync<L, A, B>).Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static EitherAsync<L, B> Apply<L, A, B>(this Func<A, B> fab, EitherAsync<L, A> fa) =>
        default(ApplEitherAsync<L, A, B>).Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static EitherAsync<L, C> Apply<L, A, B, C>(this EitherAsync<L, Func<A, B, C>> fabc, EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
        from x in fabc
        from y in default(ApplEitherAsync<L, A, B, C>).Apply(curry(x), fa, fb)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static EitherAsync<L, C> Apply<L, A, B, C>(this Func<A, B, C> fabc, EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(curry(fabc), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this EitherAsync<L, Func<A, B, C>> fabc, EitherAsync<L, A> fa) =>
        from x in fabc
        from y in default(ApplEitherAsync<L, A, B, C>).Apply(curry(x), fa)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this Func<A, B, C> fabc, EitherAsync<L, A> fa) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(curry(fabc), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this EitherAsync<L, Func<A, Func<B, C>>> fabc, EitherAsync<L, A> fa) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this Func<A, Func<B, C>> fabc, EitherAsync<L, A> fa) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static EitherAsync<L, B> Action<L, A, B>(this EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
        default(ApplEitherAsync<L, A, B>).Action(fa, fb);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Task<IEnumerable<L>> Lefts<L, R>(this IEnumerable<EitherAsync<L, R>> self) =>
        leftsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Task<Seq<L>> Lefts<L, R>(this Seq<EitherAsync<L, R>> self) =>
        leftsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Task<IEnumerable<R>> Rights<L, R>(this IEnumerable<EitherAsync<L, R>> self) =>
        rightsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Task<Seq<R>> Rights<L, R>(this Seq<EitherAsync<L, R>> self) =>
        rightsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

    /// <summary>
    /// Partitions a list of 'Either' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
    [Pure]
    public static Task<(Seq<L> Lefts, Seq<R> Rights)> Partition<L, R>(this Seq<EitherAsync<L, R>> self) =>
        partitionAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

    /// <summary>
    /// Partitions a list of 'Either' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
    [Pure]
    public static Task<(IEnumerable<L> Lefts, IEnumerable<R> Rights)> Partition<L, R>(this IEnumerable<EitherAsync<L, R>> self) =>
        partitionAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static Task<R> Sum<NUM, L, R>(this EitherAsync<L, R> self) 
        where NUM : struct, Num<R> =>
            self.Match(x => x, _ => default(NUM).Empty(), () => default(NUM).Empty());

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static Task<int> Sum<L>(this EitherAsync<L, int> self) =>
        self.Match(x => x, _ => 0, () => 0);

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    public static EitherAsync<L, Func<T2, R>> ParMap<L, T1, T2, R>(this EitherAsync<L, T1> self, Func<T1, T2, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    public static EitherAsync<L, Func<T2, Func<T3, R>>> ParMap<L, T1, T2, T3, R>(this EitherAsync<L, T1> self, Func<T1, T2, T3, R> func) =>
        self.Map(curry(func));
}
