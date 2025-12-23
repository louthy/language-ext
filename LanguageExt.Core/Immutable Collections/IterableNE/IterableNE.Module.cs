using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
    public static IterableNE<A> create<A>(A head, IEnumerable<A> tail) =>
        new (head, Iterable.createRange(tail));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, IAsyncEnumerable<A> tail) =>
        new (head, Iterable.createRange(tail));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, Iterable<A> tail) =>
        new (head, tail);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableNE<A> create<A>(A head, IterableNE<A> tail) =>
        new (head, tail.AsIterable());

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
    public static IO<IterableNE<A>> createRange<A>(IAsyncEnumerable<A> items) =>
        IO.liftVAsync(async _ =>
                      {
                          var iter = IteratorAsync.from(items);
                          if (await iter.IsEmpty) throw new ArgumentException("Can't create an IterableNE from an empty sequence");
                          var head = await iter.Head;
                          var tail = (await iter.Tail).AsIterable();
                          return new IterableNE<A>(head, tail);
                      });

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<IterableNE<A>> createRange<A>(Iterable<A> items)
    {
        return IO.liftVAsync(_ => items.IsAsync
                                      ? async()
                                      : sync());
    
        async ValueTask<IterableNE<A>> async()
        {
            var iter = IteratorAsync.from(items);
            if (await iter.IsEmpty) throw new ArgumentException("Can't create an IterableNE from an empty sequence");
            var head = await iter.Head;
            var tail = (await iter.Tail).AsIterable();
            return new IterableNE<A>(head, tail);
        }

        ValueTask<IterableNE<A>> sync()
        {
            var iter = Iterator.from(items);
            if (iter.IsEmpty) throw new ArgumentException("Can't create an IterableNE from an empty sequence");
            var head = iter.Head;
            var tail = iter.Tail.AsIterable();
            return ValueTask.FromResult(new IterableNE<A>(head, tail));
        }
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
    public static Iterable<B> choose<A, B>(IterableNE<A> list, Func<A, Option<B>> selector) =>
        list.Map(selector)
            .Filter(t => t.IsSome)
            .Map(t => t.Value!);

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
    /// Returns a new sequence with the first 'count' items from the sequence provided
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first 'count' items from the sequence provided</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> take<A>(IterableNE<A> list, int count) =>
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
    public static Iterable<A> takeWhile<A>(IterableNE<A> list, Func<A, bool> pred) =>
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
    public static Iterable<A> takeWhile<A>(IterableNE<A> list, Func<A, int, bool> pred) =>
        list.TakeWhile(pred);
}
