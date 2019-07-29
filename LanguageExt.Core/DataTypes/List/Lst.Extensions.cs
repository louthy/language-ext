using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

public static class ListExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Lst<A> Flatten<A>(this Lst<Lst<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static IEnumerable<A> Flatten<A>(this IEnumerable<IEnumerable<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Converts an enumerable to a `Seq`
    /// </summary>
    public static Seq<A> ToSeq<A>(this IEnumerable<A> enumerable) =>
        Seq(enumerable);

    /// <summary>
    /// Converts an enumerable to a `Seq`
    /// </summary>
    public static Seq<A> ToSeq<A>(this IList<A> enumerable) =>
        Seq(enumerable);

    /// <summary>
    /// Converts an array to a `Seq`
    /// </summary>
    public static Seq<A> ToSeq<A>(this A[] array) =>
        Seq(array);

    /// <summary>
    /// Concatenate two enumerables (Concat in LINQ)
    /// </summary>
    /// <typeparam name="A">Enumerable item type</typeparam>
    /// <param name="lhs">First enumerable</param>
    /// <param name="rhs">Second enumerable</param>
    /// <returns>Concatenated enumerable</returns>
    [Pure]
    public static IEnumerable<A> Append<A>(this IEnumerable<A> lhs, IEnumerable<A> rhs) =>
        lhs.ConcatFast(rhs);

    /// <summary>
    /// Match empty list, or multi-item list
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="More">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    public static B Match<A, B>(this IEnumerable<A> list,
        Func<B> Empty,
        Func<Seq<A>, B> More) =>
        Seq(list).Match(Empty, More);

    /// <summary>
    /// List pattern matching
    /// </summary>
    [Pure]
    public static B Match<A, B>(this IEnumerable<A> list,
        Func<B> Empty,
        Func<A, Seq<A>, B> More) =>
        Seq(list).Match(Empty, More);

    /// <summary>
    /// List pattern matching
    /// </summary>
    [Pure]
    public static R Match<T, R>(this IEnumerable<T> list,
        Func<R> Empty,
        Func<T, R> One,
        Func<T, Seq<T>, R> More ) =>
        Seq(list).Match(Empty, One, More);

    /// <summary>
    /// Get the item at the head (first) of the list
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Head item</returns>
    [Pure]
    public static T Head<T>(this IEnumerable<T> list) =>
        LanguageExt.List.head(list);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("HeadSafe has been deprecated, please use HeadOrNone")]
    [Pure]
    public static Option<T> HeadSafe<T>(this IEnumerable<T> list) =>
        LanguageExt.List.headOrNone(list);

    /// <summary>
    /// Get the item at the head (first) of the list or None if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Optional head item</returns>
    [Pure]
    public static Option<T> HeadOrNone<T>(this IEnumerable<T> list) =>
        LanguageExt.List.headOrNone(list);

    /// <summary>
    /// Get the item at the head (first) of the list or Left if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or left</returns>
    [Pure]
    public static Either<L, R> HeadOrLeft<L, R>(this IEnumerable<R> list, L left) =>
        LanguageExt.List.headOrLeft(list, left);

    /// <summary>
    /// Get the item at the head (first) of the list or fail if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or fail</returns>
    [Pure]
    public static Validation<Fail, Success> HeadOrInvalid<Fail, Success>(this IEnumerable<Success> list, Fail fail) =>
        LanguageExt.List.headOrInvalid(list, fail);

    /// <summary>
    /// Get the item at the head (first) of the list or fail if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or fail</returns>
    [Pure]
    public static Validation<Fail, Success> HeadOrInvalid<Fail, Success>(this IEnumerable<Success> list, Seq<Fail> fail) =>
        LanguageExt.List.headOrInvalid(list, fail);

    /// <summary>
    /// Get the item at the head (first) of the list or fail if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or fail</returns>
    [Pure]
    public static Validation<MonoidFail, Fail, Success> HeadOrInvalid<MonoidFail, Fail, Success>(this IEnumerable<Success> list, Fail fail)
        where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
        LanguageExt.List.headOrInvalid<MonoidFail, Fail, Success>(list, fail);

    /// <summary>
    /// Get the tail of the list (skips the head item)
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Enumerable of T</returns>
    [Pure]
    public static IEnumerable<T> Tail<T>(this IEnumerable<T> list) =>
        LanguageExt.List.tail(list);

    /// <summary>
    /// Apply an IEnumerable of values to an IEnumerable of functions
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable of argument values</param>
    /// <returns>Returns the result of applying the IEnumerable argument values to the IEnumerable functions</returns>
    [Pure]
    public static IEnumerable<B> Apply<A, B>(this IEnumerable<Func<A, B>> fabc, IEnumerable<A> fa) =>
        ApplEnumerable<A, B>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply an IEnumerable of values to an IEnumerable of functions
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable of argument values</param>
    /// <returns>Returns the result of applying the IEnumerable argument values to the IEnumerable functions</returns>
    [Pure]
    public static IEnumerable<B> Apply<A, B>(this Func<A, B> fabc, IEnumerable<A> fa) =>
        ApplEnumerable<A, B>.Inst.Apply(new[] { fabc }, fa);

    /// <summary>
    /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerable of argument values to the 
    /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
    [Pure]
    public static IEnumerable<Func<B, C>> Apply<A, B, C>(this IEnumerable<Func<A, B, C>> fabc, IEnumerable<A> fa) =>
        ApplEnumerable<A, B, C>.Inst.Apply(fabc.Map(curry), fa);

    /// <summary>
    /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerable of argument values to the 
    /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
    [Pure]
    public static IEnumerable<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, IEnumerable<A> fa) =>
        ApplEnumerable<A, B, C>.Inst.Apply(new[] { curry(fabc) }, fa);

    /// <summary>
    /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <param name="fb">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
    [Pure]
    public static IEnumerable<C> Apply<A, B, C>(this IEnumerable<Func<A, B, C>> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
        ApplEnumerable<A, B, C>.Inst.Apply(fabc.Map(curry), fa, fb);

    /// <summary>
    /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <param name="fb">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
    [Pure]
    public static IEnumerable<C> Apply<A, B, C>(this Func<A, B, C> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
        ApplEnumerable<A, B, C>.Inst.Apply(new[] { curry(fabc) }, fa, fb);

    /// <summary>
    /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerable of argument values to the 
    /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
    [Pure]
    public static IEnumerable<Func<B, C>> Apply<A, B, C>(this IEnumerable<Func<A, Func<B, C>>> fabc, IEnumerable<A> fa) =>
        ApplEnumerable<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerable of argument values to the 
    /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
    [Pure]
    public static IEnumerable<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, IEnumerable<A> fa) =>
        ApplEnumerable<A, B, C>.Inst.Apply(new[] { fabc }, fa);

    /// <summary>
    /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <param name="fb">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
    [Pure]
    public static IEnumerable<C> Apply<A, B, C>(this IEnumerable<Func<A, Func<B, C>>> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
        ApplEnumerable<A, B, C>.Inst.Apply(fabc, fa, fb);

    /// <summary>
    /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
    /// </summary>
    /// <param name="fabc">IEnumerable of functions</param>
    /// <param name="fa">IEnumerable argument values</param>
    /// <param name="fb">IEnumerable argument values</param>
    /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
    [Pure]
    public static IEnumerable<C> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
        ApplEnumerable<A, B, C>.Inst.Apply(new[] { fabc }, fa, fb);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static IEnumerable<B> Action<A, B>(this IEnumerable<A> fa, IEnumerable<B> fb) =>
        ApplEnumerable<A, B>.Inst.Action(fa, fb);

    /// <summary>
    /// Projects the values in the enumerable using a map function into a new enumerable (Select in LINQ).
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="list">Enumerable to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static IEnumerable<R> Map<T, R>(this IEnumerable<T> list, Func<T, R> map) =>
        LanguageExt.List.map(list, map);

    /// <summary>
    /// Projects the values in the enumerable using a map function into a new enumerable (Select in LINQ).
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="list">Enumerable to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static IEnumerable<R> Map<T, R>(this IEnumerable<T> list, Func<int, T, R> map) =>
        LanguageExt.List.map(list, map);

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static IEnumerable<Func<T2, R>> ParMap<T1, T2, R>(this IEnumerable<T1> list, Func<T1, T2, R> func) =>
        list.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static IEnumerable<Func<T2, Func<T3, R>>> ParMap<T1, T2, T3, R>(this IEnumerable<T1> list, Func<T1, T2, T3, R> func) =>
        list.Map(curry(func));

    /// <summary>
    /// Removes items from the list that do not match the given predicate (Where in LINQ)
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to filter</param>
    /// <param name="predicate">Predicate function</param>
    /// <returns>Filtered enumerable</returns>
    [Pure]
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, Func<T, bool> predicate) =>
        LanguageExt.List.filter(list, predicate);

    /// <summary>
    /// Applies the given function 'selector' to each element of the list. Returns the list comprised of 
    /// the results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered enumerable</returns>
    [Pure]
    public static IEnumerable<R> Choose<T, R>(this IEnumerable<T> list, Func<T, Option<R>> selector) =>
        LanguageExt.List.choose(list, selector);

    /// <summary>
    /// Applies the given function 'selector' to each element of the list. Returns the list comprised of 
    /// the results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered enumerable</returns>
    [Pure]
    public static IEnumerable<R> Choose<T, R>(this IEnumerable<T> list, Func<int, T, Option<R>> selector) =>
        LanguageExt.List.choose(list, selector);

    /// <summary>
    /// For each element of the list, applies the given function. Concatenates all the results and 
    /// returns the combined list.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="list">Enumerable to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static IEnumerable<R> Collect<T, R>(this IEnumerable<T> list, Func<T, IEnumerable<R>> map) =>
        LanguageExt.List.collect(list, map);

    /// <summary>
    /// Reverses the enumerable (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to reverse</param>
    /// <returns>Reversed enumerable</returns>
    [Pure]
    public static IEnumerable<T> Rev<T>(this IEnumerable<T> list) =>
        LanguageExt.List.rev(list);

    /// <summary>
    /// Reverses the list (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">List item type</typeparam>
    /// <param name="list">Listto reverse</param>
    /// <returns>Reversed list</returns>
    [Pure]
    public static Lst<A> Rev<A>(this Lst<A> list) =>
        LanguageExt.List.rev(list);

    /// <summary>
    /// Reverses the list (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">List item type</typeparam>
    /// <param name="list">Listto reverse</param>
    /// <returns>Reversed list</returns>
    [Pure]
    public static Lst<PredList, A> Rev<PredList, A>(this Lst<PredList, A> list) 
        where PredList : struct, Pred<ListInfo> =>
        LanguageExt.List.rev(list);

    /// <summary>
    /// Reverses the list (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">List item type</typeparam>
    /// <param name="list">Listto reverse</param>
    /// <returns>Reversed list</returns>
    [Pure]
    public static Lst<PredList, PredItem, A> Rev<PredList, PredItem, A>(this Lst<PredList, PredItem, A> list) 
        where PredList : struct, Pred<ListInfo>
        where PredItem : struct, Pred<A> =>
        LanguageExt.List.rev(list);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the list. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result. (Aggregate in LINQ)
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S Fold<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        LanguageExt.List.fold(list, state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an aggregate state through the computation. The fold function takes the state 
    /// argument, and applies the function 'folder' to it and the first element of the list. Then, 
    /// it feeds this result into the function 'folder' along with the second element, and so on. It 
    /// returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBack<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        LanguageExt.List.foldBack(list, state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection whilst the predicate function 
    /// returns True for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the list. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.List.foldWhile(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns True when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the list. Then, it feeds this result into the 
    /// function 'folder' along with the second element, and so on. It returns the final result. 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.List.foldWhile(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first)
    /// whilst the predicate function returns True for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the list. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.List.foldBackWhile(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns True when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the list. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.List.foldBackWhile(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection whilst the predicate function 
    /// returns False for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the list. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.List.foldUntil<S, T>(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns False when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the list. Then, it feeds this result into the 
    /// function 'folder' along with the second element, and so on. It returns the final result. 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.List.foldUntil(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first)
    /// whilst the predicate function returns False for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the list. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.List.foldBackUntil(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns False when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the list. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.List.foldBackUntil(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), threading 
    /// an accumulator argument through the computation. This function first applies the function 
    /// to the first two elements of the list. Then, it passes this result into the function along 
    /// with the third element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to reduce</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T Reduce<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
        LanguageExt.List.reduce(list, reducer);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function first applies the function to the first two 
    /// elements of the list. Then, it passes this result into the function along with the third 
    /// element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to reduce</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T ReduceBack<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
        LanguageExt.List.reduceBack(list, reducer);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the list. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static IEnumerable<S> Scan<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scan(list, state, folder);

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the list. Then, it 
    /// passes this result into the function along with the second element, and so on. Finally, 
    /// it returns the list of intermediate results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static IEnumerable<S> ScanBack<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scanBack(list, state, folder);

    /// <summary>
    /// Joins two enumerables together either into a single enumerable of tuples
    /// </summary>
    /// <param name="list">First list to join</param>
    /// <param name="other">Second list to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined enumerable</returns>
    [Pure]
    public static IEnumerable<(T, U)> Zip<T, U>(this IEnumerable<T> list, IEnumerable<U> other) =>
        list.Zip(other, (t, u) => (t, u));

    /// <summary>
    /// Returns Some(x) for the first item in the list that matches the predicate 
    /// provided, None otherwise.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to search</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Some(x) for the first item in the list that matches the predicate 
    /// provided, None otherwise.</returns>
    [Pure]
    public static Option<T> Find<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
        LanguageExt.List.find(list, pred);

    /// <summary>
    /// Returns [x] for the first item in the list that matches the predicate 
    /// provided, [] otherwise.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to search</param>
    /// <param name="pred">Predicate</param>
    /// <returns>[x] for the first item in the list that matches the predicate 
    /// provided, [] otherwise.</returns>
    [Pure]
    public static IEnumerable<T> FindSeq<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
        LanguageExt.List.findSeq(list, pred);

    /// <summary>
    /// Convert any enumerable into an immutable Lst T
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to convert</param>
    /// <returns>Lst of T</returns>
    [Pure]
    public static Lst<T> Freeze<T>(this IEnumerable<T> list) =>
        LanguageExt.List.freeze(list);

    /// <summary>
    /// Convert any enumerable into an immutable Lst T
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to convert</param>
    /// <returns>Lst of T</returns>
    [Pure]
    public static Lst<PredList, T> Freeze<PredList, T>(this IEnumerable<T> list) where PredList : struct, Pred<ListInfo> =>
        LanguageExt.List.freeze<PredList, T>(list);

    /// <summary>
    /// Convert any enumerable into an immutable Lst T
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to convert</param>
    /// <returns>Lst of T</returns>
    [Pure]
    public static Lst<PredList, PredItem, T> Freeze<PredList, PredItem, T>(this IEnumerable<T> list) 
        where PredItem : struct, Pred<T>
        where PredList : struct, Pred<ListInfo> =>
        LanguageExt.List.freeze<PredList, PredItem, T>(list);

    /// <summary>
    /// Convert the enumerable to an immutable array
    /// </summary>
    [Pure]
    public static Arr<A> ToArr<A>(this IEnumerable<A> list) =>
        new Arr<A>(list);

    /// <summary>
    /// Returns the number of items in the Lst T
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="list">List to count</param>
    /// <returns>The number of items in the list</returns>
    [Pure]
    public static int Length<T>(this IEnumerable<T> list) =>
        LanguageExt.List.length(list);

    /// <summary>
    /// Invokes an action for each item in the enumerable in order
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this IEnumerable<T> list, Action<T> action) =>
        LanguageExt.List.iter(list, action);

    /// <summary>
    /// Invokes an action for each item in the enumerable in order
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this IEnumerable<T> list, Action<int, T> action) =>
        LanguageExt.List.iter(list, action);

    /// <summary>
    /// Returns true if all items in the enumerable match a predicate (Any in LINQ)
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to test</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the enumerable match the predicate</returns>
    [Pure]
    public static bool ForAll<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
        LanguageExt.List.forall(list, pred);

    /// <summary>
    /// Return a new enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <returns>A new enumerable with all duplicate values removed</returns>
    [Pure]
    public static IEnumerable<T> Distinct<EQ, T>(this IEnumerable<T> list) where EQ : struct, Eq<T> =>
        LanguageExt.List.distinct<EQ, T>(list);

    /// <summary>
    /// Returns true if any item in the enumerable matches the predicate provided
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to test</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if any item in the enumerable matches the predicate provided</returns>
    [Pure]
    public static bool Exists<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
        LanguageExt.List.exists(list, pred);

    /// <summary>
    /// The tails function returns all final segments of the argument, longest first. For example,
    ///  i.e. tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="self">List</param>
    /// <returns>Enumerable of Enumerables of T</returns>
    [Pure]
    public static IEnumerable<IEnumerable<T>> Tails<T>(this IEnumerable<T> self) =>
        LanguageExt.List.tails(self);

    /// <summary>
    /// Span, applied to a predicate 'pred' and a list, returns a tuple where first element is 
    /// longest prefix (possibly empty) of elements that satisfy 'pred' and second element is the 
    /// remainder of the list:
    /// </summary>
    /// <example>
    /// List.span(List(1,2,3,4,1,2,3,4), x => x &lt; 3) == (List(1,2),List(3,4,1,2,3,4))
    /// </example>
    /// <example>
    /// List.span(List(1,2,3), x => x &lt; 9) == (List(1,2,3),List())
    /// </example>
    /// <example>
    /// List.span(List(1,2,3), x => x &lt; 0) == (List(),List(1,2,3))
    /// </example>
    /// <typeparam name="T">List element type</typeparam>
    /// <param name="self">List</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Split list</returns>
    [Pure]
    public static (IEnumerable<T>, IEnumerable<T>) Span<T>(this IEnumerable<T> self, Func<T, bool> pred) =>
        LanguageExt.List.span(self, pred);

    /// <summary>
    /// Monadic bind function for IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> self, Func<T, IEnumerable<R>> binder) =>
        EnumerableOptimal.BindFast(self, binder);

    /// <summary>
    /// LINQ Select implementation for Lst
    /// </summary>
    [Pure]
    public static Lst<B> Select<A, B>(this Lst<A> self, Func<A, B> map) =>
        new Lst<B>(self.AsEnumerable().Select(map));

    /// <summary>
    /// LINQ Select implementation for Lst
    /// </summary>
    [Pure]
    public static Lst<PredList, B> Select<PredList, A, B>(this Lst<PredList, A> self, Func<A, B> map)
        where PredList : struct, Pred<ListInfo> =>
        new Lst<PredList, B>(self.AsEnumerable().Select(map));

    /// <summary>
    /// Monadic bind function for Lst that returns an IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<B> BindEnumerable<A, B>(this Lst<A> self, Func<A, Lst<B>> binder) =>
        EnumerableOptimal.BindFast(self, binder);

    /// <summary>
    /// Monadic bind function for Lst that returns an IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<B> BindEnumerable<PredList, A, B>(this Lst<PredList, A> self, Func<A, Lst<PredList, B>> binder) 
        where PredList : struct, Pred<ListInfo> =>
        EnumerableOptimal.BindFast<PredList, A, B>(self, binder);

    /// <summary>
    /// Monadic bind function for Lst that returns an IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<B> BindEnumerable<PredList, PredItemA, PredItemB, A, B>(this Lst<PredList, PredItemA, A> self, Func<A, Lst<PredList, PredItemB, B>> binder)
        where PredList : struct, Pred<ListInfo>
        where PredItemA : struct, Pred<A>
        where PredItemB : struct, Pred<B> =>
        EnumerableOptimal.BindFast<PredList, PredItemA, PredItemB, A, B>(self, binder);

    /// <summary>
    /// Monadic bind function
    /// </summary>
    [Pure]
    public static Lst<B> Bind<A, B>(this Lst<A> self, Func<A, Lst<B>> binder) =>
        new Lst<B>(self.BindEnumerable(binder));

    /// <summary>
    /// Monadic bind function
    /// </summary>
    [Pure]
    public static Lst<PredList, B> Bind<PredList, A, B>(this Lst<PredList, A> self, Func<A, Lst<PredList, B>> binder)
        where PredList : struct, Pred<ListInfo> =>
        new Lst<PredList, B>(self.BindEnumerable(binder));

    /// <summary>
    /// Monadic bind function
    /// </summary>
    [Pure]
    public static Lst<PredList, PredItemB, B> Bind<PredList, PredItemA, PredItemB, A, B>(this Lst<PredList, PredItemA, A> self, Func<A, Lst<PredList, PredItemB, B>> binder)
        where PredList : struct, Pred<ListInfo>
        where PredItemA : struct, Pred<A>
        where PredItemB : struct, Pred<B> =>
        new Lst<PredList, PredItemB, B>(self.BindEnumerable(binder));

    /// <summary>
    /// Returns the number of items in the Lst T
    /// </summary>
    /// <typeparam name="A">Item type</typeparam>
    /// <param name="list">List to count</param>
    /// <returns>The number of items in the list</returns>
    [Pure]
    public static int Count<A>(this Lst<A> self) =>
        self.Count;

    /// <summary>
    /// Returns the number of items in the Lst T
    /// </summary>
    /// <typeparam name="A">Item type</typeparam>
    /// <param name="list">List to count</param>
    /// <returns>The number of items in the list</returns>
    [Pure]
    public static int Count<PredList, A>(this Lst<PredList, A> self) where PredList : struct, Pred<ListInfo> =>
        self.Count;

    /// <summary>
    /// Returns the number of items in the Lst T
    /// </summary>
    /// <typeparam name="A">Item type</typeparam>
    /// <param name="list">List to count</param>
    /// <returns>The number of items in the list</returns>
    [Pure]
    public static int Count<PredList, PredItem, A>(this Lst<PredList, PredItem, A> self) 
        where PredList : struct, Pred<ListInfo>
        where PredItem : struct, Pred<A> =>
        self.Count;

    /// <summary>
    /// LINQ bind implementation for Lst
    /// </summary>
    [Pure]
    public static Lst<C> SelectMany<A, B, C>(this Lst<A> self, Func<A, Lst<B>> bind, Func<A, B, C> project) =>
        self.Bind(t => bind(t).Map(u => project(t, u)));

    /// <summary>
    /// LINQ bind implementation for Lst
    /// </summary>
    [Pure]
    public static Lst<PredList, C> SelectMany<PredList, A, B, C>(this Lst<PredList, A> self, Func<A, Lst<PredList, B>> bind, Func<A, B, C> project) 
        where PredList : struct, Pred<ListInfo> =>
        self.Bind(t => bind(t).Map(u => project(t, u)));

    /// <summary>
    /// Take all but the last item in an enumerable
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> self)
    {
        var iter = self.GetEnumerator();
        bool remaining = false;
        bool first = true;
        T item = default(T);

        do
        {
            remaining = iter.MoveNext();
            if (remaining)
            {
                if (!first) yield return item;
                item = iter.Current;
                first = false;
            }
        } while (remaining);
    }

    /// <summary>
    /// Take all but the last n items in an enumerable
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> self, int n)
    {
        var iter = self.GetEnumerator();
        bool remaining = false;
        var cache = new Queue<T>(n + 1);

        do
        {
            if (remaining = iter.MoveNext())
            {
                cache.Enqueue(iter.Current);
                if (cache.Count > n) yield return cache.Dequeue();
            }
        } while (remaining);
    }

    /// <summary>
    /// Convert the enumerable to an Option.  
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="self">This</param>
    /// <returns>If enumerable is empty then return None, else Some(head)</returns>
    public static Option<A> ToOption<A>(this IEnumerable<A> self) =>
        self.Match(
            ()     => Option<A>.None,
            (x, _) => Option<A>.Some(x));

    /// <summary>
    /// Convert the enumerable to an Option.  
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="self">This</param>
    /// <returns>If enumerable is empty then return None, else Some(head)</returns>
    public static TryOption<A> ToTryOption<A>(this IEnumerable<A> self) => () =>
        self.Match(
            () => Option<A>.None,
            (x, _) => Option<A>.Some(x));

    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.
    /// </summary>
    [Pure]
    public static TAccumulate Aggregate<TSource, TAccumulate>(this Lst<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func) =>
        Enumerable.Aggregate(source.Value, seed, func);

    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.
    /// </summary>
    [Pure]
    public static TResult Aggregate<TSource, TAccumulate, TResult>(this Lst<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) =>
        Enumerable.Aggregate(source.Value, seed, func, resultSelector);

    /// <summary>
    /// Applies an accumulator function over a sequence.
    /// </summary>
    [Pure]
    public static TSource Aggregate<TSource>(this Lst<TSource> source, Func<TSource, TSource, TSource> func) =>
        Enumerable.Aggregate(source.Value, func);

    /// <summary>
    /// Determines whether all elements of a sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static bool All<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.All(source.Value, predicate);

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    [Pure]
    public static bool Any<TSource>(this Lst<TSource> source) =>
        Enumerable.Any(source.Value);

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    [Pure]
    public static bool Any<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Any(source.Value, predicate);

    /// <summary>
    /// Returns the input typed as IEnumerable<T>.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> AsEnumerable<TSource>(this Lst<TSource> source) =>
        Enumerable.AsEnumerable(source.Value);

    /// <summary>
    /// Converts a generic IEnumerable<T> to a generic IQueryable<T>.
    /// </summary>
    [Pure]
    public static IQueryable<TElement> AsQueryable<TElement>(this Lst<TElement> source) =>
        Queryable.AsQueryable(source.Value.AsQueryable());

    /// <summary>
    /// Computes the average of a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Average(this Lst<decimal> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal Average<TSource>(this Lst<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Average(this Lst<decimal?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal? Average<TSource>(this Lst<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Average(this Lst<double> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static double Average(this Lst<int> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static double Average(this Lst<long> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Lst<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Lst<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Lst<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Average(this Lst<double?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static double? Average(this Lst<int?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static double? Average(this Lst<long?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Lst<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Lst<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Lst<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Average(this Lst<float> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float Average<TSource>(this Lst<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Average(this Lst<float?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float? Average<TSource>(this Lst<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Concatenates two sequences.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Concat<TSource>(this Lst<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Concat(first.Value, second);

    /// <summary>
    /// Determines whether a sequence contains a specified element by using the default equality comparer.
    /// </summary>
    [Pure]
    public static bool Contains<TSource>(this Lst<TSource> source, TSource value) =>
        Enumerable.Contains(source.Value, value);

    /// <summary>
    /// Determines whether a sequence contains a specified element by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static bool Contains<TSource>(this Lst<TSource> source, TSource value, IEqualityComparer<TSource> comparer) =>
        Enumerable.Contains(source.Value, value, comparer);

    /// <summary>
    /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static int Count<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Count(source.Value, predicate);

    /// <summary>
    /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this Lst<TSource> source) =>
        Enumerable.DefaultIfEmpty(source.Value);

    /// <summary>
    /// Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this Lst<TSource> source, TSource defaultValue) =>
        Enumerable.DefaultIfEmpty(source.Value, defaultValue);

    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Distinct<TSource>(this Lst<TSource> source) =>
        Enumerable.Distinct(source.Value);

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Distinct<TSource>(this Lst<TSource> source, IEqualityComparer<TSource> comparer) =>
        Enumerable.Distinct(source.Value, comparer);

    /// <summary>
    /// Returns the element at a specified index in a sequence.
    /// </summary>
    [Pure]
    public static TSource ElementAt<TSource>(this Lst<TSource> source, int index) =>
        Enumerable.ElementAt(source.Value, index);

    /// <summary>
    /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
    /// </summary>
    [Pure]
    public static TSource ElementAtOrDefault<TSource>(this Lst<TSource> source, int index) =>
        Enumerable.ElementAtOrDefault(source.Value, index);

    /// <summary>
    /// Produces the set difference of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Except<TSource>(this Lst<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Except(first.Value, second);

    /// <summary>
    /// Produces the set difference of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Except<TSource>(this Lst<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Except(first.Value, second, comparer);

    /// <summary>
    /// Returns the first element of a sequence.
    /// </summary>
    [Pure]
    public static TSource First<TSource>(this Lst<TSource> source) =>
        Enumerable.First(source.Value);

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static TSource First<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.First(source.Value, predicate);

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static TSource FirstOrDefault<TSource>(this Lst<TSource> source) =>
        Enumerable.FirstOrDefault(source.Value);

    /// <summary>
    /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static TSource FirstOrDefault<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.FirstOrDefault(source.Value, predicate);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.GroupBy(source.Value, keySelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector, resultSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector) =>
        Enumerable.GroupBy(source.Value, keySelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, resultSelector, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Lst<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector) =>
        Enumerable.GroupJoin(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on key equality and groups the results. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Lst<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupJoin(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Intersect<TSource>(this Lst<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Intersect(first.Value, second);

    /// <summary>
    /// Produces the set intersection of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Intersect<TSource>(this Lst<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Intersect(first.Value, second, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this Lst<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector) =>
        Enumerable.Join(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this Lst<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.Join(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Returns the last element of a sequence.
    /// </summary>
    [Pure]
    public static TSource Last<TSource>(this Lst<TSource> source) =>
        Enumerable.Last(source.Value);

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static TSource Last<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Last(source.Value, predicate);

    /// <summary>
    /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static TSource LastOrDefault<TSource>(this Lst<TSource> source) =>
        Enumerable.LastOrDefault(source.Value);

    /// <summary>
    /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static TSource LastOrDefault<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.LastOrDefault(source.Value, predicate);

    /// <summary>
    /// Returns an Int64 that represents the total number of elements in a sequence.
    /// </summary>
    [Pure]
    public static long LongCount<TSource>(this Lst<TSource> source) =>
        Enumerable.LongCount(source.Value);

    /// <summary>
    /// Returns an Int64 that represents how many elements in a sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static long LongCount<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.LongCount(source.Value, predicate);

    /// <summary>
    /// Returns the maximum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Max(this Lst<decimal> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Max<TSource>(this Lst<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Max(this Lst<decimal?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Max<TSource>(this Lst<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Max(this Lst<double> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Double value.
    /// </summary>
    [Pure]
    public static double Max<TSource>(this Lst<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Max(this Lst<double?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Max<TSource>(this Lst<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Max(this Lst<float> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Single value.
    /// </summary>
    [Pure]
    public static float Max<TSource>(this Lst<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Max(this Lst<float?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Max<TSource>(this Lst<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Max(this Lst<int> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int32 value.
    /// </summary>
    [Pure]
    public static int Max<TSource>(this Lst<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Max(this Lst<int?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Max<TSource>(this Lst<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Max(this Lst<long> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int64 value.
    /// </summary>
    [Pure]
    public static long Max<TSource>(this Lst<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Max(this Lst<long?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Max<TSource>(this Lst<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.
    /// </summary>
    [Pure]
    public static TResult Max<TSource, TResult>(this Lst<TSource> source, Func<TSource, TResult> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a generic sequence.
    /// </summary>
    [Pure]
    public static TSource Max<TSource>(this Lst<TSource> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Returns the minimum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Min(this Lst<decimal> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Min<TSource>(this Lst<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Min(this Lst<decimal?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Min<TSource>(this Lst<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Min(this Lst<double> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Double value.
    /// </summary>
    [Pure]
    public static double Min<TSource>(this Lst<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Min(this Lst<double?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Min<TSource>(this Lst<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Min(this Lst<float> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Single value.
    /// </summary>
    [Pure]
    public static float Min<TSource>(this Lst<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Min(this Lst<float?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Min<TSource>(this Lst<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Min(this Lst<int> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int32 value.
    /// </summary>
    [Pure]
    public static int Min<TSource>(this Lst<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Min(this Lst<int?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Min<TSource>(this Lst<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Min(this Lst<long> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int64 value.
    /// </summary>
    [Pure]
    public static long Min<TSource>(this Lst<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Min(this Lst<long?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Min<TSource>(this Lst<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.
    /// </summary>
    [Pure]
    public static TResult Min<TSource, TResult>(this Lst<TSource> source, Func<TSource, TResult> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a generic sequence.
    /// </summary>
    [Pure]
    public static TSource Min<TSource>(this Lst<TSource> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.OrderBy(source.Value, keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
        Enumerable.OrderBy(source.Value, keySelector, comparer);

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.OrderByDescending(source.Value, keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
        Enumerable.OrderByDescending(source.Value, keySelector, comparer);

    /// <summary>
    /// Inverts the order of the elements in a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Reverse<TSource>(this Lst<TSource> source) =>
        Enumerable.Reverse(source.Value);

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<TSource>(this Lst<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.SequenceEqual(first.Value, second);

    /// <summary>
    /// Determines whether two sequences are equal by comparing their elements by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<TSource>(this Lst<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.SequenceEqual(first.Value, second, comparer);

    /// <summary>
    /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    [Pure]
    public static TSource Single<TSource>(this Lst<TSource> source) =>
        Enumerable.Single(source.Value);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
    /// </summary>
    [Pure]
    public static TSource Single<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Single(source.Value, predicate);

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    [Pure]
    public static TSource SingleOrDefault<TSource>(this Lst<TSource> source) =>
        Enumerable.SingleOrDefault(source.Value);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    [Pure]
    public static TSource SingleOrDefault<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.SingleOrDefault(source.Value, predicate);

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Skip<TSource>(this Lst<TSource> source, int count) =>
        Enumerable.Skip(source.Value, count);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> SkipWhile<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.SkipWhile(source.Value, predicate);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> SkipWhile<TSource>(this Lst<TSource> source, Func<TSource, int, bool> predicate) =>
        Enumerable.SkipWhile(source.Value, predicate);

    /// <summary>
    /// Computes the sum of a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Sum(this Lst<decimal> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal Sum<TSource>(this Lst<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Sum(this Lst<decimal?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal? Sum<TSource>(this Lst<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Sum(this Lst<double> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Sum<TSource>(this Lst<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Sum(this Lst<double?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Sum<TSource>(this Lst<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Sum(this Lst<float> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float Sum<TSource>(this Lst<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Sum(this Lst<float?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float? Sum<TSource>(this Lst<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Sum(this Lst<int> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int Sum<TSource>(this Lst<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Sum(this Lst<int?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int? Sum<TSource>(this Lst<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Sum(this Lst<long> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long Sum<TSource>(this Lst<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Sum(this Lst<long?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long? Sum<TSource>(this Lst<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Take<TSource>(this Lst<TSource> source, int count) =>
        Enumerable.Take(source.Value, count);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> TakeWhile<TSource>(this Lst<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.TakeWhile(source.Value, predicate);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> TakeWhile<TSource>(this Lst<TSource> source, Func<TSource, int, bool> predicate) =>
        Enumerable.TakeWhile(source.Value, predicate);

    /// <summary>
    /// Creates an array from a IEnumerable<T>.
    /// </summary>
    [Pure]
    public static TSource[] ToArray<TSource>(this Lst<TSource> source) =>
        Enumerable.ToArray(source.Value);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.ToDictionary(source.Value, keySelector, elementSelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToDictionary(source.Value, keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.ToDictionary(source.Value, keySelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToDictionary(source.Value, keySelector, comparer);

    /// <summary>
    /// Creates a List<T> from an IEnumerable<T>.
    /// </summary>
    [Pure]
    public static List<TSource> ToList<TSource>(this Lst<TSource> source) =>
        Enumerable.ToList(source.Value);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.ToLookup(source.Value, keySelector, elementSelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function, a comparer and an element selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this Lst<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToLookup(source.Value, keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.ToLookup(source.Value, keySelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this Lst<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToLookup(source.Value, keySelector, comparer);

    /// <summary>
    /// Produces the set union of two sequences by using the default equality comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Union<TSource>(this Lst<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Union(first.Value, second);

    /// <summary>
    /// Produces the set union of two sequences by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Union<TSource>(this Lst<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Union(first.Value, second, comparer);

    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this Lst<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) =>
        Enumerable.Zip(first.Value, second, resultSelector);
}
