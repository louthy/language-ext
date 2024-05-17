using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static class StackExtensions
{
    /// <summary>
    /// Projects the values in the stack using a map function into a new enumerable (Select in LINQ).
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="stack">Stack to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static Stck<R> Map<T, R>(this Stck<T> stack, Func<T, R> map) =>
        toStackRev(List.map(stack, map));

    /// <summary>
    /// Projects the values in the stack into a new stack using a map function, which is also given an index value
    /// (Select in LINQ - note that the order of the arguments of the map function are the other way around, here the index
    /// is the first argument).
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="stack">Stack to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static Stck<R> Map<T, R>(this Stck<T> stack, Func<int, T, R> map) =>
        toStackRev(List.map(stack, map));

    /// <summary>
    /// Removes items from the stack that do not match the given predicate (Where in LINQ)
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to filter</param>
    /// <param name="predicate">Predicate function</param>
    /// <returns>Filtered stack</returns>
    [Pure]
    public static Stck<T> Filter<T>(this Stck<T> stack, Func<T, bool> predicate) =>
        toStackRev(List.filter(stack, predicate));

    /// <summary>
    /// Applies the given function 'selector' to each element of the stack. Returns an enumerable comprised of 
    /// the results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered enumerable</returns>
    [Pure]
    public static Stck<U> Choose<T, U>(this Stck<T> stack, Func<T, Option<U>> selector) =>
        toStackRev(List.choose(stack, selector));

    /// <summary>
    /// Applies the given function 'selector' to each element of the stack. Returns an enumerable comprised of 
    /// the results for each element where the function returns Some(f(x)).
    /// An index value is passed through to the selector function also.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered enumerable</returns>
    [Pure]
    public static Stck<U> Choose<T, U>(this Stck<T> stack, Func<int, T, Option<U>> selector) =>
        toStackRev(List.choose(stack, selector));

    /// <summary>
    /// For each element of the stack, applies the given function. Concatenates all the results and 
    /// returns the combined list.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="stack">Stack to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static Stck<R> Collect<T, R>(this Stck<T> stack, Func<T, IEnumerable<R>> map) =>
        toStackRev(List.collect(stack, map));

    /// <summary>
    /// Reverses the order of the items in the stack
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static Stck<T> Rev<T>(this Stck<T> stack) =>
        toStackRev(List.rev(stack));

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the stack. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result. (Aggregate in LINQ)
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S Fold<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        List.fold(stack, state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an aggregate state through the computation. The fold function takes the state 
    /// argument, and applies the function 'folder' to it and the first element of the stack. Then, 
    /// it feeds this result into the function 'folder' along with the second element, and so on. It 
    /// returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBack<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        List.foldBack(stack, state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection whilst the predicate function 
    /// returns true for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the stack. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        List.foldWhile(stack, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns true when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the stack. Then, it feeds this result into the 
    /// function 'folder' along with the second element, and so on. It returns the final result. 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        List.foldWhile(stack, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first)
    /// whilst the predicate function returns true for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the stack. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        List.foldBackWhile(stack, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns true when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the stack. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        List.foldBackWhile(stack, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function first applies the function to the first two 
    /// elements of the stack. Then, it passes this result into the function along with the third 
    /// element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T ReduceBack<T>(Stck<T> stack, Func<T, T, T> reducer) =>
        List.reduceBack(stack, reducer);

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), threading 
    /// an accumulator argument through the computation. This function first applies the function 
    /// to the first two elements of the stack. Then, it passes this result into the function along 
    /// with the third element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T Reduce<T>(this Stck<T> stack, Func<T, T, T> reducer) =>
        List.reduce(stack, reducer);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the stack. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static Stck<S> Scan<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        toStackRev(List.scan(stack, state, folder));

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the stack. Then, it 
    /// passes this result into the function along with the second element, and so on. Finally, 
    /// it returns the list of intermediate results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static Stck<S> ScanBack<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        toStackRev(List.scanBack(stack, state, folder));

    /// <summary>
    /// Returns Some(x) for the first item in the stack that matches the predicate 
    /// provided, None otherwise.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Some(x) for the first item in the stack that matches the predicate 
    /// provided, None otherwise.</returns>
    [Pure]
    public static Option<T> Find<T>(this Stck<T> stack, Func<T, bool> pred) =>
        List.find(stack, pred);

    /// <summary>
    /// Returns the number of items in the stack
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>The number of items in the enumerable</returns>
    [Pure]
    public static int Length<T>(this Stck<T> stack) =>
        List.length(stack);

    /// <summary>
    /// Invokes an action for each item in the stack in order
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this Stck<T> stack, Action<T> action) =>
        List.iter(stack, action);

    /// <summary>
    /// Invokes an action for each item in the stack in order and supplies
    /// a running index value.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this Stck<T> stack, Action<int, T> action) =>
        List.iter(stack, action);

    /// <summary>
    /// Return an enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>An enumerable with all duplicate values removed</returns>
    [Pure]
    public static bool ForAll<T>(this Stck<T> stack, Func<T, bool> pred) =>
        List.forall(stack, pred);

    /// <summary>
    /// Return an enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>An enumerable with all duplicate values removed</returns>
    [Pure]
    public static Stck<T> Distinct<T>(this Stck<T> stack) =>
        toStackRev(List.distinct(stack));

    /// <summary>
    /// Return an enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>An enumerable with all duplicate values removed</returns>
    [Pure]
    public static Stck<T> Distinct<EQ,T>(this Stck<T> stack) where EQ : Eq<T> =>
        toStackRev(List.distinct<EQ,T>(stack));

    /// <summary>
    /// Return an enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>An enumerable with all duplicate values removed</returns>
    [Pure]
    public static Stck<T> Distinct<T, K>(this Stck<T> stack, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default(Option<Func<K, K, bool>>)) =>
        toStackRev(List.distinct(stack, keySelector, compare));

    /// <summary>
    /// Returns a new enumerable with the first 'count' items from the stack
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first 'count' items from the enumerable provided</returns>
    [Pure]
    public static Stck<T> Take<T>(this Stck<T> stack, int count) =>
        toStackRev(List.take(stack, count));

    /// <summary>
    /// Iterate the stack, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first items that match the predicate</returns>
    [Pure]
    public static Stck<T> TakeWhile<T>(this Stck<T> stack, Func<T, bool> pred) =>
        toStackRev(List.takeWhile(stack, pred));

    /// <summary>
    /// Iterate the stack, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't  An index value is also provided to the predicate function.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first items that match the predicate</returns>
    [Pure]
    public static Stck<T> TakeWhile<T>(this Stck<T> stack, Func<T, int, bool> pred) =>
        toStackRev(List.takeWhile(stack, pred));

    /// <summary>
    /// Returns true if any item in the stack matches the predicate provided
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if any item in the stack matches the predicate provided</returns>
    [Pure]
    public static bool Exists<T>(this Stck<T> stack, Func<T, bool> pred) =>
        List.exists(stack, pred);
}
