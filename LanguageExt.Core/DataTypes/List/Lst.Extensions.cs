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
        lhs.Concat(rhs);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.List.foldWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.List.foldWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.List.foldBackWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.List.foldBackWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.List.foldUntil<S, T>(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.List.foldUntil(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.List.foldBackUntil(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.List.foldBackUntil(list, state, folder, pred);

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
    public static IEnumerable<Tuple<T, U>> Zip<T, U>(this IEnumerable<T> list, IEnumerable<U> other) =>
        list.Zip(other, (t, u) => Tuple(t, u));

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
    public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> self, Func<T, IEnumerable<R>> binder)
    {
        foreach (var t in self)
        {
            foreach (var u in binder(t))
            {
                yield return u;
            }
        }
    }

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
    public static IEnumerable<B> BindEnumerable<A, B>(this Lst<A> self, Func<A, Lst<B>> binder)
    {
        foreach (var t in self)
        {
            foreach (var u in binder(t))
            {
                yield return u;
            }
        }
    }

    /// <summary>
    /// Monadic bind function for Lst that returns an IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<B> BindEnumerable<PredList, A, B>(this Lst<PredList, A> self, Func<A, Lst<PredList, B>> binder) 
        where PredList : struct, Pred<ListInfo>
    {
        foreach (var t in self)
        {
            foreach (var u in binder(t))
            {
                yield return u;
            }
        }
    }

    /// <summary>
    /// Monadic bind function for Lst that returns an IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<B> BindEnumerable<PredList, PredItemA, PredItemB, A, B>(this Lst<PredList, PredItemA, A> self, Func<A, Lst<PredList, PredItemB, B>> binder)
        where PredList : struct, Pred<ListInfo>
        where PredItemA : struct, Pred<A>
        where PredItemB : struct, Pred<B>
    {
        foreach (var t in self)
        {
            foreach (var u in binder(t))
            {
                yield return u;
            }
        }
    }

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
}
