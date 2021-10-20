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
/// Extension methods for OptionUnsafe
/// </summary>
public static class OptionUnsafeExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static OptionUnsafe<A> Flatten<A>(this OptionUnsafe<OptionUnsafe<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Extracts from a list of `OptionUnsafe` all the `Some` elements.
    /// All the `Some` elements are extracted in order.
    /// </summary>
    [Pure]
    public static IEnumerable<A> Somes<A>(this IEnumerable<OptionUnsafe<A>> self)
    {
        foreach (var item in self)
        {
            if (item.IsSome)
            {
                yield return item.Value;
            }
        }
    }
    
    /// <summary>
    /// Extracts from a list of `OptionUnsafe` all the `Some` elements.
    /// All the `Some` elements are extracted in order.
    /// </summary>
    [Pure]
    public static Seq<A> Somes<A>(this Seq<OptionUnsafe<A>> self)
    {
        IEnumerable<A> ToSequence(Seq<OptionUnsafe<A>> items)
        {
            foreach (var item in items)
            {
                if (item.IsSome)
                {
                    yield return item.Value;
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
    public static OptionUnsafe<A> Add<NUM, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where NUM : struct, Num<A> =>
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
    /// <returns>An OptionUnsafe with the subtract between x and y</returns>
    [Pure]
    public static OptionUnsafe<A> Subtract<NUM, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where NUM : struct, Num<A> =>
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
    /// <returns>An OptionUnsafe with the product of x and y</returns>
    [Pure]
    public static OptionUnsafe<A> Product<NUM, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where NUM : struct, Num<A> =>
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
    /// <returns>An OptionUnsafe x / y</returns>
    [Pure]
    public static OptionUnsafe<A> Divide<NUM, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where NUM : struct, Num<A> =>
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
    public static OptionUnsafe<B> Apply<A, B>(this OptionUnsafe<Func<A, B>> fab, OptionUnsafe<A> fa) =>
        ApplOptionUnsafe<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static OptionUnsafe<B> Apply<A, B>(this Func<A, B> fab, OptionUnsafe<A> fa) =>
        ApplOptionUnsafe<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static OptionUnsafe<C> Apply<A, B, C>(this OptionUnsafe<Func<A, B, C>> fabc, OptionUnsafe<A> fa, OptionUnsafe<B> fb) =>
        from x in fabc
        from y in ApplOptionUnsafe<A, B, C>.Inst.Apply(curry(x), fa, fb)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static OptionUnsafe<C> Apply<A, B, C>(this Func<A, B, C> fabc, OptionUnsafe<A> fa, OptionUnsafe<B> fb) =>
        ApplOptionUnsafe<A, B, C>.Inst.Apply(curry(fabc), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionUnsafe<Func<B, C>> Apply<A, B, C>(this OptionUnsafe<Func<A, B, C>> fabc, OptionUnsafe<A> fa) =>
        from x in fabc
        from y in ApplOptionUnsafe<A, B, C>.Inst.Apply(curry(x), fa)
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionUnsafe<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, OptionUnsafe<A> fa) =>
        ApplOptionUnsafe<A, B, C>.Inst.Apply(curry(fabc), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionUnsafe<Func<B, C>> Apply<A, B, C>(this OptionUnsafe<Func<A, Func<B, C>>> fabc, OptionUnsafe<A> fa) =>
        ApplOptionUnsafe<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionUnsafe<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, OptionUnsafe<A> fa) =>
        ApplOptionUnsafe<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static OptionUnsafe<B> Action<A, B>(this OptionUnsafe<A> fa, OptionUnsafe<B> fb) =>
        ApplOptionUnsafe<A, B>.Inst.Action(fa, fb);


    /// <summary>
    /// Convert the OptionUnsafe type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">OptionUnsafe to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static A? ToNullable<A>(this OptionUnsafe<A> ma) where A : struct =>
        ma.IsNone
            ? (A?)null
            : ma.Value;

    /// <summary>
    /// Match over a list of OptionUnsafes
    /// </summary>
    /// <typeparam name="T">Type of the bound values</typeparam>
    /// <typeparam name="R">Result type</typeparam>
    /// <param name="list">List of OptionUnsafes to match against</param>
    /// <param name="Some">Operation to perform when an OptionUnsafe is in the Some state</param>
    /// <param name="None">Operation to perform when an OptionUnsafe is in the None state</param>
    /// <returns>An enumerable of results of the match operations</returns>
    [Pure]
    public static IEnumerable<R> Match<T, R>(this IEnumerable<OptionUnsafe<T>> list,
        Func<T, IEnumerable<R>> Some,
        Func<IEnumerable<R>> None
        ) =>
        matchUnsafe(list, Some, None);

    /// <summary>
    /// Match over a list of OptionUnsafes
    /// </summary>
    /// <typeparam name="T">Type of the bound values</typeparam>
    /// <typeparam name="R">Result type</typeparam>
    /// <param name="list">List of OptionUnsafes to match against</param>
    /// <param name="Some">Operation to perform when an Option is in the Some state</param>
    /// <param name="None">Default if the list is empty</param>
    /// <returns>An enumerable of results of the match operations</returns>
    [Pure]
    public static IEnumerable<R> Match<T, R>(this IEnumerable<OptionUnsafe<T>> list,
        Func<T, IEnumerable<R>> Some,
        IEnumerable<R> None) =>
        matchUnsafe(list, Some, () => None);

    /// <summary>
    /// Match the two states of the OptionUnsafe and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this OptionUnsafe<A> ma, Func<A, Task<B>> Some, Func<B> None) =>
        ma.IsSome
            ? await Some(ma.Value).ConfigureAwait(false)
            : None();

    /// <summary>
    /// Match the two states of the OptionUnsafe and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this OptionUnsafe<A> ma, Func<A, Task<B>> Some, Func<Task<B>> None) =>
        ma.IsSome
            ? await Some(ma.Value).ConfigureAwait(false)
            : await None().ConfigureAwait(false);

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="a">OptionUnsafe of int</param>
    /// <returns>The bound value or 0 if None</returns>
    public static int Sum(this OptionUnsafe<int> a) =>
        a.IfNoneUnsafe(0);

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="self">Option of A that is from the type-class NUM</param>
    /// <returns>The bound value or 0 if None</returns>
    public static A Sum<NUM, A>(this OptionUnsafe<A> self)
        where NUM : struct, Num<A> =>
        sum<NUM, MOptionUnsafe<A>, OptionUnsafe<A>, A>(self);

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, Task<B>> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsSome
            ? OptionUnsafe<B>.Some(await map(val.Value).ConfigureAwait(false))
            : None;
    }

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, B> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsSome
            ? OptionUnsafe<B>.Some(map(val.Value))
            : None;
    }

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, B> map) =>
        self.IsSome
            ? OptionUnsafe<B>.Some(map(await self.Value.ConfigureAwait(false)))
            : None;

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, Task<B>> map) =>
        self.IsSome
            ? OptionUnsafe<B>.Some(await map(await self.Value.ConfigureAwait(false)).ConfigureAwait(false))
            : None;


    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this OptionUnsafe<A> self, Func<A, Task<OptionUnsafe<B>>> bind) =>
        self.IsSome
            ? await bind(self.Value).ConfigureAwait(false)
            : None;

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, Task<OptionUnsafe<B>>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsSome
            ? await bind(val.Value).ConfigureAwait(false)
            : None;
    }

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, OptionUnsafe<B>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsSome
            ? bind(val.Value)
            : OptionUnsafe<B>.None;
    }

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, OptionUnsafe<B>> bind) =>
        self.IsSome
            ? bind(await self.Value.ConfigureAwait(false))
            : OptionUnsafe<B>.None;

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, Task<OptionUnsafe<B>>> bind) =>
        self.IsSome
            ? await bind(await self.Value.ConfigureAwait(false)).ConfigureAwait(false)
            : OptionUnsafe<B>.None;

    /// <summary>
    /// Invoke an action for the bound value (if in a Some state)
    /// </summary>
    /// <param name="Some">Action to invoke</param>
    public static async Task<Unit> IterAsync<A>(this Task<OptionUnsafe<A>> self, Action<A> Some)
    {
        var val = await self.ConfigureAwait(false);
        if (val.IsSome) Some(val.Value);
        return unit;
    }

    /// <summary>
    /// Invoke an action for the bound value (if in a Some state)
    /// </summary>
    /// <param name="Some">Action to invoke</param>
    public static async Task<Unit> IterAsync<A>(this OptionUnsafe<Task<A>> self, Action<A> Some)
    {
        if (self.IsSome) Some(await self.Value.ConfigureAwait(false));
        return unit;
    }

    /// <summary>
    /// <para>
    /// Return the number of bound values in this structure:
    /// </para>
    /// <para>
    ///     None = 0
    /// </para>
    /// <para>
    ///     Some = 1
    /// </para>
    /// </summary>
    /// <returns></returns>
    public static async Task<int> CountAsync<A>(this Task<OptionUnsafe<A>> self) =>
        (await self.ConfigureAwait(false)).Count();

    /// <summary>
    /// <para>
    /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para><para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folder function, applied if OptionUnsafe is in a Some state</param>
    /// <returns>The aggregate state</returns>
    public static async Task<S> FoldAsync<A, S>(this Task<OptionUnsafe<A>> self, S state, Func<S, A, S> folder) =>
        (await self.ConfigureAwait(false)).Fold(state, folder);

    /// <summary>
    /// <para>
    /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para><para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folder function, applied if OptionUnsafe is in a Some state</param>
    /// <returns>The aggregate state</returns>
    public static async Task<S> FoldAsync<A, S>(this OptionUnsafe<Task<A>> self, S state, Func<S, A, S> folder) =>
        self.IsSome
            ? folder(state, await self.Value.ConfigureAwait(false))
            : state;

    /// <summary>
    /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
    /// then True is returned (because the predicate applies for-all values).
    /// If the OptionUnsafe is in a Some state the value is the result of running 
    /// applying the bound value to the predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the OptionUnsafe is in a None state then True is returned (because 
    /// the predicate applies for-all values).  If the OptionUnsafe is in a Some state
    /// the value is the result of running applying the bound value to the 
    /// predicate supplied.</returns>
    public static async Task<bool> ForAllAsync<A>(this Task<OptionUnsafe<A>> self, Func<A, bool> pred) =>
        (await self.ConfigureAwait(false)).ForAll(pred);

    /// <summary>
    /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
    /// then True is returned (because the predicate applies for-all values).
    /// If the OptionUnsafe is in a Some state the value is the result of running 
    /// applying the bound value to the predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the OptionUnsafe is in a None state then True is returned (because 
    /// the predicate applies for-all values).  If the OptionUnsafe is in a Some state
    /// the value is the result of running applying the bound value to the 
    /// predicate supplied.</returns>
    public static async Task<bool> ForAllAsync<A>(this OptionUnsafe<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value.ConfigureAwait(false))
            : true;

    /// <summary>
    /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
    /// then True is returned if invoking None returns True.
    /// If the OptionUnsafe is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the OptionUnsafe is in a None state then True is returned if 
    /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    public static async Task<bool> ExistsAsync<A>(this Task<OptionUnsafe<A>> self, Func<A, bool> pred) =>
        (await self.ConfigureAwait(false)).Exists(pred);

    /// <summary>
    /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
    /// then True is returned if invoking None returns True.
    /// If the OptionUnsafe is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the OptionUnsafe is in a None state then True is returned if 
    /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    public static async Task<bool> ExistsAsync<A>(this OptionUnsafe<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value.ConfigureAwait(false))
            : false;
}
