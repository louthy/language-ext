using System;
using System.Linq;
using System.Reactive.Linq;
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
/// Extension methods for Option
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Extracts from a list of 'Option' all the 'Some' elements.
    /// All the 'Some' elements are extracted in order.
    /// </summary>
    [Pure]
    public static IEnumerable<A> Somes<A>(this IEnumerable<Option<A>> self)
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
    /// Add the bound values of x and y, uses an Add type-class to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="ADD">Add of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y added to x</returns>
    [Pure]
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
    public static Option<B> Apply<A, B>(this Option<Func<A, B>> fab, Option<A> fa) =>
        ApplOption<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
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
    public static Option<C> Apply<A, B, C>(this Func<A, B, C> fabc, Option<A> fa, Option<B> fb) =>
        ApplOption<A, B, C>.Inst.Apply(curry(fabc), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
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
    public static Option<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, Option<A> fa) =>
        ApplOption<A, B, C>.Inst.Apply(curry(fabc), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Option<Func<B, C>> Apply<A, B, C>(this Option<Func<A, Func<B, C>>> fabc, Option<A> fa) =>
        ApplOption<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Option<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, Option<A> fa) =>
        ApplOption<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static Option<B> Action<A, B>(this Option<A> fa, Option<B> fb) =>
        ApplOption<A, B>.Inst.Action(fa, fb);

    /// <summary>
    /// Convert the Option type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Option to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static A? ToNullable<A>(this Option<A> ma) where A : struct =>
        ma.IsNone
            ? (A?)null
            : ma.Value;

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
    public static IEnumerable<R> Match<T, R>(this IEnumerable<Option<T>> list,
        Func<T, IEnumerable<R>> Some,
        IEnumerable<R> None) =>
        match(list, Some, () => None);

    /// <summary>
    /// Match the two states of the Option and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this Option<A> ma, Func<A, Task<B>> Some, Func<B> None) =>
        ma.IsSome
            ? CheckNullSomeReturn(await Some(ma.Value))
            : CheckNullNoneReturn(None());

    /// <summary>
    /// Match the two states of the Option and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this Option<A> ma, Func<A, Task<B>> Some, Func<Task<B>> None) =>
        ma.IsSome
            ? CheckNullSomeReturn(await Some(ma.Value))
            : CheckNullNoneReturn(await None());

    /// <summary>
    /// Match the two states of the Option and return an observable stream of non-null Rs.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A stream of non-null Rs</returns>
    [Pure]
    public static IObservable<B> MatchObservable<A, B>(this Option<A> ma, Func<A, IObservable<B>> Some, Func<B> None) =>
        ma.IsSome
            ? Some(ma.Value).Select(CheckNullSomeReturn)
            : Observable.Return(CheckNullNoneReturn(None()));

    /// <summary>
    /// Match the two states of the Option and return an observable stream of non-null Rs.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A stream of non-null Rs</returns>
    [Pure]
    public static IObservable<B> MatchObservable<A, B>(this Option<A> ma, Func<A, IObservable<B>> Some, Func<IObservable<B>> None) =>
        ma.IsSome
            ? Some(ma.Value).Select(CheckNullSomeReturn)
            : None().Select(CheckNullNoneReturn);

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="a">Option of int</param>
    /// <returns>The bound value or 0 if None</returns>
    public static int Sum(this Option<int> a) =>
        a.IfNone(0);

    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="self">Option of A that is from the type-class NUM</param>
    /// <returns>The bound value or 0 if None</returns>
    public static A Sum<NUM, A>(this Option<A> self)
        where NUM : struct, Num<A> =>
        sum<NUM, MOption<A>, Option<A>, A>(self);

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<Option<B>> MapAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> map)
    {
        var val = await self;
        return val.IsSome
            ? Optional(await map(val.Value))
            : None;
    }

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<Option<B>> MapAsync<A, B>(this Task<Option<A>> self, Func<A, B> map)
    {
        var val = await self;
        return val.IsSome
            ? Optional(map(val.Value))
            : None;
    }

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<Option<B>> MapAsync<A, B>(this Option<Task<A>> self, Func<A, B> map) =>
        self.IsSome
            ? Optional(map(await self.Value))
            : None;

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="map">Projection function</param>
    /// <returns>Mapped functor</returns>
    public static async Task<Option<B>> MapAsync<A, B>(this Option<Task<A>> self, Func<A, Task<B>> map) =>
        self.IsSome
            ? Optional(await map(await self.Value))
            : None;


    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<Option<B>> BindAsync<A, B>(this Option<A> self, Func<A, Task<Option<B>>> bind) =>
        self.IsSome
            ? await bind(self.Value)
            : None;

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<Option<B>> BindAsync<A, B>(this Task<Option<A>> self, Func<A, Task<Option<B>>> bind)
    {
        var val = await self;
        return val.IsSome
            ? await bind(val.Value)
            : None;
    }

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<Option<B>> BindAsync<A, B>(this Task<Option<A>> self, Func<A, Option<B>> bind)
    {
        var val = await self;
        return val.IsSome
            ? bind(val.Value)
            : Option<B>.None;
    }

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<Option<B>> BindAsync<A, B>(this Option<Task<A>> self, Func<A, Option<B>> bind) =>
        self.IsSome
            ? bind(await self.Value)
            : Option<B>.None;

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static async Task<Option<B>> BindAsync<A, B>(this Option<Task<A>> self, Func<A, Task<Option<B>>> bind) =>
        self.IsSome
            ? await bind(await self.Value)
            : Option<B>.None;

    /// <summary>
    /// Invoke an action for the bound value (if in a Some state)
    /// </summary>
    /// <param name="Some">Action to invoke</param>
    public static async Task<Unit> IterAsync<A>(this Task<Option<A>> self, Action<A> Some)
    {
        var val = await self;
        if (val.IsSome) Some(val.Value);
        return unit;
    }

    /// <summary>
    /// Invoke an action for the bound value (if in a Some state)
    /// </summary>
    /// <param name="Some">Action to invoke</param>
    public static async Task<Unit> IterAsync<A>(this Option<Task<A>> self, Action<A> Some)
    {
        if (self.IsSome) Some(await self.Value);
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
    public static async Task<int> CountAsync<A>(this Task<Option<A>> self) =>
        (await self).Count();

    /// <summary>
    /// <para>
    /// Option types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// <para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folder function, applied if Option is in a Some state</param>
    /// <returns>The aggregate state</returns>
    public static async Task<S> FoldAsync<A, S>(this Task<Option<A>> self, S state, Func<S, A, S> folder) =>
        (await self).Fold(state, folder);

    /// <summary>
    /// <para>
    /// Option types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// <para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folder function, applied if Option is in a Some state</param>
    /// <returns>The aggregate state</returns>
    public static async Task<S> FoldAsync<A, S>(this Option<Task<A>> self, S state, Func<S, A, S> folder) =>
        self.IsSome
            ? folder(state, await self.Value)
            : state;

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned (because the predicate applies for-all values).
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then True is returned (because 
    /// the predicate applies for-all values).  If the Option is in a Some state
    /// the value is the result of running applying the bound value to the 
    /// predicate supplied.</returns>
    public static async Task<bool> ForAllAsync<A>(this Task<Option<A>> self, Func<A, bool> pred) =>
        (await self).ForAll(pred);

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned (because the predicate applies for-all values).
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then True is returned (because 
    /// the predicate applies for-all values).  If the Option is in a Some state
    /// the value is the result of running applying the bound value to the 
    /// predicate supplied.</returns>
    public static async Task<bool> ForAllAsync<A>(this Option<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : true;

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned if invoking None returns True.
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then True is returned if 
    /// invoking None returns True. If the Option is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    public static async Task<bool> ExistsAsync<A>(this Task<Option<A>> self, Func<A, bool> pred) =>
        (await self).Exists(pred);

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned if invoking None returns True.
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then True is returned if 
    /// invoking None returns True. If the Option is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    public static async Task<bool> ExistsAsync<A>(this Option<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : false;

    [Pure]
    internal static U CheckNullReturn<U>(U value, string location) =>
        isnull(value)
            ? raise<U>(new ResultIsNullException($"'{location}' result is null.  Not allowed."))
            : value;

    [Pure]
    internal static U CheckNullNoneReturn<U>(U value) =>
        CheckNullReturn(value, "None");

    [Pure]
    internal static U CheckNullSomeReturn<U>(U value) =>
        CheckNullReturn(value, "Some");
}
