using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public static partial class List
    {
        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static Lst<A> flatten<A>(Lst<Lst<A>> ma) =>
            ma.Bind(identity);

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static IEnumerable<A> flatten<A>(IEnumerable<IEnumerable<A>> ma) =>
            ma.Bind(identity);

        /// <summary>
        /// Create an empty IEnumerable T
        /// </summary>
        [Pure]
        public static Lst<T> empty<T>() =>
            Lst<T>.Empty;

        /// <summary>
        /// Create a new empty list
        /// </summary>
        /// <returns>Lst T</returns>
        [Pure]
        public static Lst<T> create<T>() =>
            Lst<T>.Empty;

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
        public static IEnumerable<T> generate<T>(int count, Func<int, T> generator) =>
            from i in Range(0, count)
            select generator(i);

        /// <summary>
        /// Generates an int.MaxValue sequence of T using the provided delegate to initialise
        /// each item.
        /// </summary>
        [Pure]
        public static IEnumerable<T> generate<T>(Func<int, T> generator) =>
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
        public static T head<T>(IEnumerable<T> list) => 
            list.First();
        /// <summary>
        /// Get the item at the head (first) of the list or None if the list is empty
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Optional head item</returns>
        [Pure]
        public static Option<A> headOrNone<A>(IEnumerable<A> list) =>
            list.Select(Option<A>.Some)
                .DefaultIfEmpty(Option<A>.None)
                .FirstOrDefault();

        /// <summary>
        /// Get the item at the head (first) of the list or Left if the list is empty
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Either head item or left</returns>
        [Pure]
        public static Either<L, R> headOrLeft<L, R>(IEnumerable<R> list, L left) =>
            list.Select(Either<L, R>.Right)
                .DefaultIfEmpty(Either<L, R>.Left(left))
                .FirstOrDefault();

        /// <summary>
        /// Get the item at the head (first) of the list or fail if the list is empty
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Either head item or fail</returns>
        [Pure]
        public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IEnumerable<Success> list, Fail fail) =>
            list.Select(Validation<Fail, Success>.Success)
                .DefaultIfEmpty(Validation<Fail, Success>.Fail(Seq1(fail)))
                .FirstOrDefault();

        /// <summary>
        /// Get the item at the head (first) of the list or fail if the list is empty
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Either head item or fail</returns>
        [Pure]
        public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IEnumerable<Success> list, Seq<Fail> fail) =>
            list.Select(Validation<Fail, Success>.Success)
                .DefaultIfEmpty(Validation<Fail, Success>.Fail(fail))
                .FirstOrDefault();

        /// <summary>
        /// Get the item at the head (first) of the list or fail if the list is empty
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Either head item or fail</returns>
        [Pure]
        public static Validation<MonoidFail, Fail, Success> headOrInvalid<MonoidFail, Fail, Success>(IEnumerable<Success> list, Fail fail)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            list.Select(Validation<MonoidFail, Fail, Success>.Success)
                .DefaultIfEmpty(Validation<MonoidFail, Fail, Success>.Fail(fail))
                .FirstOrDefault();

        /// <summary>
        /// Get the last item of the list
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Last item</returns>
        [Pure]
        public static A last<A>(IEnumerable<A> list) =>
            list.Last();

        /// <summary>
        /// Get the last item of the list
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Last item</returns>
        [Pure]
        public static Option<A> lastOrNone<A>(IEnumerable<A> list) =>
            list.Select(Option<A>.Some)
                .DefaultIfEmpty(Option<A>.None)
                .LastOrDefault();

        /// <summary>
        /// Get the last item of the list
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Last item</returns>
        [Pure]
        public static Either<L, R> lastOrLeft<L, R>(IEnumerable<R> list, L left) =>
            list.Select(Either<L, R>.Right)
                .DefaultIfEmpty(Either<L, R>.Left(left))
                .LastOrDefault();

        /// <summary>
        /// Get the last item of the list
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Last item</returns>
        [Pure]
        public static Validation<Fail, Success> lastOrInvalid<Fail, Success>(IEnumerable<Success> list, Fail fail) =>
            list.Select(Validation<Fail, Success>.Success)
                .DefaultIfEmpty(Validation<Fail, Success>.Fail(Seq1(fail)))
                .LastOrDefault();

        /// <summary>
        /// Get the last item of the list
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Last item</returns>
        [Pure]
        public static Validation<Fail, Success> lastOrInvalid<Fail, Success>(IEnumerable<Success> list, Seq<Fail> fail) =>
            list.Select(Validation<Fail, Success>.Success)
                .DefaultIfEmpty(Validation<Fail, Success>.Fail(fail))
                .LastOrDefault();

        /// <summary>
        /// Get the last item of the list
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>Last item</returns>
        [Pure]
        public static Validation<MonoidFail, Fail, Success> lastOrInvalid<MonoidFail, Fail, Success>(IEnumerable<Success> list, Fail fail)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            list.Select(Validation<MonoidFail, Fail, Success>.Success)
                .DefaultIfEmpty(Validation<MonoidFail, Fail, Success>.Fail(fail))
                .LastOrDefault();

        /// <summary>
        /// Get all items in the list except the last one
        /// </summary>
        /// <remarks>
        /// Must evaluate the last item to know it's the last, but won't return it
        /// </remarks>
        /// <param name="list">List</param>
        /// <returns>The initial items (all but the last)</returns>
        [Pure]
        public static Seq<A> init<A>(IEnumerable<A> list)
        {
            var items = list.ToArray();
            return new Seq<A>(new SeqStrict<A>(items, 0, Math.Max(0, items.Length - 1), 0, 0));
        }


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
        [Pure]
        public static IEnumerable<Func<T2, R>> parmap<T1, T2, R>(IEnumerable<T1> list, Func<T1, T2, R> func) =>
            list.Map(curry(func));

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public static IEnumerable<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(IEnumerable<T1> list, Func<T1, T2, T3, R> func) =>
            list.Map(curry(func));

        /// <summary>
        /// Projects the values in the enumerable into a new enumerable using a map function, which is also given an index value
        /// (Select in LINQ - note that the order of the arguments of the map function are the other way around, here the index
        /// is the first argument).
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
        public static IEnumerable<R> choose<T, R>(IEnumerable<T> list, Func<T, Option<R>> selector) =>
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
        public static IEnumerable<R> choose<T, R>(IEnumerable<T> list, Func<int, T, Option<R>> selector) =>
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
        /// Reverses the list (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">List item type</typeparam>
        /// <param name="list">List to reverse</param>
        /// <returns>Reversed list</returns>
        [Pure]
        public static Lst<PredList, T> rev<PredList, T>(Lst<PredList, T> list) 
            where PredList : struct, Pred<ListInfo> =>
            list.Reverse();

        /// <summary>
        /// Reverses the list (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">List item type</typeparam>
        /// <param name="list">List to reverse</param>
        /// <returns>Reversed list</returns>
        [Pure]
        public static Lst<PredList, PredItem, T> rev<PredList, PredItem, T>(Lst<PredList, PredItem, T> list) 
            where PredList : struct, Pred<ListInfo>
            where PredItem : struct, Pred<T> =>
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
            lhs.ConcatFast(rhs);

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
                ? Enumerable.Empty<T>()
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
        /// <param name="preditem">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
        {
            foreach (var item in list)
            {
                if (!preditem(item))
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
        /// <param name="predstate">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
        {
            foreach (var item in list)
            {
                if (!predstate(state))
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
        /// <param name="preditem">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldBackWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
            foldWhile(rev(list), state, folder, preditem: preditem);

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
        public static S foldBackWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
            foldWhile(rev(list), state, folder, predstate: predstate);

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
        public static S foldUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
        {
            foreach (var item in list)
            {
                if (preditem(item))
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
        /// <param name="predstate">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
        {
            foreach (var item in list)
            {
                if (predstate(state))
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
        /// <param name="preditem">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldBackUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
            foldUntil(rev(list), state, folder, preditem: preditem);

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
        public static S foldBackUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
            foldUntil(rev(list), state, folder, predstate: predstate);

        /// <summary>
        /// Applies a function to each element of the collection (from first element to last), threading 
        /// an accumulator argument through the computation. This function first applies the function 
        /// to the first two elements of the list. Then, it passes this result into the function along 
        /// with the third element and so on. Finally, it returns the final result.
        /// </summary>
        /// <remarks>The enumerable must contain at least one item or an excpetion will be thrown</remarks>
        /// <typeparam name="A">Bound item type</typeparam>
        /// <param name="list">Enumerable to reduce</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static A reduce<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
            list.Match(
                ()      => failwith<A>("Input list was empty"),
                (x, xs) => fold(xs, x, reducer));

        /// <summary>
        /// Applies a function to each element of the collection (from first element to last), threading 
        /// an accumulator argument through the computation. This function first applies the function 
        /// to the first two elements of the list. Then, it passes this result into the function along 
        /// with the third element and so on. Finally, it returns the final result.
        /// </summary>
        /// <remarks>The enumerable must contain at least one item or None will be returned</remarks>
        /// <typeparam name="A">Bound item type</typeparam>
        /// <param name="list">Enumerable to reduce</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Optional aggregate value</returns>
        [Pure]
        public static Option<A> reduceOrNone<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
            list.Match(
                ()      => None,
                (x, xs) => Some(fold(xs, x, reducer)));

        /// <summary>
        /// Applies a function to each element of the collection, threading an accumulator argument 
        /// through the computation. This function first applies the function to the first two 
        /// elements of the list. Then, it passes this result into the function along with the third 
        /// element and so on. Finally, it returns the final result.
        /// </summary>
        /// <remarks>The enumerable must contain at least one item or an excpetion will be thrown</remarks>
        /// <typeparam name="A">Bound item type</typeparam>
        /// <param name="list">Enumerable to reduce</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static A reduceBack<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
            list.Match(
                ()      => failwith<A>("Input list was empty"),
                (x, xs) => foldBack(xs, x, reducer));

        /// <summary>
        /// Applies a function to each element of the collection, threading an accumulator argument 
        /// through the computation. This function first applies the function to the first two 
        /// elements of the list. Then, it passes this result into the function along with the third 
        /// element and so on. Finally, it returns the final result.
        /// </summary>
        /// <remarks>The enumerable must contain at least one item or None will be returned</remarks>
        /// <typeparam name="A">Bound item type</typeparam>
        /// <param name="list">Enumerable to reduce</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Optional aggregate value</returns>
        [Pure]
        public static Option<A> reduceBackOrNone<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
            list.Match(
                ()      => None,
                (x, xs) => Some(foldBack(xs, x, reducer)));

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
        /// Convert any enumerable into an immutable Lst T
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to convert</param>
        /// <returns>Lst of T</returns>
        [Pure]
        public static Lst<PredList, T> freeze<PredList, T>(IEnumerable<T> list) where PredList : struct, Pred<ListInfo> =>
            toList<PredList, T>(list);

        /// <summary>
        /// Convert any enumerable into an immutable Lst T
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to convert</param>
        /// <returns>Lst of T</returns>
        [Pure]
        public static Lst<PredList, PredItem, T> freeze<PredList, PredItem, T>(IEnumerable<T> list)
            where PredItem : struct, Pred<T>
            where PredList : struct, Pred<ListInfo> =>
                toList<PredList, PredItem, T>(list);

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
        public static IEnumerable<(T Left, U Right)> zip<T, U>(IEnumerable<T> list, IEnumerable<U> other) =>
            list.Zip(other, (t, u) => (t, u));

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
        /// Iterate each item in the enumerable in order (consume items)
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable to consume</param>
        /// <returns>Unit</returns>
        public static Unit consume<T>(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
            }
            return unit;
        }

        /// <summary>
        /// Returns true if all items in the enumerable match a predicate (Any in LINQ)
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
        public static IEnumerable<T> distinct<EQ, T>(IEnumerable<T> list) where EQ : struct, Eq<T> =>
            list.Distinct(new EqCompare<T>(static (x, y) => default(EQ).Equals(x, y)));

        /// <summary>
        /// Return a new enumerable with all duplicate values removed
        /// </summary>
        /// <typeparam name="T">Enumerable item type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <returns>A new enumerable with all duplicate values removed</returns>
        [Pure]
        public static IEnumerable<T> distinct<T, K>(IEnumerable<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default(Option<Func<K, K, bool>>)) =>
             list.Distinct(new EqCompare<T>((a, b) => compare.IfNone(default(EqDefault<K>).Equals)(keySelector(a), keySelector(b)), a => keySelector(a)?.GetHashCode() ?? 0));

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
                }
            }
        }

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.  An aggregate
        /// state value is threaded through separately to the yielded value.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="A">Bound value of resulting enumerable</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<A> unfold<S, A>(S state, Func<S, Option<(A, S)>> unfolder)
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
        /// <typeparam name="A">Bound value of resulting enumerable</typeparam>
        /// <typeparam name="S1">State type</typeparam>
        /// <typeparam name="S2">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<A> unfold<S1, S2, A>((S1, S2) state, Func<S1, S2, Option<(A, S1, S2)>> unfolder)
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
                    state = (res.Value.Item2, res.Value.Item3);
                    yield return res.Value.Item1;
                }
            }
        }

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.  An aggregate
        /// state value is threaded through separately to the yielded value.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="A">Bound value of resulting enumerable</typeparam>
        /// <typeparam name="S1">State type</typeparam>
        /// <typeparam name="S2">State type</typeparam>
        /// <typeparam name="S3">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<A> unfold<S1, S2, S3, A>((S1, S2, S3) state, Func<S1, S2, S3, Option<(A, S1, S2, S3)>> unfolder)
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
                    state = (res.Value.Item2, res.Value.Item3, res.Value.Item4);
                    yield return res.Value.Item1;
                }
            }
        }

        /// <summary>
        /// Generate a new list from an intial state value and an 'unfolding' function.  An aggregate
        /// state value is threaded through separately to the yielded value.
        /// The unfold function generates the items in the resulting list until None is returned.
        /// </summary>
        /// <typeparam name="A">Bound value of resulting enumerable</typeparam>
        /// <typeparam name="S1">State type</typeparam>
        /// <typeparam name="S2">State type</typeparam>
        /// <typeparam name="S3">State type</typeparam>
        /// <typeparam name="S4">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="unfolder">Unfold function</param>
        /// <returns>Unfolded enumerable</returns>
        [Pure]
        public static IEnumerable<A> unfold<S1, S2, S3, S4, A>((S1, S2, S3, S4) state, Func<S1, S2, S3, S4, Option<(A, S1, S2, S3, S4)>> unfolder)
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
                    state = (res.Value.Item2, res.Value.Item3, res.Value.Item4, res.Value.Item5);
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
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable of argument values</param>
        /// <returns>Returns the result of applying the IEnumerable argument values to the IEnumerable functions</returns>
        [Pure]
        public static IEnumerable<B> apply<A, B>(IEnumerable<Func<A, B>> fabc, IEnumerable<A> fa) =>
            ApplEnumerable<A, B>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable of argument values</param>
        /// <returns>Returns the result of applying the IEnumerable argument values to the IEnumerable functions</returns>
        [Pure]
        public static IEnumerable<B> apply<A, B>(Func<A, B> fabc, IEnumerable<A> fa) =>
            ApplEnumerable<A, B>.Inst.Apply(new[] { fabc }, fa);

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerable of argument values to the 
        /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
        [Pure]
        public static IEnumerable<Func<B, C>> apply<A, B, C>(IEnumerable<Func<A, B, C>> fabc, IEnumerable<A> fa) =>
            ApplEnumerable<A, B, C>.Inst.Apply(fabc.Map(curry), fa);

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerable of argument values to the 
        /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
        [Pure]
        public static IEnumerable<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, IEnumerable<A> fa) =>
            ApplEnumerable<A, B, C>.Inst.Apply(new[] { curry(fabc) }, fa);

        /// <summary>
        /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <param name="fb">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
        [Pure]
        public static IEnumerable<C> apply<A, B, C>( IEnumerable<Func<A, B, C>> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
            ApplEnumerable<A, B, C>.Inst.Apply(fabc.Map(curry), fa, fb);

        /// <summary>
        /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <param name="fb">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
        [Pure]
        public static IEnumerable<C> apply<A, B, C>(Func<A, B, C> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
            ApplEnumerable<A, B, C>.Inst.Apply(new[] { curry(fabc) }, fa, fb);

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerable of argument values to the 
        /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
        [Pure]
        public static IEnumerable<Func<B, C>> apply<A, B, C>(IEnumerable<Func<A, Func<B, C>>> fabc, IEnumerable<A> fa) =>
            ApplEnumerable<A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply an IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerable of argument values to the 
        /// IEnumerable of functions: an IEnumerable of functions of arity 1</returns>
        [Pure]
        public static IEnumerable<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, IEnumerable<A> fa) =>
            ApplEnumerable<A, B, C>.Inst.Apply(new[] { fabc }, fa);

        /// <summary>
        /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <param name="fb">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
        [Pure]
        public static IEnumerable<C> apply<A, B, C>(IEnumerable<Func<A, Func<B, C>>> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
            ApplEnumerable<A, B, C>.Inst.Apply(fabc, fa, fb);

        /// <summary>
        /// Apply IEnumerable of values to an IEnumerable of functions of arity 2
        /// </summary>
        /// <param name="fabc">IEnumerable of functions</param>
        /// <param name="fa">IEnumerable argument values</param>
        /// <param name="fb">IEnumerable argument values</param>
        /// <returns>Returns the result of applying the IEnumerables of arguments to the IEnumerable of functions</returns>
        [Pure]
        public static IEnumerable<C> apply<A, B, C>(Func<A, Func<B, C>> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
            ApplEnumerable<A, B, C>.Inst.Apply(new[] { fabc }, fa, fb);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static IEnumerable<B> action<A, B>(IEnumerable<A> fa, IEnumerable<B> fb) =>
            ApplEnumerable<A, B>.Inst.Action(fa, fb);


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
            yield return Enumerable.Empty<T>();
        }

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
        public static (IEnumerable<T>, IEnumerable<T>) span<T>(IEnumerable<T> self, Func<T, bool> pred)
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
            return (self.Take(index), self.Skip(index));
        }
    }

    class EqCompare<T> : IEqualityComparer<T>
    {
        readonly Func<T, T, bool> compare;
        readonly Option<Func<T, int>> hashCode = None;

        public EqCompare(Func<T, T, bool> compare)
        {
            this.compare = compare;
        }

        public EqCompare(Func<T, T, bool> compare, Func<T, int> hashCode)
        {
            this.compare = compare;
            this.hashCode = hashCode;
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
            hashCode.Match(
                f => isnull(obj) ? 0 : f(obj),
                () => 0);
    }
}
