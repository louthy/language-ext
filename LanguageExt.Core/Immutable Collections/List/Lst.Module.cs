﻿#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public static class List
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
    public static Iterable<A> flatten<A>(IEnumerable<IEnumerable<A>> ma) =>
        ma.Bind(identity).AsIterable();

    /// <summary>
    /// Create an empty IEnumerable T
    /// </summary>
    [Pure]
    public static Lst<T> empty<T>() =>
        Lst<T>.Empty;

    /// <summary>
    /// Create a singleton collection
    /// </summary>
    /// <param name="value">Single value</param>
    /// <returns>Collection with a single item in it</returns>
    [Pure]
    public static Lst<A> singleton<A>(A value) =>
        [value];

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
        new (items.AsSpan());

    /// <summary>
    /// Create a list from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>Lst T</returns>
    [Pure]
    public static Lst<T> createRange<T>(IEnumerable<T> items) =>
        new (items);

    /// <summary>
    /// Create a list from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>Lst T</returns>
    [Pure]
    public static Lst<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.IsEmpty
            ? Lst<A>.Empty
            : new (items);

    /// <summary>
    /// Generates a sequence of T using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static Iterable<T> generate<T>(int count, Func<int, T> generator) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(generator);

    /// <summary>
    /// Generates an int.MaxValue sequence of T using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static Iterable<T> generate<T>(Func<int, T> generator) =>
        IterableExtensions.AsIterable(Range(0, int.MaxValue)).Map(generator);

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    [Pure]
    public static Iterable<T> repeat<T>(T item, int count) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(_ => item);

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
            .FirstOrDefault() ?? left;

    /// <summary>
    /// Get the item at the head (first) of the list or fail if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or fail</returns>
    [Pure]
    public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IEnumerable<Success> list, Fail fail) 
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(fail))
            .FirstOrDefault() ?? Fail.Empty;

    /// <summary>
    /// Get the item at the head (first) of the list or fail if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or fail</returns>
    [Pure]
    public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IEnumerable<Success> list)
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(Fail.Empty))
            .FirstOrDefault() ?? Fail.Empty;

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
            .LastOrDefault() ?? left;

    /// <summary>
    /// Get the last item of the list
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Last item</returns>
    [Pure]
    public static Validation<Fail, Success> lastOrInvalid<Fail, Success>(IEnumerable<Success> list, Fail fail)
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(fail))
            .LastOrDefault() ?? fail;

    /// <summary>
    /// Get the last item of the list
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Last item</returns>
    [Pure]
    public static Validation<Fail, Success> lastOrInvalid<Fail, Success>(IEnumerable<Success> list)
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(Fail.Empty))
            .LastOrDefault() ?? Fail.Empty;

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
    public static Iterable<T> tail<T>(IEnumerable<T> list) =>
        list.Skip(1).AsIterable();

    /// <summary>
    /// Projects the values in the enumerable using a map function into a new enumerable (Select in LINQ).
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="list">Enumerable to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static Iterable<R> map<T, R>(IEnumerable<T> list, Func<T, R> map) =>
        list.Select(map).AsIterable();

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
    public static Iterable<R> map<T, R>(IEnumerable<T> list, Func<int, T, R> map) =>
        zip(list, Range(0, int.MaxValue), (t, i) => map(i, t)).AsIterable();

    /// <summary>
    /// Removes items from the list that do not match the given predicate (Where in LINQ)
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to filter</param>
    /// <param name="predicate">Predicate function</param>
    /// <returns>Filtered enumerable</returns>
    [Pure]
    public static Iterable<T> filter<T>(IEnumerable<T> list, Func<T, bool> predicate) =>
        list.Where(predicate).AsIterable();

    /// <summary>
    /// Applies the given function 'selector' to each element of the list. Returns the list comprised of 
    /// the results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered enumerable</returns>
    [Pure]
    public static Iterable<R> choose<T, R>(IEnumerable<T> list, Func<T, Option<R>> selector) =>
        map(filter(map(list, selector), t => t.IsSome), t => t.Value!);

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
    public static Iterable<R> choose<T, R>(IEnumerable<T> list, Func<int, T, Option<R>> selector) =>
        map(filter(map(list, selector), t => t.IsSome), t => t.Value!);

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
    public static Iterable<R> collect<T, R>(IEnumerable<T> list, Func<T, IEnumerable<R>> map) =>
        (from t in list
         from r in map(t)
         select r).AsIterable();

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
    public static Iterable<T> rev<T>(IEnumerable<T> list) =>
        list.Reverse().AsIterable();

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
    public static Iterable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
        lhs.ConcatFast(rhs).AsIterable();

    /// <summary>
    /// Concatenate an enumerable and an enumerable of enumerables
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="lhs">First list</param>
    /// <param name="rhs">Second list</param>
    /// <returns>Concatenated list</returns>
    [Pure]
    public static Iterable<T> append<T>(IEnumerable<T> x, IEnumerable<IEnumerable<T>> xs) =>
        xs.HeadAndTailSafe()
          .Match(
               None: x.AsIterable,
               Some: tuple => append(x, append(tuple.Head, tuple.Tail)));

    /// <summary>
    /// Concatenate N enumerables
    /// </summary>
    /// <typeparam name="T">Enumerable type</typeparam>
    /// <param name="lists">Enumerables to concatenate</param>
    /// <returns>A single enumerable with all of the items concatenated</returns>
    [Pure]
    public static Iterable<T> append<T>(params IEnumerable<T>[] lists) =>
        lists.Length == 0
            ? Iterable.empty<T>()
            : lists.Length == 1
                ? lists[0].AsIterable()
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
    public static IEnumerable<(T First, U Second)> zip<T, U>(IEnumerable<T> list, IEnumerable<U> other) =>
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
        var i = 0;
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
        foreach (var _ in list)
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
    public static IEnumerable<T> distinct<EQ, T>(IEnumerable<T> list) where EQ : Eq<T> =>
        list.Distinct(new EqCompare<T>(static (x, y) => EQ.Equals(x, y), static x => EQ.GetHashCode(x)));

    /// <summary>
    /// Return a new enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <returns>A new enumerable with all duplicate values removed</returns>
    [Pure]
    public static IEnumerable<T> distinct<T, K>(IEnumerable<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default) =>
        list.Distinct(new EqCompare<T>(
                          (a, b) => compare.IfNone(EqDefault<K>.Equals)(keySelector(a), keySelector(b)), 
                          a => keySelector(a)?.GetHashCode() ?? 0));

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
                state = res.Value!;
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
    /// List.span(List(1,2,3,4,1,2,3,4), x => x 〈 3) == (List(1,2),List(3,4,1,2,3,4))
    /// </example>
    /// <example>
    /// List.span(List(1,2,3), x => x 〈 9) == (List(1,2,3),List())
    /// </example>
    /// <example>
    /// List.span(List(1,2,3), x => x 〈 0) == (List(),List(1,2,3))
    /// </example>
    /// <typeparam name="T">List element type</typeparam>
    /// <param name="self">List</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Split list</returns>
    [Pure]
    public static (IEnumerable<T> Initial, IEnumerable<T> Remainder) span<T>(IEnumerable<T> self, Func<T, bool> pred)
    {
        var iter    = self.GetEnumerator();
        var diposed = false;

        IEnumerable<T> first(IEnumerator<T> items)
        {
            while (items.MoveNext())
            {
                if (pred(items.Current))
                {
                    yield return items.Current;
                }
                else
                {
                    yield break;
                }
            }
            items.Dispose();
            diposed = true;
        }

        IEnumerable<T> second(IEnumerator<T> items)
        {
            if (diposed) yield break;
            while (items.MoveNext())
            {
                yield return items.Current;
            }
            items.Dispose();
        }

        return (first(iter), second(iter));
    }
}

class EqCompare<T> : IEqualityComparer<T>
{
    readonly Func<T, T, bool> compare;
    readonly Option<Func<T, int>> hashCode = None;

    public EqCompare(Func<T, T, bool> compare) =>
        this.compare = compare;

    public EqCompare(Func<T, T, bool> compare, Func<T, int> hashCode)
    {
        this.compare = compare;
        this.hashCode = hashCode;
    }

    [Pure]
    public bool Equals(T? x, T? y) =>
        isnull(x) && isnull(y) || (!isnull(x) && !isnull(y) && compare(x!, y!));

    [Pure]
    public int GetHashCode(T obj) =>
        hashCode.Match(
            f => isnull(obj) ? 0 : f(obj),
            () => 0);
}
