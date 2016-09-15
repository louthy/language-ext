using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class List
    {
        /// <summary>
        /// Create an empty IEnumerable T
        /// </summary>
        [Pure]
        public static Lst<T> empty<T>() =>
            new Lst<T>();

        /// <summary>
        /// Create a new empty list
        /// </summary>
        /// <returns>Lst T</returns>
        [Pure]
        public static Lst<T> create<T>() =>
            new Lst<T>();

        /// <summary>
        /// Create a list from a initial set of items
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Lst T</returns>
        [Pure]
        public static Lst<T> create<T>(params T[] items) =>
            new Lst<T>(items);

        /// <summary>
        /// Create a list from an initial set of items
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Lst T</returns>
        [Pure]
        public static Lst<T> createRange<T>(IEnumerable<T> items) =>
            new Lst<T>(items);

        /// <summary>
        /// Generates a sequence of T using the provided delegate to initialise
        /// each item.
        /// </summary>
        [Pure]
        public static IEnumerable<T> init<T>(int count, Func<int, T> generator) =>
            from i in Range(0, count)
            select generator(i);

        /// <summary>
        /// Generates an infinite sequence of T using the provided delegate to initialise
        /// each item.
        /// 
        ///   Remarks: Not truly infinite, will end at Int32.MaxValue
        /// 
        /// </summary>
        [Pure]
        public static IEnumerable<T> initInfinite<T>(Func<int, T> generator) =>
            from i in Range(0, Int32.MaxValue)
            select generator(i);

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        [Pure]
        public static IEnumerable<T> repeat<T>(T item, int count) =>
            from _ in Range(0, count)
            select item;

        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">Item to add</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Lst<T> add<T>(Lst<T> list, T value) =>
            list.Add(value);

        /// <summary>
        /// Add a range of items to the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">Items to add</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Lst<T> addRange<T>(Lst<T> list, IEnumerable<T> value) =>
            list.AddRange(value);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">value to remove</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Lst<T> remove<T>(Lst<T> list, T value) =>
            list.Remove(value);

        /// <summary>
        /// Remove an item at a specified index in the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="index">Index of item to remove</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Lst<T> removeAt<T>(Lst<T> list, int index) =>
            list.RemoveAt(index);

        /// <summary>
        /// Get the item at the head (first) of the list
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Head item</returns>
        [Pure]
        public static T head<T>(IEnumerable<T> list) => list.First();

        /// <summary>
        /// Get the item at the head (first) of the list or None if the list is empty
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Optional head item</returns>
        [Pure]
        public static Option<T> headOrNone<T>(IEnumerable<T> list) =>
            (from x in list
             select Some(x))
            .DefaultIfEmpty(None)
            .FirstOrDefault();

        /// <summary>
        /// Get the tail of the list (skips the head item)
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Enumerable of T</returns>
        [Pure]
        public static IEnumerable<T> tail<T>(IEnumerable<T> list) =>
            list.Skip(1);

        /// <summary>
        /// Projects the values in the enumerable using a map function into a new enumerable (Select in LINQ).
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <typeparam name="R">Return enumerable item type</typeparam>
        /// <param name="list">Enumerable to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static IEnumerable<R> map<T, R>(IEnumerable<T> list, Func<T, R> map) =>
            list.Select(map);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static IEnumerable<Func<T2, R>> parmap<T1, T2, R>(IEnumerable<T1> list, Func<T1, T2, R> func) =>
            list.Map(curry(func));

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static IEnumerable<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(IEnumerable<T1> list, Func<T1, T2, T3, R> func) =>
            list.Map(curry(func));

        /// <summary>
        /// Projects the values in the enumerable using a map function into a new enumerable (Select in LINQ).
        /// An index value is passed through to the map function also.
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <typeparam name="R">Return enumerable item type</typeparam>
        /// <param name="list">Enumerable to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static IEnumerable<R> map<T, R>(IEnumerable<T> list, Func<int, T, R> map) =>
            zip(list, Range(0, Int32.MaxValue), (t, i) => map(i, t));

        /// <summary>
        /// Removes items from the list that do not match the given predicate (Where in LINQ)
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to filter</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public static IEnumerable<T> filter<T>(IEnumerable<T> list, Func<T, bool> predicate) =>
            list.Where(predicate);

        /// <summary>
        /// Applies the given function 'selector' to each element of the list. Returns the list comprised of 
        /// the results for each element where the function returns Some(f(x)).
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <param name="selector">Selector function</param>
        /// <returns>Mapped and filtered enumerable</returns>
        [Pure]
        public static IEnumerable<T> choose<T>(IEnumerable<T> list, Func<T, Option<T>> selector) =>
            map(filter(map(list, selector), t => t.IsSome), t => t.Value);

        /// <summary>
        /// Applies the given function 'selector' to each element of the list. Returns the list comprised of 
        /// the results for each element where the function returns Some(f(x)).
        /// An index value is passed through to the selector function also.
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <param name="selector">Selector function</param>
        /// <returns>Mapped and filtered enumerable</returns>
        [Pure]
        public static IEnumerable<T> choose<T>(IEnumerable<T> list, Func<int, T, Option<T>> selector) =>
            map(filter(map(list, selector), t => t.IsSome), t => t.Value);

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
        public static IEnumerable<R> collect<T, R>(IEnumerable<T> list, Func<T, IEnumerable<R>> map) =>
            from t in list
            from r in map(t)
            select r;

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        public static int sum(IEnumerable<int> list) =>
            fold(list, 0, (s, x) => s + x);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        public static float sum(IEnumerable<float> list) =>
            fold(list, 0.0f, (s, x) => s + x);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        public static double sum(IEnumerable<double> list) =>
            fold(list, 0.0, (s, x) => s + x);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        public static decimal sum(IEnumerable<decimal> list) =>
            fold(list, (decimal)0, (s, x) => s + x);

        /// <summary>
        /// Reverses the enumerable (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to reverse</param>
        /// <returns>Reversed enumerable</returns>
        [Pure]
        public static IEnumerable<T> rev<T>(IEnumerable<T> list) =>
            list.Reverse();

        /// <summary>
        /// Reverses the list (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">List item type</typeparam>
        /// <param name="list">List to reverse</param>
        /// <returns>Reversed list</returns>
        [Pure]
        public static Lst<T> rev<T>(Lst<T> list) =>
            list.Reverse();

        /// <summary>
        /// Concatenate two enumerables (Concat in LINQ)
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="lhs">First enumerable</param>
        /// <param name="rhs">Second enumerable</param>
        /// <returns>Concatenated enumerable</returns>
        [Pure]
        public static IEnumerable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            lhs.Concat(rhs);

        /// <summary>
        /// Concatenate an enumerable and an enumerable of enumerables
        /// </summary>
        /// <typeparam name="T">List item type</typeparam>
        /// <param name="lhs">First list</param>
        /// <param name="rhs">Second list</param>
        /// <returns>Concatenated list</returns>
        [Pure]
        public static IEnumerable<T> append<T>(IEnumerable<T> x, IEnumerable<IEnumerable<T>> xs) =>
            headOrNone(xs).IsNone
                ? x
                : append(x, append(xs.First(), xs.Skip(1)));

        /// <summary>
        /// Concatenate N enumerables
        /// </summary>
        /// <typeparam name="T">Enumerable type</typeparam>
        /// <param name="lists">Enumerables to concatenate</param>
        /// <returns>A single enumerable with all of the items concatenated</returns>
        [Pure]
        public static IEnumerable<T> append<T>(params IEnumerable<T>[] lists) =>
            lists.Length == 0
                ? new T[0]
                : lists.Length == 1
                    ? lists[0]
                    : append(lists[0], lists.Skip(1));

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
        public static S fold<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder)
        {
            foreach (var item in list)
            {
                state = folder(state, item);
            }
            return state;
        }

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
        public static S foldBack<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder) =>
            fold(rev(list), state, folder);

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
        public static S foldWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred)
        {
            foreach (var item in list)
            {
                if (!pred(item))
                {
                    return state;
                }
                state = folder(state, item);
            }
            return state;
        }

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
        public static S foldWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred)
        {
            foreach (var item in list)
            {
                if (!pred(state))
                {
                    return state;
                }
                state = folder(state, item);
            }
            return state;
        }

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
        public static S foldBackWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
            foldWhile(rev(list), state, folder, pred);

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
        public static S foldBackWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
            foldWhile(rev(list), state, folder, pred);

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
        public static S foldUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred)
        {
            foreach (var item in list)
            {
                if (pred(item))
                {
                    return state;
                }
                state = folder(state, item);
            }
            return state;
        }

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
        public static S foldUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred)
        {
            foreach (var item in list)
            {
                if (pred(state))
                {
                    return state;
                }
                state = folder(state, item);
            }
            return state;
        }

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
        public static S foldBackUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
            foldWhile(rev(list), state, folder, pred);

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
        public static S foldBackUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
            foldWhile(rev(list), state, folder, pred);

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
        public static T reduce<T>(IEnumerable<T> list, Func<T, T, T> reducer) =>
            match(headOrNone(list),
                Some: x => fold(tail(list), x, reducer),
                None: () => failwith<T>("Input list was empty")
            );

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
        public static T reduceBack<T>(IEnumerable<T> list, Func<T, T, T> reducer) =>
            reduce(rev(list), reducer);

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
        public static IEnumerable<S> scan<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder)
        {
            yield return state;
            foreach (var item in list)
            {
                state = folder(state, item);
                yield return state;
            }
        }

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
        public static IEnumerable<S> scanBack<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder) =>
            scan(rev(list), state, folder);

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
        public static Option<T> find<T>(IEnumerable<T> list, Func<T, bool> pred)
        {
            foreach (var item in list)
            {
                if (pred(item)) return Some(item);
            }
            return None;
        }

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
        public static IEnumerable<T> findSeq<T>(IEnumerable<T> list, Func<T, bool> pred)
        {
            foreach (var item in list)
            {
                if (pred(item))
                {
                    yield return item;
                    break;
                }
            }
        }

        /// <summary>
        /// Convert any enumerable into an immutable Lst T
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to convert</param>
        /// <returns>Lst of T</returns>
        [Pure]
        public static Lst<T> freeze<T>(IEnumerable<T> list) =>
            toList(list);

        /// <summary>
        /// Joins two enumerables together either into a single enumerable
        /// using the join function provided
        /// </summary>
        /// <param name="list">First list to join</param>
        /// <param name="other">Second list to join</param>
        /// <param name="zipper">Join function</param>
        /// <returns>Joined enumerable</returns>
        [Pure]
        public static IEnumerable<V> zip<T, U, V>(IEnumerable<T> list, IEnumerable<U> other, Func<T, U, V> zipper) =>
            list.Zip(other, zipper);

        /// <summary>
        /// Joins two enumerables together either into an enumerables of tuples
        /// </summary>
        /// <param name="list">First list to join</param>
        /// <param name="other">Second list to join</param>
        /// <param name="zipper">Join function</param>
        /// <returns>Joined enumerable of tuples</returns>
        [Pure]
        public static IEnumerable<Tuple<T, U>> zip<T, U>(IEnumerable<T> list, IEnumerable<U> other) =>
            list.Zip(other, (t, u) => Tuple(t, u));

        /// <summary>
        /// Returns the number of items in the enumerable
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to count</param>
        /// <returns>The number of items in the enumerable</returns>
        [Pure]
        public static int length<T>(IEnumerable<T> list) =>
           list.Count();

        /// <summary>
        /// Invokes an action for each item in the enumerable in order
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to iterate</param>
        /// <param name="action">Action to invoke with each item</param>
        /// <returns>Unit</returns>
        public static Unit iter<T>(IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return unit;
        }

        /// <summary>
        /// Invokes an action for each item in the enumerable in order and supplies
        /// a running index value.
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to iterate</param>
        /// <param name="action">Action to invoke with each item</param>
        /// <returns>Unit</returns>
        public static Unit iter<T>(IEnumerable<T> list, Action<int, T> action)
        {
            int i = 0;
            foreach (var item in list)
            {
                action(i++, item);
            }
            return unit;
        }

        /// <summary>
        /// Returns true if all items in the enumerable match a predicate (All in LINQ)
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to test</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the enumerable match the predicate</returns>
        [Pure]
        public static bool forall<T>(IEnumerable<T> list, Func<T, bool> pred) =>
            list.All(pred);

        /// <summary>
        /// Return a new enumerable with all duplicate values removed
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <returns>A new enumerable with all duplicate values removed</returns>
        [Pure]
        public static IEnumerable<T> distinct<T>(IEnumerable<T> list) =>
            list.Distinct();

        /// <summary>
        /// Return a new enumerable with all duplicate values removed
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <returns>A new enumerable with all duplicate values removed</returns>
        [Pure]
        public static IEnumerable<T> distinct<T>(IEnumerable<T> list, Func<T, T, bool> compare) =>
            list.Distinct(new EqCompare<T>(compare));

        /// <summary>
        /// Returns a new enumerable with the first 'count' items from the enumerable provided
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <param name="count">Number of items to take</param>
        /// <returns>A new enumerable with the first 'count' items from the enumerable provided</returns>
        [Pure]
        public static IEnumerable<T> take<T>(IEnumerable<T> list, int count) =>
            list.Take(count);

        /// <summary>
        /// Iterate the list, yielding items if they match the predicate provided, and stopping 
        /// as soon as one doesn't
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <param name="count">Number of items to take</param>
        /// <returns>A new enumerable with the first items that match the predicate</returns>
        [Pure]
        public static IEnumerable<T> takeWhile<T>(IEnumerable<T> list, Func<T, bool> pred) =>
            list.TakeWhile(pred);

        /// <summary>
        /// Iterate the list, yielding items if they match the predicate provided, and stopping 
        /// as soon as one doesn't.  An index value is also provided to the predicate function.
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <param name="count">Number of items to take</param>
        /// <returns>A new enumerable with the first items that match the predicate</returns>
        [Pure]
        public static IEnumerable<T> takeWhile<T>(IEnumerable<T> list, Func<T, int, bool> pred) =>
            list.TakeWhile(pred);

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<S> unfold<S>(S state, Func<S, Option<S>> unfolder)
        {
            while (true)
            {
                yield return state;
                var res = unfolder(state);
                if (res.IsNone)
                {
                    yield break;
                }
                else
                {
                    state = res.Value;
                    yield return res.Value;
                }
            }
        }

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.  An aggregate
        /// state value is threaded through separately to the yielded value.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<T> unfold<S, T>(S state, Func<S, Option<Tuple<T, S>>> unfolder)
        {
            while (true)
            {
                var res = unfolder(state);
                if (res.IsNone)
                {
                    yield break;
                }
                else
                {
                    state = res.Value.Item2;
                    yield return res.Value.Item1;
                }
            }
        }

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.  An aggregate
        /// state value is threaded through separately to the yielded value.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<T> unfold<S1, S2, T>(Tuple<S1, S2> state, Func<S1, S2, Option<Tuple<T, S1, S2>>> unfolder)
        {
            while (true)
            {
                var res = unfolder(state.Item1, state.Item2);
                if (res.IsNone)
                {
                    yield break;
                }
                else
                {
                    state = System.Tuple.Create(res.Value.Item2, res.Value.Item3);
                    yield return res.Value.Item1;
                }
            }
        }

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.  An aggregate
        /// state value is threaded through separately to the yielded value.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<T> unfold<S1, S2, S3, T>(Tuple<S1, S2, S3> state, Func<S1, S2, S3, Option<Tuple<T, S1, S2, S3>>> unfolder)
        {
            while (true)
            {
                var res = unfolder(state.Item1, state.Item2, state.Item3);
                if (res.IsNone)
                {
                    yield break;
                }
                else
                {
                    state = System.Tuple.Create(res.Value.Item2, res.Value.Item3, res.Value.Item4);
                    yield return res.Value.Item1;
                }
            }
        }

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.  An aggregate
        /// state value is threaded through separately to the yielded value.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<T> unfold<S1, S2, S3, S4, T>(Tuple<S1, S2, S3, S4> state, Func<S1, S2, S3, S4, Option<Tuple<T, S1, S2, S3, S4>>> unfolder)
        {
            while (true)
            {
                var res = unfolder(state.Item1, state.Item2, state.Item3, state.Item4);
                if (res.IsNone)
                {
                    yield break;
                }
                else
                {
                    state = System.Tuple.Create(res.Value.Item2, res.Value.Item3, res.Value.Item4, res.Value.Item5);
                    yield return res.Value.Item1;
                }
            }
        }

        /// <summary>
        /// Returns true if any item in the enumerable matches the predicate provided
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to test</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if any item in the enumerable matches the predicate provided</returns>
        [Pure]
        public static bool exists<T>(IEnumerable<T> list, Func<T, bool> pred)
        {
            foreach (var item in list)
            {
                if (pred(item))
                    return true;
            }
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("headSafe has been deprecated, please use headOrNone")]
        public static Option<T> headSafe<T>(IEnumerable<T> list) =>
            (from x in list
             select Some(x))
            .DefaultIfEmpty(None)
            .FirstOrDefault();

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions
        /// </summary>
        /// <param name="opt">IEnumerable of functions</param>
        /// <param name="arg">IEnumerable of argument values</param>
        /// <returns>Returns the result of applying the IEnumerable argument values to the IEnumerable functions</returns>
        [Pure]
        public static IEnumerable<R> apply<T, R>(IEnumerable<Func<T, R>> self, IEnumerable<T> arg) =>
            from f in self
            from x in arg
            select f(x);

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="opt">IEnumerable of functions</param>
        /// <param name="arg">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerable of argument values to the 
        /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
        [Pure]
        public static IEnumerable<Func<T2, R>> apply<T1, T2, R>(IEnumerable<Func<T1, T2, R>> self, IEnumerable<T1> arg) =>
            from f in self
            let c = curry(f)
            from x in arg
            select c(x);

        /// <summary>
        /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="opt">IEnumerable of functions</param>
        /// <param name="arg1">IEnumerable of arguments</param>
        /// <param name="arg2">IEnumerable of arguments</param>
        /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
        [Pure]
        public static IEnumerable<R> apply<T1, T2, R>(IEnumerable<Func<T1, T2, R>> self, IEnumerable<T1> arg1, IEnumerable<T2> arg2) =>
            self.Apply(arg1).Apply(arg2);

        /// <summary>
        /// The tails function returns all final segments of the argument, longest first. For example,
        ///  i.e. tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
        /// </summary>
        /// <typeparam name="T">List item type</typeparam>
        /// <param name="self">List</param>
        /// <returns>Enumerable of Enumerables of T</returns>
        [Pure]
        public static IEnumerable<IEnumerable<T>> tails<T>(IEnumerable<T> self)
        {
            var lst = new List<T>(self);
            for (var skip = 0; skip < lst.Count; skip++)
            {
                yield return lst.Skip(skip);
            }
            yield return new T[0];
        }

        /// <summary>
        /// Span, applied to a predicate 'pred' and a list, returns a tuple where first element is 
        /// longest prefix (possibly empty) of elements that satisfy 'pred' and second element is the 
        /// remainder of the list:
        /// </summary>
        /// <example>
        /// List.span(List(1,2,3,4,1,2,3,4), x => x &lt; 3) == Tuple(List(1,2),List(3,4,1,2,3,4))
        /// </example>
        /// <example>
        /// List.span(List(1,2,3), x => x &lt; 9) == Tuple(List(1,2,3),List())
        /// </example>
        /// <example>
        /// List.span(List(1,2,3), x => x &lt; 0) == Tuple(List(),List(1,2,3))
        /// </example>
        /// <typeparam name="T">List element type</typeparam>
        /// <param name="self">List</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Split list</returns>
        [Pure]
        public static Tuple<IEnumerable<T>, IEnumerable<T>> span<T>(IEnumerable<T> self, Func<T, bool> pred)
        {
            int index = 0;
            foreach (var item in self)
            {
                if (!pred(item))
                {
                    break;
                }
                index++;
            }
            return Tuple(self.Take(index), self.Skip(index));
        }
    }

    class EqCompare<T> : IEqualityComparer<T>
    {
        readonly Func<T, T, bool> compare;

        public EqCompare(Func<T, T, bool> compare)
        {
            this.compare = compare;
        }

        [Pure]
        public bool Equals(T x, T y) =>
            isnull(x) && isnull(y)
                ? true
                : isnull(x) || isnull(y)
                    ? false
                    : compare(x, y);

        [Pure]
        public int GetHashCode(T obj) =>
            isnull(obj)
                ? 0
                : obj.GetHashCode();
    }

    public static class EnumerableExtensions
    {
        /// <summary>
        /// List pattern matching
        /// </summary>
        [Pure]
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, IEnumerable<T>, R> More
            )
        {
            if (list == null)
            {
                return Empty();
            }
            else
            {
                list = list.Memo();
                var head = list.HeadOrNone();
                var tail = list.Skip(1);

                return head.IsNone
                    ? Empty()
                    : More(head.Value, tail);
            }
        }


        /// <summary>
        /// List pattern matching
        /// </summary>
        [Pure]
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, IEnumerable<T>, R> More
            )
        {
            if (list == null)
            {
                return Empty();
            }
            else
            {
                list = list.Memo();
                var head = list.HeadOrNone();
                var tail = list.Skip(1);
                if (head.IsNone)
                {
                    return Empty();
                }
                else
                {
                    var second = tail.HeadOrNone();
                    return second.IsNone
                        ? One(head.Value)
                        : More(head.Value, tail);
                }
            }
        }

        /// <summary>
        /// List pattern matching
        /// </summary>
        [Pure]
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, IEnumerable<T>, R> More
            )
        {
            if (list == null) return Empty();
            list = list.Memo();
            var items = new Lst<T>(list.Take(3));
            switch (items.Count)
            {
                case 0: return Empty();
                case 1: return One(items[0]);
                case 2: return Two(items[0], items[1]);
                default: return More(items[0], items[1], list.Skip(2));
            }
        }

        /// <summary>
        /// List pattern matching
        /// </summary>
        [Pure]
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, IEnumerable<T>, R> More
            )
        {
            if (list == null) return Empty();
            list = list.Memo();
            var items = new Lst<T>(list.Take(4));
            switch (items.Count)
            {
                case 0: return Empty();
                case 1: return One(items[0]);
                case 2: return Two(items[0], items[1]);
                case 3: return Three(items[0], items[1], items[2]);
                default: return More(items[0], items[1], items[2], list.Skip(3));
            }
        }

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, IEnumerable<T>, R> More
            )
        {
            if (list == null) return Empty();
            list = list.Memo();
            var items = new Lst<T>(list.Take(5));
            switch (items.Count)
            {
                case 0: return Empty();
                case 1: return One(items[0]);
                case 2: return Two(items[0], items[1]);
                case 3: return Three(items[0], items[1], items[2]);
                case 4: return Four(items[0], items[1], items[2], items[3]);
                default: return More(items[0], items[1], items[2], items[3], list.Skip(4));
            }
        }

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, T, R> Five,
            Func<T, T, T, T, T, IEnumerable<T>, R> More
            )
        {
            if (list == null) return Empty();
            list = list.Memo();
            var items = new Lst<T>(list.Take(6));
            switch (items.Count)
            {
                case 0: return Empty();
                case 1: return One(items[0]);
                case 2: return Two(items[0], items[1]);
                case 3: return Three(items[0], items[1], items[2]);
                case 4: return Four(items[0], items[1], items[2], items[3]);
                case 5: return Five(items[0], items[1], items[2], items[3], items[4]);
                default: return More(items[0], items[1], items[2], items[3], items[4], list.Skip(5));
            }
        }

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, T, R> Five,
            Func<T, T, T, T, T, T, R> Six,
            Func<T, T, T, T, T, T, IEnumerable<T>, R> More
            )
        {
            if (list == null) return Empty();
            list = list.Memo();
            var items = new Lst<T>(list.Take(7));
            switch (items.Count)
            {
                case 0: return Empty();
                case 1: return One(items[0]);
                case 2: return Two(items[0], items[1]);
                case 3: return Three(items[0], items[1], items[2]);
                case 4: return Four(items[0], items[1], items[2], items[3]);
                case 5: return Five(items[0], items[1], items[2], items[3], items[4]);
                case 6: return Six(items[0], items[1], items[2], items[3], items[4], items[5]);
                default: return More(items[0], items[1], items[2], items[3], items[4], items[5], list.Skip(6));
            }
        }

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
        /// <param name="opt">IEnumerable of functions</param>
        /// <param name="arg">IEnumerable of argument values</param>
        /// <returns>Returns the result of applying the IEnumerable argument values to the IEnumerable functions</returns>
        [Pure]
        public static IEnumerable<R> Apply<T, R>(this IEnumerable<Func<T, R>> self, IEnumerable<T> arg) =>
            from f in self
            from x in arg
            select f(x);

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="opt">IEnumerable of functions</param>
        /// <param name="arg">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerable of argument values to the 
        /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
        [Pure]
        public static IEnumerable<Func<T2, R>> Apply<T1, T2, R>(this IEnumerable<Func<T1, T2, R>> self, IEnumerable<T1> arg) =>
            from f in self
            let c = curry(f)
            from x in arg
            select c(x);

        /// <summary>
        /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="opt">IEnumerable of functions</param>
        /// <param name="arg1">IEnumerable of arguments</param>
        /// <param name="arg2">IEnumerable of arguments</param>
        /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
        [Pure]
        public static IEnumerable<R> Apply<T1, T2, R>(this IEnumerable<Func<T1, T2, R>> self, IEnumerable<T1> arg1, IEnumerable<T2> arg2) =>
            self.Apply(arg1).Apply(arg2);

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
        public static IEnumerable<T> Choose<T>(this IEnumerable<T> list, Func<T, Option<T>> selector) =>
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
        public static IEnumerable<T> Choose<T>(this IEnumerable<T> list, Func<int, T, Option<T>> selector) =>
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
        /// <typeparam name="T">List item type</typeparam>
        /// <param name="list">Listto reverse</param>
        /// <returns>Reversed list</returns>
        [Pure]
        public static Lst<T> Rev<T>(this Lst<T> list) =>
            LanguageExt.List.rev(list);

        /// <summary>
        /// Concatenate two enumerables (Concat in LINQ)
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="lhs">First enumerable</param>
        /// <param name="rhs">Second enumerable</param>
        /// <returns>Concatenated enumerable</returns>
        [Pure]
        public static IEnumerable<T> Append<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            LanguageExt.List.append(lhs, rhs);

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
        /// Returns true if all items in the enumerable match a predicate (All in LINQ)
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
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> list, Func<T, T, bool> compare) =>
            LanguageExt.List.distinct(list, compare);

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
        /// List.span(List(1,2,3,4,1,2,3,4), x => x &lt; 3) == Tuple(List(1,2),List(3,4,1,2,3,4))
        /// </example>
        /// <example>
        /// List.span(List(1,2,3), x => x &lt; 9) == Tuple(List(1,2,3),List())
        /// </example>
        /// <example>
        /// List.span(List(1,2,3), x => x &lt; 0) == Tuple(List(),List(1,2,3))
        /// </example>
        /// <typeparam name="T">List element type</typeparam>
        /// <param name="self">List</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Split list</returns>
        [Pure]
        public static Tuple<IEnumerable<T>, IEnumerable<T>> Span<T>(this IEnumerable<T> self, Func<T, bool> pred) =>
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
        public static Lst<U> Select<T, U>(this Lst<T> self, Func<T, U> map) =>
            new Lst<U>(self.AsEnumerable().Select(map));

        /// <summary>
        /// Monadic bind function for Lst that returns an IEnumerable
        /// </summary>
        [Pure]
        public static IEnumerable<R> BindEnumerable<T, R>(this Lst<T> self, Func<T, Lst<R>> binder)
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
        public static Lst<R> Bind<T, R>(this Lst<T> self, Func<T, Lst<R>> binder) =>
            new Lst<R>(self.BindEnumerable(binder));

        /// <summary>
        /// Returns the number of items in the Lst T
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="list">List to count</param>
        /// <returns>The number of items in the list</returns>
        [Pure]
        public static int Count<T>(this Lst<T> self) =>
            self.Count;

        /// <summary>
        /// LINQ bind implementation for Lst
        /// </summary>
        [Pure]
        public static Lst<V> SelectMany<T, U, V>(this Lst<T> self, Func<T, Lst<U>> bind, Func<T, U, V> project) =>
            self.Bind(t => bind(t).Map(u => project(t, u)));

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<V> SelectMany<T, U, V>(this Lst<T> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            )
        {
            if (self.Count == 0) return Lst<V>.Empty;
            return self.Bind(t => bind(t).Map(u => project(t, u))).Freeze();
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<V> SelectMany<T, U, V>(this IEnumerable<T> self,
            Func<T, Lst<U>> bind,
            Func<T, U, V> project
            )
        {
            var ta = self.Take(1).ToArray();
            if (ta.Length == 0) return Lst<V>.Empty;
            return self.Bind(t => bind(t).Map(u => project(t, u))).Freeze();
        }
    }
}
