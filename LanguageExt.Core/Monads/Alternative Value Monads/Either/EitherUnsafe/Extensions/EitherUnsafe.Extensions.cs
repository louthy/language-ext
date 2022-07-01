#nullable enable
using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.Choice;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

/// <summary>
/// Extension methods for `EitherUnsafe`
/// </summary>
public static partial class EitherUnsafeExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, R> Flatten<L, R>(this EitherUnsafe<L, EitherUnsafe<L, R>> ma) =>
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
    public static EitherUnsafe<L, R> Plus<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        RightUnsafe<L, Func<R, R, R>>(default(NUM).Plus).Apply(x).Apply(y);

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
    public static EitherUnsafe<L, R> Subtract<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        RightUnsafe<L, Func<R, R, R>>(default(NUM).Subtract).Apply(x).Apply(y);

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
    public static EitherUnsafe<L, R> Product<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        RightUnsafe<L, Func<R, R, R>>(default(NUM).Product).Apply(x).Apply(y);

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
    public static EitherUnsafe<L, R> Divide<NUM, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
        RightUnsafe<L, Func<R, R, R>>(default(NUM).Divide).Apply(x).Apply(y);

    /// <summary>
    /// Extracts from a list of 'EitherUnsafe' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">EitherUnsafe list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<L> Lefts<L, R>(this IEnumerable<EitherUnsafe<L, R>> self) =>
        self.Choose(e => e.IsLeft && e.LeftValue != null ? Some(e.LeftValue) : None);

    /// <summary>
    /// Extracts from a list of 'EitherUnsafe' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">EitherUnsafe list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<L> Lefts<L, R>(this Seq<EitherUnsafe<L, R>> self) =>
        self.Choose(e => e.IsLeft && e.LeftValue != null ? Some(e.LeftValue) : None).ToSeq();

    /// <summary>
    /// Extracts from a list of 'EitherUnsafe' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">EitherUnsafe list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<R> Rights<L, R>(this IEnumerable<EitherUnsafe<L, R>> self) =>
        self.Choose(e => e.IsRight && e.RightValue != null ? Some(e.RightValue) : None);

    /// <summary>
    /// Extracts from a list of 'EitherUnsafe' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">EitherUnsafe list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<R> Rights<L, R>(this Seq<EitherUnsafe<L, R>> self) =>
        self.Choose(e => e.IsRight && e.RightValue != null ? Some(e.RightValue) : None).ToSeq();

    /// <summary>
    /// Partitions a list of 'EitherUnsafe' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">EitherUnsafe list</param>
    /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
    [Pure]
    public static (IEnumerable<L> Lefts, IEnumerable<R> Rights) Partition<L, R>(
        this IEnumerable<EitherUnsafe<L, R>> self)
    {
        List<L> ls = new();
        List<R> rs = new();

        foreach (var e in self)
        {
            switch (e)
            {
                case {IsRight:true, RightValue: not null} x: rs.Add(x.RightValue);
                    break;
                case {IsLeft:true, LeftValue: not null} x: ls.Add(x.LeftValue);
                    break;
            }
        }

        return (ls, rs);
    }

    /// <summary>
    /// Partitions a list of 'EitherUnsafe' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">EitherUnsafe list</param>
    /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
    [Pure]
    public static (Seq<L> Lefts, Seq<R> Rights) Partition<L, R>(this Seq<EitherUnsafe<L, R>> self)
    {
        List<L> ls = new();
        List<R> rs = new();

        foreach (var e in self)
        {
            switch (e)
            {
                case {IsRight:true, RightValue: not null} x: rs.Add(x.RightValue);
                    break;
                case {IsLeft:true, LeftValue: not null} x: ls.Add(x.LeftValue);
                    break;
            }
        }

        return (ls.ToSeq(), rs.ToSeq());
    }

    /// <summary>
    /// Sum of the EitherUnsafe
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">EitherUnsafe to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static R Sum<NUM, L, R>(this EitherUnsafe<L, R> self) 
        where NUM : struct, Num<R> =>
        sum<NUM, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, R>(self);

    /// <summary>
    /// Sum of the EitherUnsafe
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">EitherUnsafe to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static int Sum<L>(this EitherUnsafe<L, int> self)=>
        sum<TInt, MEitherUnsafe<L, int>, EitherUnsafe<L, int>, int>(self);

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, Func<T2?, R?>> ParMap<L, T1, T2, R>(this EitherUnsafe<L, T1> self, Func<T1?, T2?, R?> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, Func<T2?, Func<T3?, R?>>> ParMap<L, T1, T2, T3, R>(this EitherUnsafe<L, T1> self, Func<T1?, T2?, T3?, R?> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Match the two states of the EitherUnsafe and return a promise of a non-null R2.
    /// </summary>
    public static async Task<R2?> MatchAsync<L, R, R2>(this EitherUnsafe<L, Task<R?>> self, Func<R?, R2?> Right, Func<L?, R2?> Left) =>
        Check.NullReturn(self.IsRight
            ? Right(self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false))
            : Left(self.LeftValue));

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, R> self, Func<R?, Task<R2?>> map) =>
        self.IsRight
            ? await map(self.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R?, Task<R2?>> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? await map(val.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R?, R2?> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? map(val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R?, R2?> map) =>
        self.IsRight
            ? map(self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false))
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R?, Task<R2?>> map) =>
        self.IsRight
            ? await map(self.RightValue is null ? default : await self.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(self.LeftValue);


    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, R> self, Func<R?, Task<EitherUnsafe<L, R2>>> bind) =>
        self.IsRight
            ? await bind(self.RightValue is null ? default : self.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R?, Task<EitherUnsafe<L, R2>>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? await bind(val.RightValue is null ? default : val.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R?, EitherUnsafe<L, R2>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? bind(val.RightValue is null ? default : val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R?, EitherUnsafe<L, R2>> bind) =>
        self.IsRight
            ? bind(self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false))
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R?, Task<EitherUnsafe<L, R2>>> bind) =>
        self.IsRight
            ? await bind(self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false)).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<Unit> IterAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Action<R?> action)
    {
        var val = await self.ConfigureAwait(false);
        if (val.IsRight) action(val.RightValue);
        return unit;
    }

    public static async Task<Unit> IterAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Action<R?> action)
    {
        if (self.IsRight) action(self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false));
        return unit;
    }

    public static async Task<int> CountAsync<L, R>(this Task<EitherUnsafe<L, R>> self) =>
        (await self.ConfigureAwait(false)).Count();

    public static async Task<int> SumAsync<L>(this Task<EitherUnsafe<L, int>> self) =>
        (await self.ConfigureAwait(false)).Sum<TInt, L, int>();

    public static async Task<int> SumAsync<L>(this EitherUnsafe<L, Task<int>> self) =>
        self.IsRight
            ? self.RightValue is null ? 0 : await self.RightValue.ConfigureAwait(false)
            : 0;

    public static async Task<S?> FoldAsync<L, R, S>(this Task<EitherUnsafe<L, R>> self, S? state, Func<S?, R?, S?> folder) =>
        (await self.ConfigureAwait(false)).Fold(state, folder);

    public static async Task<S?> FoldAsync<L, R, S>(this EitherUnsafe<L, Task<R>> self, S? state, Func<S?, R?, S?> folder) =>
        self.IsRight
            ? folder(state, self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false))
            : state;

    public static async Task<bool> ForAllAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Func<R?, bool> pred) =>
        (await self.ConfigureAwait(false)).ForAll(pred);

    public static async Task<bool> ForAllAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Func<R?, bool> pred) =>
        self.IsRight && pred(self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false));

    public static async Task<bool> ExistsAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Func<R?, bool> pred) =>
        (await self.ConfigureAwait(false)).Exists(pred);

    public static async Task<bool> ExistsAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Func<R?, bool> pred) =>
        self.IsRight && pred(self.RightValue is null ? default : await self.RightValue.ConfigureAwait(false));
}
