using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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
        new IterableEnumerable<A>([value]);

    /// <summary>
    /// Create a new empty sequence
    /// </summary>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> create<A>() =>
        Iterable<A>.Empty;

    /// <summary>
    /// Create a sequence from a initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
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
    public static Iterable<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.Length == 0 ? Iterable<A>.Empty : Iterable<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> createRange<A>(IEnumerable<A> items) =>
        new IterableEnumerable<A>(items);

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
        list.Head();

    /// <summary>
    /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
    /// comprised of the results for each element where the function returns Some(f(x)).
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
    /// Applies a function to each element of the sequence, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the sequence. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static Iterable<S> scan<S, A>(Iterable<A> list, S state, Func<S, A, S> folder) =>
        list.Scan(state, folder);

    /// <summary>
    /// Applies a function to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the sequence. Then, it 
    /// passes this result into the function along with the second element, and so on. Finally, 
    /// it returns the list of intermediate results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<S> scanBack<S, A>(Iterable<A> list, S state, Func<S, A, S> folder) =>
        scan(rev(list), state, folder);

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
    /// Joins two sequences together either into an sequence of tuples
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
}
