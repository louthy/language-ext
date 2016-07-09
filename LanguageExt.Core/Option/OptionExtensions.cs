using System;
using System.Linq;
using System.Reactive.Linq;
using LanguageExt;
using LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// Extension methods for Option
/// By using extension methods we can check for null references in 'this'
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
    public static Option<T> Mappend<SEMI, T>(this Option<T> lhs, Option<T> rhs) where SEMI : struct, Semigroup<T> =>
        from x in lhs
        from y in rhs
        select append<SEMI, T>(x, y);

    /// <summary>
    /// Folds the provided list of options
    /// </summary>
    /// <typeparam name="SEMI">Semigroup that represents T</typeparam>
    /// <typeparam name="T">Monadic value</typeparam>
    /// <param name="xs">List of Options to concat</param>
    /// <returns>Folded options</returns>
    [Pure]
    public static Option<T> Mconcat<SEMI, T>(this Option<T>[] xs) where SEMI : struct, Semigroup<T> =>
        xs == null || xs.Length == 0
            ? Option<T>.None
            : xs.Reduce((s, x) =>
                s.IsNone()
                    ? s
                    : x.IsNone()
                        ? x
                        : mappend<SEMI, T>(s, x));

    /// <summary>
    /// Folds the provided list of options
    /// </summary>
    /// <typeparam name="SEMI">Semigroup that represents T</typeparam>
    /// <typeparam name="T">Monadic value</typeparam>
    /// <param name="xs">List of Options to concat</param>
    /// <returns>Folded options</returns>
    [Pure]
    public static Option<T> Mconcat<SEMI, T>(this IEnumerable<Option<T>> xs) where SEMI : struct, Semigroup<T> =>
        xs == null || !xs.Any()
            ? Option<T>.None
            : xs.Reduce((s, x) =>
                s.IsNone()
                    ? s
                    : x.IsNone()
                        ? x
                        : mappend<SEMI, T>(s, x));

    /// <summary>
    /// Test the option state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>True if the Option is in a None state</returns>
    [Pure]
    public static bool IsNone<A>(this Option<A> ma) =>
        ma == null || ma is None<A>;

    /// <summary>
    /// Test the option state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>True if the Option is in a Some state</returns>
    [Pure]
    public static bool IsSome<A>(this Option<A> ma) =>
        !IsNone(ma);

    /// <summary>
    /// Functor map operation for Option
    /// </summary>
    [Pure]
    public static Option<B> Select<A, B>(this Option<A> ma, Func<A, B> f) =>
        ma == null || f == null || ma is None<A>
            ? Option<B>.None
            : Option<B>.Optional(f(Option<A>.Some(ma).Value));

    /// <summary>
    /// Monad bind operation for Option
    /// </summary>
    [Pure]
    public static Option<C> SelectMany<A, B, C>(
        this Option<A> ma,
        Func<A, Option<B>> bind,
        Func<A, B, C> project
        )
    {
        if (ma == null || bind == null || project == null || ma is None<A>) return Option<C>.None;
        var a = Option<A>.Some(ma).Value;
        var mb = bind(a);
        if (mb == null || mb is None<B>) return Option<C>.None;
        var b = Option<B>.Some(mb).Value;
        return Option<C>.Optional(project(a, b));
    }

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <typeparam name="EQ">Type-class of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>True if the bound values are equal</returns>
    public static bool Equals<EQ, A>(this Option<A> x, Option<A> y) where EQ : struct, Eq<A> =>
        x == null && y == null
            ? true
            : x == null || y == null
                ? false
                : x.IsNone() && y.IsNone()
                    ? true
                    : x.IsNone() || y.IsNone()
                        ? false
                        : equals<EQ, A>(Option<A>.Some(x).Value, Option<A>.Some(y).Value);

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable of zero or one items</returns>
    public static A[] ToArray<A>(this Option<A> ma) =>
        ma == null || ma.IsNone()
            ? new A[0]
            : new A[1] { Option<A>.Some(ma).Value };

    /// <summary>
    /// Convert the Option to an immutable list of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An immutable list of zero or one items</returns>
    public static Lst<A> ToList<A>(this Option<A> ma) =>
        List(ToArray(ma));

    /// <summary>
    /// Convert the Option to an enumerable sequence of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable sequence of zero or one items</returns>
    public static IEnumerable<A> ToSeq<A>(this Option<A> ma) =>
        ToArray(ma).AsEnumerable();

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable of zero or one items</returns>
    public static IEnumerable<A> AsEnumerable<A>(this Option<A> ma) =>
        ToArray(ma).AsEnumerable();

    /// <summary>
    /// Appends the bound values of x and y, uses a semigroup type-class to provide 
    /// the append operation for type A.  For example x.Append<TString,string>(y)
    /// </summary>
    /// <typeparam name="SEMI">Semigroup of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y appended to x</returns>
    [Pure]
    public static Option<A> Append<SEMI, A>(this Option<A> x, Option<A> y) where SEMI : struct, Semigroup<A> =>
        x == null || y == null
            ? Option<A>.None
            : from a in x
                from b in y
                select append<SEMI, A>(a, b);

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
    public static Option<A> Add<ADD, A>(this Option<A> x, Option<A> y) where ADD : struct, Add<A> =>
        x == null || y == null
            ? Option<A>.None
            : from a in x
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
        x == null || y == null
            ? Option<A>.None
            : from a in x
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
        x == null || y == null
            ? Option<A>.None
            : from a in x
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
    public static Option<A> Divide<DIV, A>(this Option<A> x, Option<A> y) where DIV : struct, Divide<A> =>
        x == null || y == null
            ? Option<A>.None
            : from a in x
                from b in y
                select divide<DIV, A>(a, b);

    /// <summary>
    /// Convert the Option type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Option to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static A? ToNullable<A>(this Option<A> ma) where A : struct =>
        ma.IsNone()
            ? (A?)null
            : Option<A>.Some(ma).Value;

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A non-null R</returns>
    [Pure]
    public static B Match<A, B>(this Option<A> ma, Func<A, B> Some, Func<B> None) =>
        ma.IsNone()
            ? CheckNullNoneReturn(None())
            : CheckNullSomeReturn(Some(Option<A>.Some(ma).Value));


    /// <summary>
    /// Match the two states of the Option and return an R, which can be null.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  May return null.</param>
    /// <param name="None">None handler.  May return null.</param>
    /// <returns>R, or null</returns>
    [Pure]
    public static B MatchUnsafe<A, B>(this Option<A> ma, Func<A, B> Some, Func<B> None) =>
        ma.IsNone()
            ? None()
            : Some(Option<A>.Some(ma).Value);

    /// <summary>
    /// Match the two states of the Option and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this Option<A> ma, Func<A, Task<B>> Some, Func<B> None) =>
        ma.IsSome()
            ? CheckNullSomeReturn(await Some(Option<A>.Some(ma).Value))
            : CheckNullNoneReturn(None());

    /// <summary>
    /// Match the two states of the Option and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this Option<A> ma, Func<A, Task<B>> Some, Func<Task<B>> None) =>
        ma.IsSome()
            ? CheckNullSomeReturn(await Some(Option<A>.Some(ma).Value))
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
        ma.IsSome()
            ? Some(Option<A>.Some(ma).Value).Select(CheckNullSomeReturn)
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
        ma.IsSome()
            ? Some(Option<A>.Some(ma).Value).Select(CheckNullSomeReturn)
            : None().Select(CheckNullNoneReturn);

    /// <summary>
    /// Match the two states of the Option and return an R, or null.
    /// </summary>
    /// <param name="Some">Some handler.  May return null.</param>
    /// <param name="None">None handler.  May return null.</param>
    /// <returns>An R, or null</returns>
    [Pure]
    public static B MatchUntyped<B>(this Option<object> ma, Func<object, B> Some, Func<B> None) =>
        ma.IsSome()
            ? Some(Option<object>.Some(ma).Value)
            : None();

    /// <summary>
    /// Match the two states of the Option T
    /// </summary>
    /// <param name="Some">Some match</param>
    /// <param name="None">None match</param>
    /// <returns></returns>
    public static Unit Match<A>(this Option<A> ma, Action<A> Some, Action None)
    {
        if (ma.IsSome())
        {
            Some(Option<A>.Some(ma).Value);
        }
        else
        {
            None();
        }
        return unit;
    }

    /// <summary>
    /// Invokes the someHandler if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit IfSome<A>(this Option<A> ma, Action<A> someHandler)
    {
        if (ma.IsSome())
        {
            someHandler(Option<A>.Some(ma).Value);
        }
        return unit;
    }

    /// <summary>
    /// Invokes the someHandler if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit IfSome<A>(this Option<A> ma, Func<A, Unit> someHandler)
    {
        if (ma.IsSome())
        {
            someHandler(Option<A>.Some(ma).Value);
        }
        return unit;
    }

    [Pure]
    public static A IfNone<A>(this Option<A> ma, Func<A> None) =>
        ma.Match(identity, None);

    [Pure]
    public static A IfNone<A>(this Option<A> ma, A noneValue) =>
        ma.Match(identity, () => noneValue);

    [Pure]
    public static A IfNoneUnsafe<A>(this Option<A> ma, Func<A> None) =>
        ma.MatchUnsafe(identity, None);

    [Pure]
    public static A IfNoneUnsafe<A>(this Option<A> ma, A noneValue) =>
        ma.MatchUnsafe(identity, () => noneValue);

    /// <summary>
    /// Fold the bound value
    /// </summary>
    /// <typeparam name="S">Initial state type</typeparam>
    /// <param name="ma">Monad to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="f">Fold operation</param>
    /// <returns>Aggregated state</returns>
    public static S Fold<A, S>(this Option<A> ma, S state, Func<S, A, S> f) =>
        ma.IsSome()
            ? f(state, Option<A>.Some(ma).Value)
            : state;

    /// <summary>
    /// Fold the bound value
    /// </summary>
    /// <typeparam name="S">Initial state type</typeparam>
    /// <param name="ma">Monad to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="f">Fold operation</param>
    /// <returns>Aggregated state</returns>
    public static S FoldBack<A, S>(this Option<A> ma, S state, Func<S, A, S> f) =>
        ma.IsSome()
            ? f(state, Option<A>.Some(ma).Value)
            : state;

    /// <summary>
    /// Iterate the bound value (or not if the option is in a Some state)
    /// </summary>
    /// <param name="action">Action to perform</param>
    public static Unit Iter<A>(this Option<A> ma, Action<A> action) =>
        ma.IfSome(action);

    /// <summary>
    /// Returns 0 if the option is in a None state, 1 otherwise
    /// </summary>
    /// <returns>0 or 1</returns>
    [Pure]
    public static int Count<A>(this Option<A> ma) =>
        ma.IsSome()
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
    public static bool ForAll<A>(this Option<A> ma, Func<A, bool> pred) =>
        ma.IsSome()
            ? pred(Option<A>.Some(ma).Value)
            : true;

    /// <summary>
    /// Runs a predicate against the bound value.  If the option is in a 
    /// None state then 'false' is returned (because the predicate doesn't
    /// hold for any value).
    /// </summary>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>False if the predicate holds for any value</returns>
    [Pure]
    public static bool Exists<A>(this Option<A> ma, Func<A, bool> pred) =>
        ma.IsSome()
            ? pred(Option<A>.Some(ma).Value)
            : false;

    /// <summary>
    /// Bi-fold
    /// </summary>
    [Pure]
    public static S BiFold<A, S>(this Option<A> ma, S state, Func<S, A, S> Some, Func<S, S> None) =>
        ma.IsSome()
            ? Some(state, Option<A>.Some(ma).Value)
            : None(state);

    [Pure]
    public static Option<B> Map<A, B>(this Option<A> ma, Func<A, B> mapper) =>
        ma.IsSome()
            ? Optional(mapper(Option<A>.Some(ma).Value))
            : Option<B>.None;

    [Pure]
    public static Option<B> BiMap<A,B>(this Option<A> ma, Func<A, B> Some, Func<B> None) =>
        ma.IsSome()
            ? Optional(Some(Option<A>.Some(ma).Value))
            : Option<B>.None;

    [Pure]
    public static Option<A> Filter<A>(this Option<A> ma, Func<A, bool> pred) =>
        ma.IsSome()
            ? pred(Option<A>.Some(ma).Value)
                ? ma
                : Option<A>.None
            : ma;

    [Pure]
    public static Option<A> BiFilter<A>(this Option<A> ma, Func<A, bool> Some, Func<bool> None) =>
        ma.IsSome()
            ? Some(Option<A>.Some(ma).Value)
                ? ma
                : Option<A>.None
            : None()
                ? ma
                : Option<A>.None;

    [Pure]
    public static Option<B> Bind<A, B>(this Option<A> ma, Func<A, Option<B>> binder) =>
        ma.IsSome()
            ? binder(Option<A>.Some(ma).Value)
            : Option<B>.None;

    [Pure]
    public static Option<B> BiBind<A,B>(this Option<A> ma, Func<A, Option<B>> Some, Func<Option<B>> None) =>
        ma.IsSome()
            ? Some(Option<A>.Some(ma).Value)
            : None();

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<A> Where<A>(this Option<A> ma, Func<A, bool> pred) =>
        ma.Filter(pred);

    public static Option<V> Join<T, U, K, V>(
        this Option<T> ma,
        Option<U> inner,
        Func<T, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<T, U, V> project)
    {
        if (ma.IsNone()) return Option<V>.None;
        if (inner.IsNone()) return Option<V>.None;
        return EqualityComparer<K>.Default.Equals(outerKeyMap(Option<T>.Some(ma).Value), innerKeyMap(Option<U>.Some(inner).Value))
            ? Option<V>.Some(project(Option<T>.Some(ma).Value, Option<U>.Some(inner).Value))
            : Option<V>.None;
    }

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Option<Func<T2, R>> ParMap<T1, T2, R>(this Option<T1> opt, Func<T1, T2, R> func) =>
        opt.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Option<Func<T2, Func<T3, R>>> ParMap<T1, T2, T3, R>(this Option<T1> opt, Func<T1, T2, T3, R> func) =>
        opt.Map(curry(func));

    /// <summary>
    /// Extracts from a list of 'Option' all the 'Some' elements.
    /// All the 'Some' elements are extracted in order.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Somes<T>(this IEnumerable<Option<T>> self)
    {
        foreach (var item in self)
        {
            if (item.IsSome())
            {
                yield return Option<T>.Some(item).Value;
            }
        }
    }

    public static async Task<Option<R>> MapAsync<T, R>(this Option<T> self, Func<T, Task<R>> map) =>
        self.IsSome()
            ? Some(await map(Option<T>.Some(self).Value))
            : Option<R>.None;

    public static async Task<Option<R>> MapAsync<T, R>(this Task<Option<T>> self, Func<T, Task<R>> map)
    {
        var val = await self;
        return val.IsSome()
            ? Some(await map(Option<T>.Some(val).Value))
            : Option<R>.None;
    }

    public static async Task<Option<R>> MapAsync<T, R>(this Task<Option<T>> self, Func<T, R> map)
    {
        var val = await self;
        return val.IsSome()
            ? Some(map(Option<T>.Some(val).Value))
            : Option<R>.None;
    }

    public static async Task<Option<R>> MapAsync<T, R>(this Option<Task<T>> self, Func<T, R> map) =>
        self.IsSome()
            ? Some(map(await Option<Task<T>>.Some(self).Value))
            : Option<R>.None;

    public static async Task<Option<R>> MapAsync<T, R>(this Option<Task<T>> self, Func<T, Task<R>> map) =>
        self.IsSome()
            ? Some(await map(await Option<Task<T>>.Some(self).Value))
            : Option<R>.None;


    public static async Task<Option<R>> BindAsync<T, R>(this Option<T> self, Func<T, Task<Option<R>>> bind) =>
        self.IsSome()
            ? await bind(Option<T>.Some(self).Value)
            : Option<R>.None;

    public static async Task<Option<R>> BindAsync<T, R>(this Task<Option<T>> self, Func<T, Task<Option<R>>> bind)
    {
        var val = await self;
        return val.IsSome()
            ? await bind(Option<T>.Some(val).Value)
            : Option<R>.None;
    }

    public static async Task<Option<R>> BindAsync<T, R>(this Task<Option<T>> self, Func<T, Option<R>> bind)
    {
        var val = await self;
        return val.IsSome()
            ? bind(Option<T>.Some(val).Value)
            : Option<R>.None;
    }

    public static async Task<Option<R>> BindAsync<T, R>(this Option<Task<T>> self, Func<T, Option<R>> bind) =>
        self.IsSome()
            ? bind(await Option<Task<T>>.Some(self).Value)
            : Option<R>.None;

    public static async Task<Option<R>> BindAsync<T, R>(this Option<Task<T>> self, Func<T, Task<Option<R>>> bind) =>
        self.IsSome()
            ? await bind(await Option<Task<T>>.Some(self).Value)
            : Option<R>.None;

    public static async Task<Unit> IterAsync<T>(this Task<Option<T>> self, Action<T> action)
    {
        var val = await self;
        if (val.IsSome()) action(Option<T>.Some(val).Value);
        return unit;
    }

    public static async Task<Unit> IterAsync<T>(this Option<Task<T>> self, Action<T> action)
    {
        if (self.IsSome()) action(await Option<Task<T>>.Some(self).Value);
        return unit;
    }

    public static async Task<int> CountAsync<T>(this Task<Option<T>> self) =>
        (await self).Count();

    public static async Task<S> FoldAsync<T, S>(this Task<Option<T>> self, S state, Func<S, T, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<T, S>(this Option<Task<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome()
            ? folder(state, await Option<Task<T>>.Some(self).Value)
            : state;

    public static async Task<bool> ForAllAsync<T>(this Task<Option<T>> self, Func<T, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<T>(this Option<Task<T>> self, Func<T, bool> pred) =>
        self.IsSome()
            ? pred(await Option<Task<T>>.Some(self).Value)
            : true;

    public static async Task<bool> ExistsAsync<T>(this Task<Option<T>> self, Func<T, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<T>(this Option<Task<T>> self, Func<T, bool> pred) =>
        self.IsSome()
            ? pred(await Option<Task<T>>.Some(self).Value)
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

    [Pure]
    internal static A Value<A>(this Option<A> self) =>
        (self as Some<A>).Value;
}
