using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.Choice;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for Either
/// </summary>
public static class EitherExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Either<L, R> Flatten<L, R>(this Either<L, Either<L, R>> ma) =>
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
    public static Either<L, R> Plus<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : struct, Num<R> =>
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
    public static Either<L, R> Subtract<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : struct, Num<R> =>
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
    public static Either<L, R> Product<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : struct, Num<R> =>
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
    public static Either<L, R> Divide<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : struct, Num<R> =>
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
    public static Either<L, B> Apply<L, A, B>(this Either<L, Func<A, B>> fab, Either<L, A> fa) =>
        ApplEither<L, A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Either<L, B> Apply<L, A, B>(this Func<A, B> fab, Either<L, A> fa) =>
        ApplEither<L, A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static Either<L, C> Apply<L, A, B, C>(this Either<L, Func<A, B, C>> fabc, Either<L, A> fa, Either<L, B> fb) =>
        from x in fabc
        from y in ApplEither<L, A, B, C>.Inst.Apply(curry(x), fa, fb)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static Either<L, C> Apply<L, A, B, C>(this Func<A, B, C> fabc, Either<L, A> fa, Either<L, B> fb) =>
        ApplEither<L, A, B, C>.Inst.Apply(curry(fabc), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Either<L, Func<B, C>> Apply<L, A, B, C>(this Either<L, Func<A, B, C>> fabc, Either<L, A> fa) =>
        from x in fabc
        from y in ApplEither<L, A, B, C>.Inst.Apply(curry(x), fa)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Either<L, Func<B, C>> Apply<L, A, B, C>(this Func<A, B, C> fabc, Either<L, A> fa) =>
        ApplEither<L, A, B, C>.Inst.Apply(curry(fabc), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Either<L, Func<B, C>> Apply<L, A, B, C>(this Either<L, Func<A, Func<B, C>>> fabc, Either<L, A> fa) =>
        ApplEither<L, A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Either<L, Func<B, C>> Apply<L, A, B, C>(this Func<A, Func<B, C>> fabc, Either<L, A> fa) =>
        ApplEither<L, A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static Either<L, B> Action<L, A, B>(this Either<L, A> fa, Either<L, B> fb) =>
        ApplEither<L, A, B>.Inst.Action(fa, fb);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<L> Lefts<L, R>(this IEnumerable<Either<L, R>> self) =>
        lefts<MEither<L, R>, Either<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<L> Lefts<L, R>(this Seq<Either<L, R>> self) =>
        lefts<MEither<L, R>, Either<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<R> Rights<L, R>(this IEnumerable<Either<L, R>> self) =>
        rights<MEither<L, R>, Either<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<R> Rights<L, R>(this Seq<Either<L, R>> self) =>
        rights<MEither<L, R>, Either<L, R>, L, R>(self);

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
    public static (IEnumerable<L> Lefts, IEnumerable<R> Rights) Partition<L, R>(this IEnumerable<Either<L, R>> self) =>
        partition<MEither<L, R>, Either<L, R>, L, R>(self);

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
    public static (Seq<L> Lefts, Seq<R> Rights) Partition<L, R>(this Seq<Either<L, R>> self) =>
        partition<MEither<L, R>, Either<L, R>, L, R>(self);

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static R Sum<NUM, L, R>(this Either<L, R> self) 
        where NUM : struct, Num<R> =>
        sum<NUM, MEither<L, R>, Either<L, R>, R>(self);

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static int Sum<L>(this Either<L, int> self)=>
        sum<TInt, MEither<L, int>, Either<L, int>, int>(self);

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, R>> ParMap<L, T1, T2, R>(this Either<L, T1> self, Func<T1, T2, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, Func<T3, R>>> ParMap<L, T1, T2, T3, R>(this Either<L, T1> self, Func<T1, T2, T3, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Match the two states of the Either and return a promise of a non-null R2.
    /// </summary>
    public static async Task<R2> MatchAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        Check.NullReturn(self.IsRight
            ? Right(await self.RightValue)
            : Left(self.LeftValue));

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, R> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Task<R2>> map)
    {
        var val = await self;
        return val.IsRight
            ? await map(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, R2> map)
    {
        var val = await self;
        return val.IsRight
            ? map(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, R2> map) =>
        self.IsRight
            ? map(await self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(await self.RightValue)
            : Left<L, R2>(self.LeftValue);


    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, R> self, Func<R, Task<Either<L, R2>>> bind) =>
        self.IsRight
            ? await bind(self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Task<Either<L, R2>>> bind)
    {
        var val = await self;
        return val.IsRight
            ? await bind(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Either<L, R2>> bind)
    {
        var val = await self;
        return val.IsRight
            ? bind(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Either<L, R2>> bind) =>
        self.IsRight
            ? bind(await self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Task<Either<L, R2>>> bind) =>
        self.IsRight
            ? await bind(await self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Unit> IterAsync<L, R>(this Task<Either<L, R>> self, Action<R> action)
    {
        var val = await self;
        if (val.IsRight) action(val.RightValue);
        return unit;
    }

    public static async Task<Unit> IterAsync<L, R>(this Either<L, Task<R>> self, Action<R> action)
    {
        if (self.IsRight) action(await self.RightValue);
        return unit;
    }

    public static async Task<int> CountAsync<L, R>(this Task<Either<L, R>> self) =>
        (await self).Count();

    public static async Task<int> SumAsync<L>(this Task<Either<L, int>> self) =>
        (await self).Sum<TInt, L, int>();

    public static async Task<int> SumAsync<L>(this Either<L, Task<int>> self) =>
        self.IsRight
            ? await self.RightValue
            : 0;

    public static async Task<S> FoldAsync<L, R, S>(this Task<Either<L, R>> self, S state, Func<S, R, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<L, R, S>(this Either<L, Task<R>> self, S state, Func<S, R, S> folder) =>
        self.IsRight
            ? folder(state, await self.RightValue)
            : state;

    public static async Task<bool> ForAllAsync<L, R>(this Task<Either<L, R>> self, Func<R, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<L, R>(this Either<L, Task<R>> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(await self.RightValue)
            : true;

    public static async Task<bool> ExistsAsync<L, R>(this Task<Either<L, R>> self, Func<R, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<L, R>(this Either<L, Task<R>> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(await self.RightValue)
            : false;
}
