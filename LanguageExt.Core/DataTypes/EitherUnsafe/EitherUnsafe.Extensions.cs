using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.ChoiceUnsafe;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

/// <summary>
/// Extension methods for Either
/// </summary>
public static class EitherUnsafeExtensions
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
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static EitherUnsafe<L, B> Apply<L, A, B>(this EitherUnsafe<L, Func<A, B>> fab, EitherUnsafe<L, A> fa) =>
        ApplEitherUnsafe<L, A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static EitherUnsafe<L, B> Apply<L, A, B>(this Func<A, B> fab, EitherUnsafe<L, A> fa) =>
        ApplEitherUnsafe<L, A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static EitherUnsafe<L, C> Apply<L, A, B, C>(this EitherUnsafe<L, Func<A, B, C>> fabc, EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
        from x in fabc
        from y in ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(x), fa, fb)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static EitherUnsafe<L, C> Apply<L, A, B, C>(this Func<A, B, C> fabc, EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
        ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(fabc), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherUnsafe<L, Func<B, C>> Apply<L, A, B, C>(this EitherUnsafe<L, Func<A, B, C>> fabc, EitherUnsafe<L, A> fa) =>
        from x in fabc
        from y in ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(x), fa)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherUnsafe<L, Func<B, C>> Apply<L, A, B, C>(this Func<A, B, C> fabc, EitherUnsafe<L, A> fa) =>
        ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(fabc), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherUnsafe<L, Func<B, C>> Apply<L, A, B, C>(this EitherUnsafe<L, Func<A, Func<B, C>>> fabc, EitherUnsafe<L, A> fa) =>
        ApplEitherUnsafe<L, A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static EitherUnsafe<L, B> Action<L, A, B>(this EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
        ApplEitherUnsafe<L, A, B>.Inst.Action(fa, fb);

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
        lefts<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<L> Lefts<L, R>(this Seq<EitherUnsafe<L, R>> self) =>
        lefts<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

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
        rights<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<R> Rights<L, R>(this Seq<EitherUnsafe<L, R>> self) =>
        rights<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

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
    public static (IEnumerable<L> Lefts, IEnumerable<R> Rights) Partition<L, R>(this IEnumerable<EitherUnsafe<L, R>> self) =>
        partition<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

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
    public static (Seq<L> Lefts, Seq<R> Rights) Partition<L, R>(this Seq<EitherUnsafe<L, R>> self) =>
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
    [Pure]
    public static EitherUnsafe<L, Func<T2, R>> ParMap<L, T1, T2, R>(this EitherUnsafe<L, T1> self, Func<T1, T2, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, Func<T2, Func<T3, R>>> ParMap<L, T1, T2, T3, R>(this EitherUnsafe<L, T1> self, Func<T1, T2, T3, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Match the two states of the Either and return a promise of a non-null R2.
    /// </summary>
    public static async Task<R2> MatchAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        Check.NullReturn(self.IsRight
            ? Right(await self.RightValue.ConfigureAwait(false))
            : Left(self.LeftValue));


    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, R> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(self.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, Task<R2>> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? await map(val.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, R2> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? map(val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, R2> map) =>
        self.IsRight
            ? map(await self.RightValue.ConfigureAwait(false))
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> MapAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(await self.RightValue.ConfigureAwait(false)).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(self.LeftValue);


    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, R> self, Func<R, Task<EitherUnsafe<L, R2>>> bind) =>
        self.IsRight
            ? await bind(self.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, Task<EitherUnsafe<L, R2>>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? await bind(val.RightValue).ConfigureAwait(false)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this Task<EitherUnsafe<L, R>> self, Func<R, EitherUnsafe<L, R2>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? bind(val.RightValue)
            : LeftUnsafe<L, R2>(val.LeftValue);
    }

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, EitherUnsafe<L, R2>> bind) =>
        self.IsRight
            ? bind(await self.RightValue.ConfigureAwait(false))
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<EitherUnsafe<L, R2>> BindAsync<L, R, R2>(this EitherUnsafe<L, Task<R>> self, Func<R, Task<EitherUnsafe<L, R2>>> bind) =>
        self.IsRight
            ? await bind(await self.RightValue.ConfigureAwait(false))
            : LeftUnsafe<L, R2>(self.LeftValue);

    public static async Task<Unit> IterAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Action<R> action)
    {
        var val = await self.ConfigureAwait(false);
        if (val.IsRight) action(val.RightValue);
        return unit;
    }

    public static async Task<Unit> IterAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Action<R> action)
    {
        if (self.IsRight) action(await self.RightValue.ConfigureAwait(false));
        return unit;
    }

    public static async Task<int> CountAsync<L, R>(this Task<EitherUnsafe<L, R>> self) =>
        (await self.ConfigureAwait(false)).Count();

    public static async Task<int> SumAsync<L>(this Task<EitherUnsafe<L, int>> self) =>
        (await self.ConfigureAwait(false)).Sum<TInt, L, int>();

    public static async Task<int> SumAsync<L>(this EitherUnsafe<L, Task<int>> self) =>
        self.IsRight
            ? await self.RightValue.ConfigureAwait(false)
            : 0;

    public static async Task<S> FoldAsync<L, R, S>(this Task<EitherUnsafe<L, R>> self, S state, Func<S, R, S> folder) =>
        (await self.ConfigureAwait(false)).Fold(state, folder);

    public static async Task<S> FoldAsync<L, R, S>(this EitherUnsafe<L, Task<R>> self, S state, Func<S, R, S> folder) =>
        self.IsRight
            ? folder(state, await self.RightValue.ConfigureAwait(false))
            : state;

    public static async Task<bool> ForAllAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Func<R, bool> pred) =>
        (await self.ConfigureAwait(false)).ForAll(pred);

    public static async Task<bool> ForAllAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Func<R, bool> pred) =>
        !self.IsRight || pred(await self.RightValue.ConfigureAwait(false));

    public static async Task<bool> ExistsAsync<L, R>(this Task<EitherUnsafe<L, R>> self, Func<R, bool> pred) =>
        (await self.ConfigureAwait(false)).Exists(pred);

    public static async Task<bool> ExistsAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Func<R, bool> pred) =>
        self.IsRight && pred(await self.RightValue.ConfigureAwait(false));

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this EitherUnsafe<Error, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => SuccessEff<R>(ma.RightValue),
            EitherStatus.IsLeft  => FailEff<R>(ma.LeftValue),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Aff
    /// </summary>
    /// <returns>Aff monad</returns>
    [Pure]
    public static Aff<R> ToAff<R>(this EitherUnsafe<Error, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => SuccessAff<R>(ma.RightValue),
            EitherStatus.IsLeft  => FailAff<R>(ma.LeftValue),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this EitherUnsafe<Exception, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => SuccessEff<R>(ma.RightValue),
            EitherStatus.IsLeft  => FailEff<R>(ma.LeftValue),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Aff
    /// </summary>
    /// <returns>Aff monad</returns>
    [Pure]
    public static Aff<R> ToAff<R>(this EitherUnsafe<Exception, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => SuccessAff<R>(ma.RightValue),
            EitherStatus.IsLeft  => FailAff<R>(ma.LeftValue),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this EitherUnsafe<string, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => SuccessEff<R>(ma.RightValue),
            EitherStatus.IsLeft  => FailEff<R>(Error.New(ma.LeftValue)),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Aff
    /// </summary>
    /// <returns>Aff monad</returns>
    [Pure]
    public static Aff<R> ToAff<R>(this EitherUnsafe<string, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => SuccessAff<R>(ma.RightValue),
            EitherStatus.IsLeft  => FailAff<R>(Error.New(ma.LeftValue)),
            _                    => default // bottom
        };
}
