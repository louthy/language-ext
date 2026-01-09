using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;

namespace LanguageExt;

/// <summary>
/// Cons sequence module
/// Represents a sequence of values in a similar way to IEnumerable, but without the
/// issues of multiple evaluation for key LINQ operators like Skip, Count, etc.
/// </summary>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
public partial class Iterable
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> flatten<A>(Iterable<Iterable<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> empty<A>() =>
        Iterable<A>.Empty;

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> singleton<A>(A value) =>
        new IterableSingleton<A>(value);

    /// <summary>
    /// Create a new empty sequence
    /// </summary>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> create<A>() =>
        Iterable<A>.Empty;

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> create<A>(params A[] items)
    {
        if (items.Length == 0) return Iterable<A>.Empty;
        var nitems = new A[items.Length];
        System.Array.Copy(items, nitems, items.Length);
        return Iterable<A>.FromSpan(items);
    }

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> create<A>(ReadOnlySpan<A> items) =>
        items.Length == 0 ? Iterable<A>.Empty : Iterable<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> createRange<A>(IEnumerable<A> items) =>
        new IterableEnumerable<A>(IO.pure(items));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> createRange<A>(IAsyncEnumerable<A> items) =>
        new IterableAsyncEnumerable<A>(IO.pure(items));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> createRange<A>(IO<IAsyncEnumerable<A>> items) =>
        new IterableAsyncEnumerable<A>(items);

    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> generate<A>(int count, Func<int, A> generator) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(generator);

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> repeat<A>(A item, int count) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(_ => item);

    /// <summary>
    /// Get the item at the head (first) of the sequence or None if the sequence is empty
    /// </summary>
    /// <param name="list">sequence</param>
    /// <returns>Optional head item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> head<A>(Iterable<A> list) =>
        list.Head;

    /// <summary>
    /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
    /// of results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<B> choose<A, B>(Iterable<A> list, Func<A, Option<B>> selector) =>
        list.Map(selector).Filter(t => t.IsSome).Map(t => t.Value!);

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> rev<A>(Iterable<A> list) =>
        list.Reverse();

    /// <summary>
    /// Joins two sequences together either into a single sequence using the join 
    /// function provided
    /// </summary>
    /// <param name="list">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<C> zip<A, B, C>(Iterable<A> list, Iterable<B> other, Func<A, B, C> zipper) =>
        list.Zip(other, zipper);

    /// <summary>
    /// Joins two sequences together either into a sequence of tuples
    /// </summary>
    /// <param name="list">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<(A First, B Second)> zip<A, B>(Iterable<A> list, Iterable<B> other) =>
        list.Zip(other, (t, u) => (t, u));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> distinct<A>(Iterable<A> list) =>
        list.Distinct();

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> distinct<EqA, A>(Iterable<A> list) where EqA : Eq<A> =>
        list.Distinct<EqA>();

    /// <summary>
    /// Returns a new sequence with the first 'count' items from the sequence provided
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first 'count' items from the sequence provided</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> take<A>(Iterable<A> list, int count) =>
        list.Take(count);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first items that match the predicate</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> takeWhile<A>(Iterable<A> list, Func<A, bool> pred) =>
        list.TakeWhile(pred);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't.  An index value is also provided to the predicate function.
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first items that match the predicate</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> takeWhile<A>(Iterable<A> list, Func<A, int, bool> pred) =>
        list.TakeWhile(pred);

    /// <summary>
    /// Generates an int.MaxValue sequence of T using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static Iterable<T> generate<T>(Func<int, T> generator) =>
        IterableExtensions.AsIterable(Range(0, int.MaxValue)).Map(generator);

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
        list.Select(Option.Some)
            .DefaultIfEmpty(Option<A>.None)
            .FirstOrDefault();

    /// <summary>
    /// Get the item at the head (first) of the list or Left if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or left</returns>
    [Pure]
    public static Either<L, R> headOrLeft<L, R>(IEnumerable<R> list, L left) =>
        list.Select(Either.Right<L, R>)
            .DefaultIfEmpty(Either.Left<L, R>(left))
            .FirstOrDefault() ?? left;

    /// <summary>
    /// Get the item at the head (first) of the list or fail if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or fail</returns>
    [Pure]
    public static Validation<F, S> headOrInvalid<F, S>(IEnumerable<S> list, F fail) 
        where F : Monoid<F> =>
        list.Select(Validation.Success<F, S>)
            .DefaultIfEmpty(Validation.Fail<F, S>(fail))
            .FirstOrDefault() ?? F.Empty;

    /// <summary>
    /// Get the item at the head (first) of the list or fail if the list is empty
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Either head item or fail</returns>
    [Pure]
    public static Validation<F, S> headOrInvalid<F, S>(IEnumerable<S> list)
        where F : Monoid<F> =>
        list.Select(Validation.Success<F, S>)
            .DefaultIfEmpty(Validation.Fail<F, S>(F.Empty))
            .FirstOrDefault() ?? F.Empty;

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
        list.Select(Option.Some)
            .DefaultIfEmpty(Option<A>.None)
            .LastOrDefault();

    /// <summary>
    /// Get the last item of the list
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Last item</returns>
    [Pure]
    public static Either<L, R> lastOrLeft<L, R>(IEnumerable<R> list, L left) =>
        list.Select(Either.Right<L, R>)
            .DefaultIfEmpty(Either.Left<L, R>(left))
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
    /// <typeparam name="A">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to search</param>
    /// <param name="pred">Predicate</param>
    /// <returns>[x] for the first item in the list that matches the predicate 
    /// provided, [] otherwise.</returns>
    [Pure]
    public static Iterable<A> findSeq<A>(IEnumerable<A> list, Func<A, bool> pred)
    {
        return createRange(go());
        IEnumerable<A> go()
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
    }

    /// <summary>
    /// Joins two enumerables together either into a single enumerable
    /// using the join function provided
    /// </summary>
    /// <param name="list">First list to join</param>
    /// <param name="other">Second list to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined enumerable</returns>
    [Pure]
    public static Iterable<V> zip<T, U, V>(IEnumerable<T> list, IEnumerable<U> other, Func<T, U, V> zipper) =>
        list.Zip(other, zipper).AsIterable();

    /// <summary>
    /// Joins two enumerables together either into an enumerables of tuples
    /// </summary>
    /// <param name="list">First list to join</param>
    /// <param name="other">Second list to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined enumerable of tuples</returns>
    [Pure]
    public static Iterable<(T First, U Second)> zip<T, U>(IEnumerable<T> list, IEnumerable<U> other) =>
        list.Zip(other, (t, u) => (t, u)).AsIterable();

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
    public static Iterable<T> distinct<T>(IEnumerable<T> list) =>
        list.Distinct().AsIterable();

    /// <summary>
    /// Return a new enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <returns>A new enumerable with all duplicate values removed</returns>
    [Pure]
    public static Iterable<T> distinct<EQ, T>(IEnumerable<T> list) where EQ : Eq<T> =>
        list.Distinct(new EqCompare<T>(static (x, y) => EQ.Equals(x, y), static x => EQ.GetHashCode(x))).AsIterable();

    /// <summary>
    /// Return a new enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <returns>A new enumerable with all duplicate values removed</returns>
    [Pure]
    public static Iterable<T> distinct<T, K>(IEnumerable<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default) =>
        list.Distinct(new EqCompare<T>(
                          (a, b) => compare.IfNone(EqDefault<K>.Equals)(keySelector(a), keySelector(b)), 
                          a => keySelector(a)?.GetHashCode() ?? 0))
            .AsIterable();

    /// <summary>
    /// Returns a new enumerable with the first 'count' items from the enumerable provided
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first 'count' items from the enumerable provided</returns>
    [Pure]
    public static Iterable<T> take<T>(IEnumerable<T> list, int count) =>
        list.Take(count).AsIterable();

    /// <summary>
    /// Iterate the list, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first items that match the predicate</returns>
    [Pure]
    public static Iterable<T> takeWhile<T>(IEnumerable<T> list, Func<T, bool> pred) =>
        list.TakeWhile(pred).AsIterable();

    /// <summary>
    /// Iterate the list, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't.  An index value is also provided to the predicate function.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first items that match the predicate</returns>
    [Pure]
    public static Iterable<T> takeWhile<T>(IEnumerable<T> list, Func<T, int, bool> pred) =>
        list.TakeWhile(pred).AsIterable();

    /// <summary>
    /// Generate a new list from an intial state value and an 'unfolding' function.
    /// The unfold function generates the items in the resulting list until None is returned.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="unfolder">Unfold function</param>
    /// <returns>Unfolded enumerable</returns>
    [Pure]
    public static Iterable<S> unfold<S>(S state, Func<S, Option<S>> unfolder)
    {
        return go().AsIterable();
        IEnumerable<S> go()
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
    public static Iterable<A> unfold<S, A>(S state, Func<S, Option<(A, S)>> unfolder)
    {
        return go().AsIterable();
        IEnumerable<A> go()
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
    public static Iterable<A> unfold<S1, S2, A>((S1, S2) state, Func<S1, S2, Option<(A, S1, S2)>> unfolder)
    {
        return go().AsIterable();

        IEnumerable<A> go()
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
    public static Iterable<A> unfold<S1, S2, S3, A>((S1, S2, S3) state, Func<S1, S2, S3, Option<(A, S1, S2, S3)>> unfolder)
    {
        return go().AsIterable();
        IEnumerable<A> go()
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
    public static Iterable<A> unfold<S1, S2, S3, S4, A>((S1, S2, S3, S4) state, Func<S1, S2, S3, S4, Option<(A, S1, S2, S3, S4)>> unfolder)
    {
        return go().AsIterable();
        IEnumerable<A> go()
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
            if (pred(item)) return true;
        }
        return false;
    }

    /// <summary>
    /// The tails function returns all final segments of the argument, longest first. For example,
    ///  i.e. tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="self">List</param>
    /// <returns>Enumerable of Enumerables of T</returns>
    [Pure]
    public static Iterable<Iterable<T>> tails<T>(IEnumerable<T> self)
    {
        return go().AsIterable();

        IEnumerable<Iterable<T>> go()
        {
            var lst = new List<T>(self);
            for (var skip = 0; skip < lst.Count; skip++)
            {
                yield return lst.Skip(skip).AsIterable();
            }
            yield return Iterable<T>.Empty;
        }
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
    public static (Iterable<T> Initial, Iterable<T> Remainder) span<T>(IEnumerable<T> self, Func<T, bool> pred)
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

        return (first(iter).AsIterable(), second(iter).AsIterable());
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
