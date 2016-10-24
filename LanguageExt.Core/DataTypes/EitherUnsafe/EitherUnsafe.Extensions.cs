using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;

/// <summary>
/// Extension methods for Either
/// </summary>
public static class EitherUnsafeExtensions
{
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
    public static EitherUnsafe<L, R> Plus<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Plus(a, b);

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
    public static EitherUnsafe<L, R> Subtract<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Subtract(a, b);

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
    public static EitherUnsafe<L, R> Product<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Product(a, b);

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
    public static EitherUnsafe<L, R> Divide<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        from a in x
        from b in y
        select default(NUM).Divide(a, b);

    /// <summary>
    /// Apply
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, B> Apply<L, A, B>(this EitherUnsafe<L, Func<A, B>> x, EitherUnsafe<L, A> y) =>
        apply<MEitherUnsafe<L, Func<A, B>>, MEitherUnsafe<L, A>, MEitherUnsafe<L, B>, EitherUnsafe<L, Func<A, B>>, EitherUnsafe<L, A>, EitherUnsafe<L, B>, A, B>(x, y);

    /// <summary>
    /// Apply
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, C> Apply<L, A, B, C>(this EitherUnsafe<L, Func<A, B, C>> x, EitherUnsafe<L, A> y, EitherUnsafe<L, B> z) =>
        apply<MEitherUnsafe<L, Func<A, B, C>>, MEitherUnsafe<L, A>, MEitherUnsafe<L, B>, MEitherUnsafe<L, C>, EitherUnsafe<L, Func<A, B, C>>, EitherUnsafe<L, A>, EitherUnsafe<L, B>, EitherUnsafe<L, C>, A, B, C>(x, y, z);

    /// <summary>
    /// Apply y to x
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, Func<B, C>> Apply<L, A, B, C>(this EitherUnsafe<L, Func<A, B, C>> x, EitherUnsafe<L, A> y) =>
        apply<MEitherUnsafe<L, Func<A, B, C>>, MEitherUnsafe<L, A>, MEitherUnsafe<L, Func<B, C>>, EitherUnsafe<L, Func<A, B, C>>, EitherUnsafe<L, A>, EitherUnsafe<L, Func<B, C>>, A, B, C>(x, y);

    /// <summary>
    /// Apply
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, Func<B, C>> Apply<L, A, B, C>(this EitherUnsafe<L, Func<A, Func<B, C>>> x, EitherUnsafe<L, A> y) =>
        apply2<MEitherUnsafe<L, Func<A, Func<B, C>>>, MEitherUnsafe<L, A>, MEitherUnsafe<L, Func<B, C>>, EitherUnsafe<L, Func<A, Func<B, C>>>, EitherUnsafe<L, A>, EitherUnsafe<L, Func<B, C>>, A, B, C>(x, y);

    /// <summary>
    /// Apply x, then y, ignoring the result of x
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, B> Action<L, A, B>(this EitherUnsafe<L, A> x, EitherUnsafe<L, B> y) =>
        action<MEitherUnsafe<L, A>, MEitherUnsafe<L, B>, EitherUnsafe<L, A>, EitherUnsafe<L, B>, A, B>(x, y);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<L> Lefts<L, R>(this IEnumerable<EitherUnsafe<L, R>> self) =>
        choice1s<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<R> Rights<L, R>(this IEnumerable<EitherUnsafe<L, R>> self) =>
        choice2s<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

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
    public static Tuple<IEnumerable<L>, IEnumerable<R>> Partition<L, R>(this IEnumerable<EitherUnsafe<L, R>> self) =>
        partition<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static R Sum<NUM, L, R>(this EitherUnsafe<L, R> self)
        where NUM : struct, Num<R> =>
        sum<NUM, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, R>(self);

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static int Sum<L>(this EitherUnsafe<L, int> self) =>
        sum<TInt, MEitherUnsafe<L, int>, EitherUnsafe<L, int>, int>(self);

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static EitherUnsafe<L, Func<T2, R>> ParMap<L, T1, T2, R>(this EitherUnsafe<L, T1> self, Func<T1, T2, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static EitherUnsafe<L, Func<T2, Func<T3, R>>> ParMap<L, T1, T2, T3, R>(this EitherUnsafe<L, T1> self, Func<T1, T2, T3, R> func) =>
        self.Map(curry(func));

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EitherUnsafe<L, V> SelectMany<L, T, U, V>(this IEnumerable<T> self,
        Func<T, EitherUnsafe<L, U>> bind,
        Func<T, U, V> project
        )
    {
        var ta = self.Take(1).ToArray();
        if (ta.Length == 0) return EitherUnsafe<L, V>.Bottom;
        var u = bind(ta[0]);
        if (u.IsBottom) return EitherUnsafe<L, V>.Bottom;
        if (u.IsLeft) return EitherUnsafe<L, V>.Left(u.LeftValue);
        return project(ta[0], u.RightValue);
    }

    /// <summary>
    /// Match the two states of the Either and return a promise of a non-null R2.
    /// </summary>
    public static async Task<R2> MatchAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        Check.NullReturn(self.IsRight
            ? Right(await self.RightValue)
            : Left(self.LeftValue));

    /// <summary>
    /// Match the two states of the Either and return a stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this EitherUnsafe<L, IObservable<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        self.IsRight
            ? self.RightValue.Select(Right).Select(Check.NullReturn)
            : Observable.Return(Check.NullReturn(Left(self.LeftValue)));

    /// <summary>
    /// Match the two states of the IObservable Either and return a stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this IObservable<EitherUnsafe<L, R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        self.Select(either => matchUnsafe(either, Right, Left));

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, R> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(self.RightValue)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, Task<R2>> map)
    {
        var val = await self;
        return val.IsRight
            ? await map(val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, R2> map)
    {
        var val = await self;
        return val.IsRight
            ? map(val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, R2> map) =>
        self.IsRight
            ? map(await self.RightValue)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(await self.RightValue)
            : LeftUnsafe<L, R2>(self.LeftValue);


    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, R> self, Func<R, Task<EitherUnsafe<L, R2>>> bind) =>
        self.IsRight
            ? await bind(self.RightValue)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, Task<EitherUnsafe<L, R2>>> bind)
    {
        var val = await self;
        return val.IsRight
            ? await bind(val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, EitherUnsafe<L, R2>> bind)
    {
        var val = await self;
        return val.IsRight
            ? bind(val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, EitherUnsafe<L, R2>> bind) =>
        self.IsRight
            ? bind(await self.RightValue)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, Task<EitherUnsafe<L, R2>>> bind) =>
        self.IsRight
            ? await bind(await self.RightValue)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<Unit> IterAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Action<R> action)
    {
        var val = await self;
        if (val.IsRight) action(val.RightValue);
        return unit;
    }

    public static async Task<Unit> IterAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Action<R> action)
    {
        if (self.IsRight) action(await self.RightValue);
        return unit;
    }

    public static async Task<int> CountAsync<L, R>(this Task<EitherUnsafe<L, R>> self) =>
        (await self).Count();

    public static async Task<int> SumAsync<L>(this Task<EitherUnsafe<L, int>> self) =>
        (await self).Sum<TInt, L, int>();

    public static async Task<int> SumAsync<L>(this EitherUnsafe<L, Task<int>> self) =>
        self.IsRight
            ? await self.RightValue
            : 0;

    public static async Task<S> FoldAsync<L, R, S>(this Task<EitherUnsafe<L, R>> self, S state, Func<S, R, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<L, R, S>(this EitherUnsafe<L, Task<R>> self, S state, Func<S, R, S> folder) =>
        self.IsRight
            ? folder(state, await self.RightValue)
            : state;

    public static async Task<bool> ForAllAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Func<R, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(await self.RightValue)
            : true;

    public static async Task<bool> ExistsAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Func<R, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(await self.RightValue)
            : false;
}