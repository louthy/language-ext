#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Cons sequence module
/// Represents a sequence of values in a similar way to IEnumerable, but without the
/// issues of multiple evaluation for key LINQ operators like Skip, Count, etc.
/// </summary>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
public partial class EnumerableM
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> flatten<A>(EnumerableM<EnumerableM<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> empty<A>() =>
        EnumerableM<A>.Empty;

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> singleton<A>(A value) =>
        [value];

    /// <summary>
    /// Create a new empty sequence
    /// </summary>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> create<A>() =>
        EnumerableM<A>.Empty;

    /// <summary>
    /// Create a sequence from a initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static EnumerableM<A> create<A>(params A[] items)
    {
        if (items.Length == 0) return EnumerableM<A>.Empty;
        var nitems = new A[items.Length];
        System.Array.Copy(items, nitems, items.Length);
        return FromArray(items);
    }

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.Length == 0 ? EnumerableM<A>.Empty : new (items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> createRange<A>(IEnumerable<A> items) =>
        new (items);

    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> generate<A>(int count, Func<int, A> generator) =>
        Range(0, count).Map(generator).As().runRange.AsEnumerableM();

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> repeat<A>(A item, int count) =>
        Range(0, count).Map(_ => item).As().runRange.AsEnumerableM();

    /// <summary>
    /// Get the item at the head (first) of the sequence or None if the sequence is empty
    /// </summary>
    /// <param name="list">sequence</param>
    /// <returns>Optional head item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> head<A>(EnumerableM<A> list) =>
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
    public static EnumerableM<B> choose<A, B>(EnumerableM<A> list, Func<A, Option<B>> selector) =>
        list.Map(selector).Filter(t => t.IsSome).Map(t => t.Value!);

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<T> rev<T>(EnumerableM<T> list) =>
        new (list.Reverse());

    /// <summary>
    /// Applies a function to each element of the sequence, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the sequence. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static EnumerableM<S> scan<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder)
    {
        IEnumerable<S> Yield()
        {
            yield return state;
            foreach (var item in list)
            {
                state = folder(state, item);
                yield return state;
            }
        }
        return new (Yield());
    }

    /// <summary>
    /// Applies a function to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the sequence. Then, it 
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<S> scanBack<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder) =>
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
    public static EnumerableM<V> zip<T, U, V>(EnumerableM<T> list, EnumerableM<U> other, Func<T, U, V> zipper) =>
        new (list.Zip(other, zipper));

    /// <summary>
    /// Joins two sequences together either into an sequence of tuples
    /// </summary>
    /// <param name="list">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<(T Left, U Right)> zip<T, U>(EnumerableM<T> list, EnumerableM<U> other) =>
        new (list.Zip(other, (t, u) => (t, u)));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<T> distinct<T>(EnumerableM<T> list) =>
        new (list.Distinct());

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<T> distinct<EQ, T>(EnumerableM<T> list) where EQ : Eq<T> =>
        new (list.Distinct(new EqCompare<T>(static (x, y) => EQ.Equals(x, y), static x => EQ.GetHashCode(x))));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<T> distinct<T, K>(EnumerableM<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default) =>
        new (list.Distinct(new EqCompare<T>(
                               (a, b) => compare.IfNone(EqDefault<K>.Equals)(keySelector(a), keySelector(b)), 
                               a => compare.Match(Some: _  => 0, None: () => EqDefault<K>.GetHashCode(keySelector(a))))));

    /// <summary>
    /// Returns a new sequence with the first 'count' items from the sequence provided
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first 'count' items from the sequence provided</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<T> take<T>(EnumerableM<T> list, int count) =>
        list.Take(count);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first items that match the predicate</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<T> takeWhile<T>(EnumerableM<T> list, Func<T, bool> pred) =>
        list.TakeWhile(pred);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't.  An index value is also provided to the predicate function.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new sequence with the first items that match the predicate</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<T> takeWhile<T>(EnumerableM<T> list, Func<T, int, bool> pred) =>
        list.TakeWhile(pred);

    /// <summary>
    /// Span, applied to a predicate 'pred' and a list, returns a tuple where first element is 
    /// longest prefix (possibly empty) of elements that satisfy 'pred' and second element is the 
    /// remainder of the list:
    /// </summary>
    /// <example>
    /// Seq.span(Seq(1,2,3,4,1,2,3,4), x => x &lt; 3) == (Seq(1,2), Seq(3,4,1,2,3,4))
    /// </example>
    /// <example>
    /// Seq.span(Seq(1,2,3), x => x &lt; 9) == (Seq(1,2,3), Seq())
    /// </example>
    /// <example>
    /// Seq.span(Seq(1,2,3), x => x &lt; 0) == (Seq(), Seq(1,2,3))
    /// </example>
    /// <typeparam name="T">List element type</typeparam>
    /// <param name="self">List</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Split list</returns>
    [Pure]
    public static (EnumerableM<T>, EnumerableM<T>) span<T>(EnumerableM<T> self, Func<T, bool> pred)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static EnumerableM<A> FromSingleValue<A>(A value) =>
        new (SeqStrict<A>.FromSingleValue(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static EnumerableM<A> FromArray<A>(A[] value) =>
        new (new SeqStrict<A>(value, 0, value.Length, 0, 0));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static EnumerableM<A> FromArray<A>(A[] value, int length) =>
        new (new SeqStrict<A>(value, 0, length, 0, 0));
}
