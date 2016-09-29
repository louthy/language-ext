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
/// Extension methods for Option
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Append the Some(x) of one option to the Some(y) of another.  If either of the
    /// options are None then the result is None
    /// For numeric values the behaviour is to sum the Somes (lhs + rhs)
    /// For string values the behaviour is to concatenate the strings
    /// For Lst/Stck/Que values the behaviour is to concatenate the lists
    /// For Map or Set values the behaviour is to merge the sets
    /// Otherwise if the R type derives from IAppendable then the behaviour
    /// is to call lhs.Append(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Option<A> Append<SEMI, A>(this Option<A> lhs, Option<A> rhs) where SEMI : struct, Semigroup<A> =>
        from x in lhs
        from y in rhs
        select append<SEMI, A>(x, y);

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
    public static Option<A> Add<ADD, A>(this Option<A> x, Option<A> y) where ADD : struct, Addition<A> =>
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
    /// <returns>An option with the difference between x and y</returns>
    [Pure]
    public static Option<A> Difference<DIFF, A>(this Option<A> x, Option<A> y) where DIFF : struct, Difference<A> =>
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
    /// <returns>An option with the product of x and y</returns>
    [Pure]
    public static Option<A> Product<PROD, A>(this Option<A> x, Option<A> y) where PROD : struct, Product<A> =>
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
    /// <returns>An option x / y</returns>
    [Pure]
    public static Option<A> Divide<DIV, A>(this Option<A> x, Option<A> y) where DIV : struct, Divisible<A> =>
        from a in x
        from b in y
        select divide<DIV, A>(a, b);

    /// Apply y to x
    /// </summary>
    [Pure]
    public static Option<B> Apply<A, B>(this Option<Func<A, B>> x, Option<A> y) =>
        x.Apply<Option<B>, A, B>(y);

    /// <summary>
    /// Apply y and z to x
    /// </summary>
    [Pure]
    public static Option<C> Apply<A, B, C>(this Option<Func<A, B, C>> x, Option<A> y, Option<B> z) =>
        x.Apply<Option<C>, A, B, C>(y, z);

    /// <summary>
    /// Apply y to x
    /// </summary>
    [Pure]
    public static Option<Func<B, C>> Apply<A, B, C>(this Option<Func<A, Func<B, C>>> x, Option<A> y) =>
        x.Apply<Option<Func<B, C>>, A, B, C>(y);

    /// <summary>
    /// Apply x, then y, ignoring the result of x
    /// </summary>
    [Pure]
    public static Option<B> Action<A, B>(this Option<A> x, Option<B> y) =>
        x.Action<Option<B>, A, B>(y);

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
    public static int Sum(this Option<int> a) =>
        a.IfNone(0);

    /// <summary>
    /// Bi-fold
    /// </summary>
    [Pure]
    public static S BiFold<A, S>(this Option<A> ma, S state, Func<S, A, S> Some, Func<S, S> None) =>
        ma.IsSome
            ? Some(state, ma.Value)
            : None(state);

    [Pure]
    public static Option<B> Map<A, B>(this Option<A> ma, Func<A, B> mapper) =>
        ma.IsSome
            ? Optional(mapper(ma.Value))
            : None;

    [Pure]
    public static Option<B> BiMap<A, B>(this Option<A> ma, Func<A, B> Some, Func<B> None) =>
        ma.IsSome
            ? Optional(Some(ma.Value))
            : Optional(None());

    [Pure]
    public static Option<A> Filter<A>(this Option<A> ma, Func<A, bool> pred) =>
        ma.IsSome
            ? pred(ma.Value)
                ? ma
                : Option<A>.None
            : ma;

    [Pure]
    public static Option<A> BiFilter<A>(this Option<A> ma, Func<A, bool> Some, Func<bool> None) =>
        ma.IsSome
            ? Some(ma.Value)
                ? ma
                : Option<A>.None
            : None()
                ? ma
                : Option<A>.None;

    [Pure]
    public static Option<B> Bind<A, B>(this Option<A> ma, Func<A, Option<B>> binder) =>
        ma.IsSome
            ? binder(ma.Value)
            : Option<B>.None;

    [Pure]
    public static Option<B> BiBind<A, B>(this Option<A> ma, Func<A, Option<B>> Some, Func<Option<B>> None) =>
        ma.IsSome
            ? Some(ma.Value)
            : None();

    [Pure]
    public static Option<A> Where<A>(this Option<A> ma, Func<A, bool> pred) =>
        ma.Filter(pred);

    public static Option<D> Join<A, B, C, D>(
        this Option<A> ma,
        Option<B> inner,
        Func<A, C> outerKeyMap,
        Func<B, C> innerKeyMap,
        Func<A, B, D> project)
    {
        if (ma.IsNone) return Option<D>.None;
        if (inner.IsNone) return Option<D>.None;
        return EqualityComparer<C>.Default.Equals(outerKeyMap(ma.Value), innerKeyMap(inner.Value))
            ? Optional(project(ma.Value, inner.Value))
            : None;
    }

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Option<Func<B, C>> ParMap<A, B, C>(this Option<A> opt, Func<A, B, C> func) =>
        opt.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Option<Func<B, Func<C, D>>> ParMap<A, B, C, D>(this Option<A> opt, Func<A, B, C, D> func) =>
        opt.Map(curry(func));

    public static async Task<Option<B>> MapAsync<A, B>(this Option<A> self, Func<A, Task<B>> map) =>
        self.IsSome
            ? Optional(await map(self.Value))
            : None;

    public static async Task<Option<B>> MapAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> map)
    {
        var val = await self;
        return val.IsSome
            ? Optional(await map(val.Value))
            : None;
    }

    public static async Task<Option<B>> MapAsync<A, B>(this Task<Option<A>> self, Func<A, B> map)
    {
        var val = await self;
        return val.IsSome
            ? Optional(map(val.Value))
            : None;
    }

    public static async Task<Option<B>> MapAsync<A, B>(this Option<Task<A>> self, Func<A, B> map) =>
        self.IsSome
            ? Optional(map(await self.Value))
            : None;

    public static async Task<Option<B>> MapAsync<A, B>(this Option<Task<A>> self, Func<A, Task<B>> map) =>
        self.IsSome
            ? Optional(await map(await self.Value))
            : None;


    public static async Task<Option<B>> BindAsync<A, B>(this Option<A> self, Func<A, Task<Option<B>>> bind) =>
        self.IsSome
            ? await bind(self.Value)
            : None;

    public static async Task<Option<B>> BindAsync<A, B>(this Task<Option<A>> self, Func<A, Task<Option<B>>> bind)
    {
        var val = await self;
        return val.IsSome
            ? await bind(val.Value)
            : None;
    }

    public static async Task<Option<B>> BindAsync<A, B>(this Task<Option<A>> self, Func<A, Option<B>> bind)
    {
        var val = await self;
        return val.IsSome
            ? bind(val.Value)
            : Option<B>.None;
    }

    public static async Task<Option<B>> BindAsync<A, B>(this Option<Task<A>> self, Func<A, Option<B>> bind) =>
        self.IsSome
            ? bind(await self.Value)
            : Option<B>.None;

    public static async Task<Option<B>> BindAsync<A, B>(this Option<Task<A>> self, Func<A, Task<Option<B>>> bind) =>
        self.IsSome
            ? await bind(await self.Value)
            : Option<B>.None;

    public static async Task<Unit> IterAsync<A>(this Task<Option<A>> self, Action<A> action)
    {
        var val = await self;
        if (val.IsSome) action(val.Value);
        return unit;
    }

    public static async Task<Unit> IterAsync<A>(this Option<Task<A>> self, Action<A> action)
    {
        if (self.IsSome) action(await self.Value);
        return unit;
    }

    public static async Task<int> CountAsync<A>(this Task<Option<A>> self) =>
        (await self).Count();

    public static async Task<S> FoldAsync<A, S>(this Task<Option<A>> self, S state, Func<S, A, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<A, S>(this Option<Task<A>> self, S state, Func<S, A, S> folder) =>
        self.IsSome
            ? folder(state, await self.Value)
            : state;

    public static async Task<bool> ForAllAsync<A>(this Task<Option<A>> self, Func<A, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<A>(this Option<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : true;

    public static async Task<bool> ExistsAsync<A>(this Task<Option<A>> self, Func<A, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<A>(this Option<Task<A>> self, Func<A, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : false;

    /// <summary>
    /// Bind Option -> IEnumerable
    /// </summary>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<C> SelectMany<A, B, C>(this Option<A> self,
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
    public static IEnumerable<Option<C>> SelectMany<A, B, C>(this IEnumerable<A> self,
        Func<A, Option<B>> bind,
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
