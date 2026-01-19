using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

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
    public static IterableNE<A> flatten<A>(IterableNE<IterableNE<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    public static IterableNE<A> singleton<A>(A value) =>
        new (value, Iterator.empty<A>());

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, params ReadOnlySpan<A> tail) =>
        new (head, Iterator.forward(tail));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, IEnumerable<A> tail) =>
        new (head, Iterator.forward(tail));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, Iterable<A> tail) =>
        new (head, tail.ForwardIterator());

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, IterableNE<A> tail) =>
        new(head, Iterator.cons(tail.Head, tail.Tail));

    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static Option<IterableNE<A>> generate<A>(long count, Func<long, A> generator) =>
        count < 1
            ? None
            : create(generator(0L), Range(1L, count).Select(generator));

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    [Pure]
    public static Option<IterableNE<A>> repeat<A>(A item, long count) =>
        count < 1
            ? None
            : create(item, Range(1L, count).Select(_ => item));

    /// <summary>
    /// Get the item at the head (first) of the sequence
    /// </summary>
    /// <param name="items">sequence</param>
    /// <returns>Head item</returns>
    [Pure]
    public static A head<A>(IterableNE<A> items) =>
        items.Head;

    /// <summary>
    /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
    /// of results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public static Iterable<B> choose<A, B>(IterableNE<A> items, Func<A, Option<B>> selector) =>
        items.Map(selector)
            .Filter(t => t.IsSome)
            .Map(t => t.Value!);

    /// <summary>
    /// Joins two sequences together either into a single sequence using the join 
    /// function provided
    /// </summary>
    /// <param name="items">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence</returns>
    [Pure]
    public static IterableNE<C> zip<A, B, C>(IterableNE<A> items, IterableNE<B> other, Func<A, B, C> zipper) =>
        items.Zip(other, zipper);

    /// <summary>
    /// Joins two sequences together either into an sequence of tuples
    /// </summary>
    /// <param name="items">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    public static IterableNE<(A First, B Second)> zip<A, B>(IterableNE<A> items, IterableNE<B> other) =>
        items.Zip(other, (t, u) => (t, u));

    /// <summary>
    /// Returns a new sequence with the first 'count' items from the sequence provided
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first 'count' items from the sequence provided</returns>
    [Pure]
    public static Iterable<A> take<A>(IterableNE<A> items, long count) =>
        items.Take(count);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first items that match the predicate</returns>
    [Pure]
    public static Iterable<A> takeWhile<A>(IterableNE<A> items, Func<A, bool> pred) =>
        items.TakeWhile(pred);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first items that match the predicate</returns>
    [Pure]
    public static Iterable<A> takeUntil<A>(IterableNE<A> items, Func<A, bool> pred) =>
        items.TakeUntil(pred);    
}
