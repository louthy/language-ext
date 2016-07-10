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

/// <summary>
/// Extension methods for OptionM
/// By using extension methods we can check for null references in 'this'
/// </summary>
public static class OptionMExtensions
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
    public static OptionM<T> Mappend<SEMI, T>(this OptionM<T> lhs, OptionM<T> rhs) where SEMI : struct, Semigroup<T> =>
        (OptionM<T>)(from x in lhs
                     from y in rhs
                     select append<SEMI, T>(x, y));

    /// <summary>
    /// Folds the provided list of options
    /// </summary>
    /// <typeparam name="SEMI">Semigroup that represents T</typeparam>
    /// <typeparam name="T">Monadic value</typeparam>
    /// <param name="xs">List of Options to concat</param>
    /// <returns>Folded options</returns>
    [Pure]
    public static OptionM<T> Mconcat<SEMI, T>(this OptionM<T>[] xs) where SEMI : struct, Semigroup<T> =>
        xs == null || xs.Length == 0
            ? OptionM<T>.None
            : xs.Reduce((s, x) =>
                s.IsNone()
                    ? s
                    : x.IsNone()
                        ? x
                        : s.Mappend<SEMI, T>(x));

    /// <summary>
    /// Folds the provided list of options
    /// </summary>
    /// <typeparam name="SEMI">Semigroup that represents T</typeparam>
    /// <typeparam name="T">Monadic value</typeparam>
    /// <param name="xs">List of Options to concat</param>
    /// <returns>Folded options</returns>
    [Pure]
    public static OptionM<T> Mconcat<SEMI, T>(this IEnumerable<OptionM<T>> xs) where SEMI : struct, Semigroup<T> =>
        xs == null || !xs.Any()
            ? OptionM<T>.None
            : xs.Reduce((s, x) =>
                s.IsNone()
                    ? s
                    : x.IsNone()
                        ? x
                        : s.Mappend<SEMI, T>(x));

    /// <summary>
    /// Test the option state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>True if the Option is in a None state</returns>
    [Pure]
    public static bool IsNone<A>(this OptionM<A> ma) =>
        ma.Value.IsNone();

    /// <summary>
    /// Test the option state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>True if the Option is in a Some state</returns>
    [Pure]
    public static bool IsSome<A>(this OptionM<A> ma) =>
        !IsNone(ma);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <typeparam name="EQ">Type-class of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>True if the bound values are equal</returns>
    public static bool Equals<EQ, A>(this OptionM<A> x, OptionM<A> y) where EQ : struct, Eq<A> =>
        x.IsNone() && y.IsNone()
            ? true
            : x.IsNone() || y.IsNone()
                ? false
                : equals<EQ, A>(x.Value.Value(), y.Value.Value());

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable of zero or one items</returns>
    public static A[] ToArray<A>(this OptionM<A> ma) =>
        ma.IsNone()
            ? new A[0]
            : new A[1] { ma.Value.Value() };

    /// <summary>
    /// Convert the Option to an immutable list of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An immutable list of zero or one items</returns>
    public static Lst<A> ToList<A>(this OptionM<A> ma) =>
        List(ToArray(ma));

    /// <summary>
    /// Convert the Option to an enumerable sequence of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable sequence of zero or one items</returns>
    public static IEnumerable<A> ToSeq<A>(this OptionM<A> ma) =>
        ToArray(ma).AsEnumerable();

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable of zero or one items</returns>
    public static IEnumerable<A> AsEnumerable<A>(this OptionM<A> ma) =>
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
    public static OptionM<A> Append<SEMI, A>(this OptionM<A> x, OptionM<A> y) where SEMI : struct, Semigroup<A> =>
        (OptionM<A>)(from a in x
                     from b in y
                     select append<SEMI, A>(a, b));

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
    public static OptionM<A> Add<ADD, A>(this OptionM<A> x, OptionM<A> y) where ADD : struct, Addition<A> =>
        (OptionM<A>)(from a in x
                     from b in y
                     select add<ADD, A>(a, b));

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
    public static OptionM<A> Difference<DIFF, A>(this OptionM<A> x, OptionM<A> y) where DIFF : struct, Difference<A> =>
        (OptionM<A>)(from a in x
                     from b in y
                     select difference<DIFF, A>(a, b));

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
    public static OptionM<A> Product<PROD, A>(this OptionM<A> x, OptionM<A> y) where PROD : struct, Product<A> =>
        (OptionM<A>)(from a in x
                     from b in y
                     select product<PROD, A>(a, b));

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
    public static OptionM<A> Divide<DIV, A>(this OptionM<A> x, OptionM<A> y) where DIV : struct, Division<A> =>
        (OptionM<A>)(from a in x
                     from b in y
                     select divide<DIV, A>(a, b));

    /// Apply y to x
    /// </summary>
    [Pure]
    public static Option<B> Apply<A, B>(this Option<Func<A, B>> x, Option<A> y) =>
        from a in x
        from b in y
        select a(b);

    /// <summary>
    /// Apply y and z to x
    /// </summary>
    [Pure]
    public static Option<C> Apply<A, B, C>(this Option<Func<A, B, C>> x, Option<A> y, Option<B> z) =>
        from a in x
        from b in y
        from c in z
        select a(b, c);

    /// <summary>
    /// Apply y to x
    /// </summary>
    [Pure]
    public static Option<Func<B, C>> Apply<A, B, C>(this Option<Func<A, Func<B, C>>> x, Option<A> y) =>
        from a in x
        from b in y
        select a(b);

    /// <summary>
    /// Apply x, then y, ignoring the result of x
    /// </summary>
    [Pure]
    public static Option<B> Action<A, B>(this Option<A> x, Option<B> y) =>
        from a in x
        from b in y
        select b;

    /// <summary>
    /// Convert the Option type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Option to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static A? ToNullable<A>(this OptionM<A> ma) where A : struct =>
        ma.IsNone()
            ? (A?)null
            : ma.Value.Value();

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A non-null R</returns>
    [Pure]
    public static B Match<A, B>(this OptionM<A> ma, Func<A, B> Some, Func<B> None) =>
        ma.IsNone()
            ? CheckNullNoneReturn(None())
            : CheckNullSomeReturn(Some(ma.Value.Value()));


    /// <summary>
    /// Match the two states of the Option and return an R, which can be null.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  May return null.</param>
    /// <param name="None">None handler.  May return null.</param>
    /// <returns>R, or null</returns>
    [Pure]
    public static B MatchUnsafe<A, B>(this OptionM<A> ma, Func<A, B> Some, Func<B> None) =>
        ma.IsNone()
            ? None()
            : Some(ma.Value.Value());

    /// <summary>
    /// Match the two states of the Option and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this OptionM<A> ma, Func<A, Task<B>> Some, Func<B> None) =>
        ma.IsSome()
            ? CheckNullSomeReturn(await Some(ma.Value.Value()))
            : CheckNullNoneReturn(None());

    /// <summary>
    /// Match the two states of the Option and return a promise for a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<B> MatchAsync<A, B>(this OptionM<A> ma, Func<A, Task<B>> Some, Func<Task<B>> None) =>
        ma.IsSome()
            ? CheckNullSomeReturn(await Some(ma.Value.Value()))
            : CheckNullNoneReturn(await None());

    /// <summary>
    /// Match the two states of the Option and return an observable stream of non-null Rs.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A stream of non-null Rs</returns>
    [Pure]
    public static IObservable<B> MatchObservable<A, B>(this OptionM<A> ma, Func<A, IObservable<B>> Some, Func<B> None) =>
        ma.IsSome()
            ? Some(ma.Value.Value()).Select(CheckNullSomeReturn)
            : Observable.Return(CheckNullNoneReturn(None()));

    /// <summary>
    /// Match the two states of the Option and return an observable stream of non-null Rs.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some handler.  Must not return null.</param>
    /// <param name="None">None handler.  Must not return null.</param>
    /// <returns>A stream of non-null Rs</returns>
    [Pure]
    public static IObservable<B> MatchObservable<A, B>(this OptionM<A> ma, Func<A, IObservable<B>> Some, Func<IObservable<B>> None) =>
        ma.IsSome()
            ? Some(ma.Value.Value()).Select(CheckNullSomeReturn)
            : None().Select(CheckNullNoneReturn);

    /// <summary>
    /// Match the two states of the Option and return an R, or null.
    /// </summary>
    /// <param name="Some">Some handler.  May return null.</param>
    /// <param name="None">None handler.  May return null.</param>
    /// <returns>An R, or null</returns>
    [Pure]
    public static B MatchUntyped<B>(this OptionM<object> ma, Func<object, B> Some, Func<B> None) =>
        ma.IsSome()
            ? Some(ma.Value.Value())
            : None();

    /// <summary>
    /// Match the two states of the Option T
    /// </summary>
    /// <param name="Some">Some match</param>
    /// <param name="None">None match</param>
    /// <returns></returns>
    public static Unit Match<A>(this OptionM<A> ma, Action<A> Some, Action None)
    {
        if (ma.IsSome())
        {
            Some(ma.Value.Value());
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
    public static Unit IfSome<A>(this OptionM<A> ma, Action<A> someHandler)
    {
        if (ma.IsSome())
        {
            someHandler(ma.Value.Value());
        }
        return unit;
    }

    /// <summary>
    /// Invokes the someHandler if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit IfSome<A>(this OptionM<A> ma, Func<A, Unit> someHandler)
    {
        if (ma.IsSome())
        {
            someHandler(ma.Value.Value());
        }
        return unit;
    }

    [Pure]
    public static A IfNone<A>(this OptionM<A> ma, Func<A> None) =>
        ma.Match(identity, None);

    [Pure]
    public static A IfNone<A>(this OptionM<A> ma, A noneValue) =>
        ma.Match(identity, () => noneValue);

    [Pure]
    public static A IfNoneUnsafe<A>(this OptionM<A> ma, Func<A> None) =>
        ma.MatchUnsafe(identity, None);

    [Pure]
    public static A IfNoneUnsafe<A>(this OptionM<A> ma, A noneValue) =>
        ma.MatchUnsafe(identity, () => noneValue);

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
