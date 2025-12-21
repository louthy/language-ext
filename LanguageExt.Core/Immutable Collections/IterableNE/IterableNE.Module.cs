using System;
using System.Linq;
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
public partial class IterableNE
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<IterableNE<A>> flatten<A>(IterableNE<IterableNE<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IterableNE<A> singleton<A>(A value) =>
        IterableNE<A>.FromSpan([value]);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, params ReadOnlySpan<A> tail) =>
        new (head, Iterable<A>.FromSpan(tail));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<IterableNE<A>> createRange<A>(ReadOnlySpan<A> items) =>
        items.Length == 0 
            ? None
            : IterableNE<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<IterableNE<A>> createRange<A>(IEnumerable<A> items)
    {
        var iter = Iterator.from(items);
        if (iter.IsEmpty) return None;
        var head = iter.Head;
        var tail = iter.Tail.AsIterable();
        return new IterableNE<A>(head, tail);
    }

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IterableNE<A> createRangeUnsafe<A>(ReadOnlySpan<A> items) =>
        IterableNE<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IterableNE<A> createRangeUnsafe<A>(IEnumerable<A> items)
    {
        var iter = Iterator.from(items);
        if (iter.IsEmpty) throw new ArgumentException("Can't create an IterableNE from an empty sequence");
        var head = iter.Head;
        var tail = iter.Tail.AsIterable();
        return new IterableNE<A>(head, tail);
    }

    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<IterableNE<A>> generate<A>(int count, Func<int, A> generator) =>
        count < 1
            ? None
            : createRange(Range(0, count).Select(generator));

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<IterableNE<A>> repeat<A>(A item, int count) =>
        count < 1
            ? None
            : createRange(Range(0, count).Select(_ => item));

    /// <summary>
    /// Get the item at the head (first) of the sequence
    /// </summary>
    /// <param name="list">sequence</param>
    /// <returns>Head item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A head<A>(IterableNE<A> list) =>
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
    public static Option<IterableNE<B>> choose<A, B>(IterableNE<A> list, Func<A, Option<B>> selector) =>
        list.Map(selector)
            .Filter(t => t.IsSome)
            .Map(ts => ts.Map(t => t.Value!));

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IterableNE<A> rev<A>(IterableNE<A> list) =>
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
    public static IterableNE<S> scan<S, A>(IterableNE<A> list, S state, Func<S, A, S> folder) =>
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
    public static IterableNE<S> scanBack<S, A>(IterableNE<A> list, S state, Func<S, A, S> folder) =>
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
    public static IterableNE<C> zip<A, B, C>(IterableNE<A> list, IterableNE<B> other, Func<A, B, C> zipper) =>
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
    public static IterableNE<(A First, B Second)> zip<A, B>(IterableNE<A> list, IterableNE<B> other) =>
        list.Zip(other, (t, u) => (t, u));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IterableNE<A> distinct<A>(IterableNE<A> list) =>
        list.Distinct();

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IterableNE<A> distinct<EqA, A>(IterableNE<A> list) where EqA : Eq<A> =>
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
    public static Option<IterableNE<A>> take<A>(IterableNE<A> list, int count) =>
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
    public static Option<IterableNE<A>> takeWhile<A>(IterableNE<A> list, Func<A, bool> pred) =>
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
    public static Option<IterableNE<A>> takeWhile<A>(IterableNE<A> list, Func<A, int, bool> pred) =>
        list.TakeWhile(pred);
}
