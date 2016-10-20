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

/// <summary>
/// Extension methods for OptionUnsafe
/// By using extension methods we can check for null references in 'this'
/// </summary>
public static class OptionUnsafeExtensions
{
    /// <summary>
    /// <summary>
    /// Extracts from a list of 'OptionUnsafe' all the 'Some' elements.
    /// All the 'Some' elements are extracted in order.
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
    /// Add the bound values of x and y, uses an Add type-class to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="ADD">Add of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An OptionUnsafe with y added to x</returns>
    [Pure]
    public static OptionUnsafe<A> Add<ADD, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where ADD : struct, Addition<A> =>
        from a in x
        from b in y
        select add<ADD, A>(a, b);

    /// <summary>
    /// Find the difference between the two bound values of x and y, uses a Difference type-class 
    /// to provide the difference operation for type A.  For example x.Difference<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="DIFF">Difference of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An OptionUnsafe with the difference between x and y</returns>
    [Pure]
    public static OptionUnsafe<A> Difference<DIFF, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where DIFF : struct, Difference<A> =>
        from a in x
        from b in y
        select difference<DIFF, A>(a, b);

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
    public static OptionUnsafe<A> Product<PROD, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where PROD : struct, Product<A> =>
        from a in x
        from b in y
        select product<PROD, A>(a, b);

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
    public static OptionUnsafe<A> Divide<DIV, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where DIV : struct, Divisible<A> =>
        from a in x
        from b in y
        select divide<DIV, A>(a, b);

    /// Apply y to x
    /// </summary>
    [Pure]
    public static OptionUnsafe<B> Apply<A, B>(this OptionUnsafe<Func<A, B>> x, OptionUnsafe<A> y) =>
        from a in x
        from b in y
        select a(b);
                    
    /// <summary>
    /// Apply y and z to x
    /// </summary>
    [Pure]
    public static OptionUnsafe<C> Apply<A, B, C>(this OptionUnsafe<Func<A, B, C>> x, OptionUnsafe<A> y, OptionUnsafe<B> z) =>
        (OptionUnsafe<C>)(from a in x
                    from b in y
                    from c in z
                    select a(b, c));

    /// <summary>
    /// Apply y to x
    /// </summary>
    [Pure]
    public static OptionUnsafe<Func<B, C>> Apply<A, B, C>(this OptionUnsafe<Func<A, Func<B, C>>> x, OptionUnsafe<A> y) =>
        (OptionUnsafe<Func<B,C>>)(from a in x
                            from b in y
                            select a(b));
                    
    /// <summary>
    /// Apply x, then y, ignoring the result of x
    /// </summary>
    [Pure]
    public static OptionUnsafe<B> Action<A, B>(this OptionUnsafe<A> x, OptionUnsafe<B> y) =>
        (OptionUnsafe<B>)(from a in x
                    from b in y
                    select b);

    /// <summary>
    /// Convert the Option type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Option to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static A? ToNullable<A>(this OptionUnsafe<A> ma) where A : struct =>
        ma.IsNone
            ? (A?)null
            : ma.Value;

    /// <summary>
    /// Match the two states of the Option and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this OptionUnsafe<A> ma, Func<A, Task<B>> Some, Func<B> None) =>
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
    public static async Task<B> MatchAsync<A, B>(this OptionUnsafe<A> ma, Func<A, Task<B>> Some, Func<Task<B>> None) =>
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
    public static IObservable<B> MatchObservable<A, B>(this OptionUnsafe<A> ma, Func<A, IObservable<B>> Some, Func<B> None) =>
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
    public static IObservable<B> MatchObservable<A, B>(this OptionUnsafe<A> ma, Func<A, IObservable<B>> Some, Func<IObservable<B>> None) =>
        ma.IsSome
            ? Some(ma.Value).Select(CheckNullSomeReturn)
            : None().Select(CheckNullNoneReturn);

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


    /// <summary>
    /// Sum the bound value
    /// </summary>
    /// <remarks>This is a legacy method for backwards compatibility</remarks>
    /// <param name="a">Option of int</param>
    /// <returns>The bound value or 0 if None</returns>
    public static int Sum(this OptionUnsafe<int> a) =>
        a.IfNoneUnsafe(0);

    /// <summary>
    /// Generic sum operation
    /// 
    /// Call option.Sum<TInt,int>() with an OptionV<int>, option.Sum<TDouble,double>() with a
    /// System.Double, etc.
    /// </summary>
    /// <typeparam name="NUM">Num<A> typeclass</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="a">Option of A</param>
    /// <returns>The bound value</returns>
    public static A Sum<NUM, A>(this OptionUnsafe<A> a) where NUM : struct, Num<A> =>
        a.IfNoneUnsafe(default(NUM).FromInteger(0));

    /// <summary>
    /// Fold the bound value
    /// </summary>
    /// <typeparam name="S">Initial state type</typeparam>
    /// <param name="ma">Monad to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="f">Fold operation</param>
    /// <returns>Aggregated state</returns>
    public static S Fold<A, S>(this OptionUnsafe<A> ma, S state, Func<S, A, S> f) =>
        ma.IsSome
            ? f(state, ma.Value)
            : state;

    /// <summary>
    /// Fold the bound value
    /// </summary>
    /// <typeparam name="S">Initial state type</typeparam>
    /// <param name="ma">Monad to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="f">Fold operation</param>
    /// <returns>Aggregated state</returns>
    public static S FoldBack<A, S>(this OptionUnsafe<A> ma, S state, Func<S, A, S> f) =>
        ma.IsSome
            ? f(state, ma.Value)
            : state;

    /// <summary>
    /// Iterate the bound value (or not if the option is in a Some state)
    /// </summary>
    /// <param name="action">Action to perform</param>
    public static Unit Iter<A>(this OptionUnsafe<A> ma, Action<A> action) =>
        ma.IfSomeUnsafe(action);

    /// <summary>
    /// Returns 0 if the option is in a None state, 1 otherwise
    /// </summary>
    /// <returns>0 or 1</returns>
    [Pure]
    public static int Count<A>(this OptionUnsafe<A> ma) =>
        ma.IsSome
            ? 1
            : 0;

    /// <summary>
    /// Runs a predicate against the bound value.  If the option is in a 
    /// None state then 'true' is returned (because the predicate holds 
    /// for-all values).
    /// </summary>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>True if the predicate holds for all values</returns>
    [Pure]
    public static bool ForAll<A>(this OptionUnsafe<A> ma, Func<A, bool> pred) =>
        ma.IsSome
            ? pred(ma.Value)
            : true;

    /// <summary>
    /// Runs a predicate against the bound value.  If the option is in a 
    /// None state then 'false' is returned (because the predicate doesn't
    /// hold for any value).
    /// </summary>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>False if the predicate holds for any value</returns>
    [Pure]
    public static bool Exists<A>(this OptionUnsafe<A> ma, Func<A, bool> pred) =>
        ma.IsSome
            ? pred(ma.Value)
            : false;

    /// <summary>
    /// Bi-fold
    /// </summary>
    [Pure]
    public static S BiFold<A, S>(this OptionUnsafe<A> ma, S state, Func<S, A, S> Some, Func<S, S> None) =>
        ma.IsSome
            ? Some(state, ma.Value)
            : None(state);

    [Pure]
    public static OptionUnsafe<B> Map<A, B>(this OptionUnsafe<A> ma, Func<A, B> mapper) =>
        ma.IsSome
            ? SomeUnsafe(mapper(ma.Value))
            : None;

    [Pure]
    public static OptionUnsafe<B> BiMap<A, B>(this OptionUnsafe<A> ma, Func<A, B> Some, Func<B> None) =>
        ma.IsSome
            ? SomeUnsafe(Some(ma.Value))
            : SomeUnsafe(None());

    [Pure]
    public static OptionUnsafe<A> Filter<A>(this OptionUnsafe<A> ma, Func<A, bool> pred) =>
        ma.IsSome
            ? pred(ma.Value)
                ? ma
                : OptionUnsafe<A>.None
            : ma;

    [Pure]
    public static OptionUnsafe<A> BiFilter<A>(this OptionUnsafe<A> ma, Func<A, bool> Some, Func<bool> None) =>
        ma.IsSome
            ? Some(ma.Value)
                ? ma
                : OptionUnsafe<A>.None
            : None()
                ? ma
                : OptionUnsafe<A>.None;

    [Pure]
    public static OptionUnsafe<B> Bind<A, B>(this OptionUnsafe<A> ma, Func<A, OptionUnsafe<B>> binder) =>
        ma.IsSome
            ? binder(ma.Value)
            : OptionUnsafe<B>.None;

    [Pure]
    public static OptionUnsafe<B> BiBind<A, B>(this OptionUnsafe<A> ma, Func<A, OptionUnsafe<B>> Some, Func<OptionUnsafe<B>> None) =>
        ma.IsSome
            ? Some(ma.Value)
            : None();

    [Pure]
    public static OptionUnsafe<A> Where<A>(this OptionUnsafe<A> ma, Func<A, bool> pred) =>
        ma.Filter(pred);

    public static OptionUnsafe<D> Join<A, B, C, D>(
        this OptionUnsafe<A> ma,
        OptionUnsafe<B> inner,
        Func<A, C> outerKeyMap,
        Func<B, C> innerKeyMap,
        Func<A, B, D> project)
    {
        if (ma.IsNone) return OptionUnsafe<D>.None;
        if (inner.IsNone) return OptionUnsafe<D>.None;
        return EqualityComparer<C>.Default.Equals(outerKeyMap(ma.Value), innerKeyMap(inner.Value))
            ? SomeUnsafe(project(ma.Value, inner.Value))
            : None;
    }

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static OptionUnsafe<Func<B, C>> ParMap<A, B, C>(this OptionUnsafe<A> opt, Func<A, B, C> func) =>
        opt.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static OptionUnsafe<Func<B, Func<C, D>>> ParMap<A, B, C, D>(this OptionUnsafe<A> opt, Func<A, B, C, D> func) =>
        opt.Map(curry(func));

    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this OptionUnsafe<A> self, Func<A, Task<B>> map) =>
        self.IsSome
            ? SomeUnsafe(await map(self.Value))
            : None;

    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, Task<B>> map)
    {
        var val = await self;
        return val.IsSome
            ? SomeUnsafe(await map(val.Value))
            : None;
    }

    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, B> map)
    {
        var val = await self;
        return val.IsSome
            ? SomeUnsafe(map(val.Value))
            : None;
    }

    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, B> map) =>
        self.IsSome
            ? SomeUnsafe(map(await self.Value))
            : None;

    public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, Task<B>> map) =>
        self.IsSome
            ? SomeUnsafe(await map(await self.Value))
            : None;


    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this OptionUnsafe<A> self, Func<A, Task<OptionUnsafe<B>>> bind) =>
        self.IsSome
            ? await bind(self.Value)
            : None;

    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, Task<OptionUnsafe<B>>> bind)
    {
        var val = await self;
        return val.IsSome
            ? await bind(val.Value)
            : None;
    }

    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this Task<OptionUnsafe<A>> self, Func<A, OptionUnsafe<B>> bind)
    {
        var val = await self;
        return val.IsSome
            ? bind(val.Value)
            : OptionUnsafe<B>.None;
    }

    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, OptionUnsafe<B>> bind) =>
        self.IsSome
            ? bind(await self.Value)
            : OptionUnsafe<B>.None;

    public static async Task<OptionUnsafe<B>> BindAsync<A, B>(this OptionUnsafe<Task<A>> self, Func<A, Task<OptionUnsafe<B>>> bind) =>
        self.IsSome
            ? await bind(await self.Value)
            : OptionUnsafe<B>.None;

    public static async Task<Unit> IterAsync<A>(this Task<OptionUnsafe<A>> self, Action<A> action)
    {
        var val = await self;
        if (val.IsSome) action(val.Value);
        return unit;
    }

    public static async Task<Unit> IterAsync<A>(this OptionUnsafe<Task<A>> self, Action<A> action)
    {
        if (self.IsSome) action(await self.Value);
        return unit;
    }

    public static async Task<int> CountAsync<A>(this Task<OptionUnsafe<A>> self) =>
        (await self).Count();

    public static async Task<S> FoldAsync<A, S>(this Task<OptionUnsafe<A>> self, S state, Func<S, A, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<A, S>(this OptionUnsafe<Task<A>> self, S state, Func<S, A, S> folder) =>
        self.IsSome
            ? folder(state, await self.Value)
            : state;

    public static async Task<bool> ForAllAsync<A>(this Task<OptionUnsafe<A>> self, Func<A, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<A>(this OptionUnsafe<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : true;

    public static async Task<bool> ExistsAsync<A>(this Task<OptionUnsafe<A>> self, Func<A, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<A>(this OptionUnsafe<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : false;

    /// <summary>
    /// Bind Option -> IEnumerable
    /// </summary>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<C> SelectMany<A, B, C>(this OptionUnsafe<A> self,
        Func<A, IEnumerable<B>> bind,
        Func<A, B, C> project
        )
    {
        if (self.IsNone) return new C[0];
        var v = self.Value;
        return bind(v).Map(resU => project(v, resU));
    }

    /// <summary>
    /// Bind IEnumerable -> Option
    /// </summary>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<OptionUnsafe<C>> SelectMany<A, B, C>(this IEnumerable<A> self,
        Func<A, OptionUnsafe<B>> bind,
        Func<A, B, C> project
        )
    {
        foreach(var a in self)
        {
            var mb = bind(a);
            yield return mb.Map(b => project(a, b));
        }
    }

}
