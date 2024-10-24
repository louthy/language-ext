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
public partial class Seq
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> flatten<A>(Seq<Seq<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> empty<A>() =>
        Seq<A>.Empty;

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> singleton<A>(A value) =>
        [value];

    /// <summary>
    /// Create a new empty sequence
    /// </summary>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> create<A>() =>
        Seq<A>.Empty;

    /// <summary>
    /// Create a sequence from a initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Seq<A> create<A>(params A[] items)
    {
        if (items.Length == 0) return Seq<A>.Empty;
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
    public static Seq<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.Length == 0 ? Seq<A>.Empty : new (items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> createRange<A>(IEnumerable<A> items) =>
        new (items);

    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> generate<A>(int count, Func<int, A> generator) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(generator).ToSeq();

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> repeat<A>(A item, int count) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(_ => item).ToSeq();

    /// <summary>
    /// Get the item at the head (first) of the sequence
    /// </summary>
    /// <param name="list">sequence</param>
    /// <returns>Head item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> head<A>(Seq<A> list) => 
        list.Head;

    /// <summary>
    /// Get the last item of the sequence
    /// </summary>
    /// <param name="list">sequence</param>
    /// <returns>Last item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> last<A>(Seq<A> list) =>
        list.Last;

    /// <summary>
    /// Get all items in the list except the last one
    /// </summary>
    /// <remarks>
    /// Must evaluate the last item to know it's the last, but won't return it
    /// </remarks>
    /// <param name="list">List</param>
    /// <returns>The initial items (all but the last)</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> init<A>(Seq<A> list) =>
        list.Init;

    /// <summary>
    /// Get the tail of the sequence (skips the head item)
    /// </summary>
    /// <param name="list">sequence</param>
    /// <returns>Tail sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> tail<A>(Seq<A> list) =>
        list.Tail;

    /// <summary>
    /// Projects the values in the sequence using a map function into a new sequence (Select in LINQ).
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <typeparam name="B">Return sequence item type</typeparam>
    /// <param name="list">sequence to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<B> map<A, B>(Seq<A> list, Func<A, B> map) =>
        list.Select(map);

    /// <summary>
    /// Projects the values in the sequence into a new sequence using a map function, which is also given an index value
    /// (Select in LINQ - note that the order of the arguments of the map function are the other way around, here the index
    /// is the first argument).
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <typeparam name="B">Return sequence item type</typeparam>
    /// <param name="list">sequence to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<B> map<A, B>(Seq<A> list, Func<int, A, B> map) =>
        toSeq(zip(list, toSeq(Range(0, int.MaxValue)), (t, i) => map(i, t)));

    /// <summary>
    /// Removes items from the sequence that do not match the given predicate (Where in LINQ)
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence to filter</param>
    /// <param name="predicate">Predicate function</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> filter<A>(Seq<A> list, Func<A, bool> predicate) =>
        list.Where(predicate);

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
    public static Seq<B> choose<A, B>(Seq<A> list, Func<A, Option<B>> selector) =>
        map(filter(map(list, selector), t => t.IsSome), t => t.Value!);

    /// <summary>
    /// Applies the given function 'selector' to each element of the sequence. Returns the 
    /// sequence comprised of the results for each element where the function returns Some(f(x)).
    /// An index value is passed through to the selector function also.
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<B> choose<A, B>(Seq<A> list, Func<int, A, Option<B>> selector) =>
        map(filter(map(list, selector), t => t.IsSome), t => t.Value!);

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<T> rev<T>(Seq<T> list) =>
        toSeq(list.Reverse());

    /// <summary>
    /// Concatenate two sequences (Concat in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="lhs">First sequence</param>
    /// <param name="rhs">Second sequence</param>
    /// <returns>Concatenated sequence</returns>
    [Pure]
    public static Seq<T> append<T>(Seq<T> lhs, Seq<T> rhs) =>
        lhs.Concat(rhs);

    /// <summary>
    /// Concatenate a sequence and a sequence of sequences
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="lhs">First list</param>
    /// <param name="rhs">Second list</param>
    /// <returns>Concatenated list</returns>
    [Pure]
    public static Seq<T> append<T>(Seq<T> x, Seq<Seq<T>> xs) =>
        head(xs).IsNone
            ? x
            : append(x, append((Seq<T>)xs.Head, xs.Skip(1)));

    /// <summary>
    /// Concatenate N sequences
    /// </summary>
    /// <typeparam name="T">sequence type</typeparam>
    /// <param name="lists">sequences to concatenate</param>
    /// <returns>A single sequence with all of the items concatenated</returns>
    [Pure]
    public static Seq<T> append<T>(params Seq<T>[] lists) =>
        lists.Length switch
        {
            0 => Seq<T>.Empty,
            1 => lists[0],
            _ => append(lists[0], toSeq(lists).Skip(1))
        };

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
    public static Seq<S> scan<S, T>(Seq<T> list, S state, Func<S, T, S> folder)
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
        return toSeq(Yield());
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
    public static Seq<S> scanBack<S, T>(Seq<T> list, S state, Func<S, T, S> folder) =>
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
    public static Seq<V> zip<T, U, V>(Seq<T> list, Seq<U> other, Func<T, U, V> zipper) =>
        toSeq(list.Zip(other, zipper));

    /// <summary>
    /// Joins two sequences together either into an sequence of tuples
    /// </summary>
    /// <param name="list">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<(T First, U Second)> zip<T, U>(Seq<T> list, Seq<U> other) =>
        toSeq(list.Zip(other, (t, u) => (t, u)));


    /// <summary>
    /// Returns true if all items in the sequence match a predicate (Any in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to test</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the sequence match the predicate</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool forall<T>(Seq<T> list, Func<T, bool> pred) =>
        list.ForAll(pred);

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<T> distinct<T>(Seq<T> list) =>
        toSeq(list.Distinct());

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<T> distinct<EQ, T>(Seq<T> list) where EQ : Eq<T> =>
        toSeq(list.Distinct(new EqCompare<T>(static (x, y) => EQ.Equals(x, y), static x => EQ.GetHashCode(x))));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<T> distinct<T, K>(Seq<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default) =>
        toSeq(list.Distinct(new EqCompare<T>(
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
    public static Seq<T> take<T>(Seq<T> list, int count) =>
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
    public static Seq<T> takeWhile<T>(Seq<T> list, Func<T, bool> pred) =>
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
    public static Seq<T> takeWhile<T>(Seq<T> list, Func<T, int, bool> pred) =>
        list.TakeWhile(pred);

    /// <summary>
    /// The tails function returns all final segments of the argument, longest first. For example:
    /// 
    ///     tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
    /// </summary>
    /// <typeparam name="A">Seq item type</typeparam>
    /// <param name="self">Seq</param>
    /// <returns>Seq of Seq of A</returns>
    [Pure]
    public static Seq<Seq<A>> tails<A>(Seq<A> self)
    {
        var res = Seq<Seq<A>>.Empty;
        while (!self.IsEmpty)
        {
            res = self.Cons(res);
            self = self.Tail;
        }
        return rev(res);
    }

    /// <summary>
    /// The tailsr function returns all final segments of the argument, longest first. For example:
    /// 
    ///     tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
    /// </summary>
    /// <remarks>Differs from `tails` in implementation only.  The `tailsr` uses recursive processing
    /// whereas `tails` uses a while loop aggregation followed by a reverse.  For small sequences 
    /// `tailsr` is probably more efficient. </remarks>
    /// <typeparam name="A">Seq item type</typeparam>
    /// <param name="self">Seq</param>
    /// <returns>Seq of Seq of A</returns>
    [Pure]
    public static Seq<Seq<A>> tailsr<A>(Seq<A> self) =>
        self.Match(
            () => Seq<Seq<A>>.Empty,
            xs => xs.Cons(tailsr(xs.Tail)));

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
    public static (Seq<T>, Seq<T>) span<T>(Seq<T> self, Func<T, bool> pred)
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
    internal static Seq<A> FromSingleValue<A>(A value) =>
        new (SeqStrict<A>.FromSingleValue(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Seq<A> FromArray<A>(A[] value) =>
        new (new SeqStrict<A>(value, 0, value.Length, 0, 0));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Seq<A> FromArray<A>(A[] value, int length) =>
        new (new SeqStrict<A>(value, 0, length, 0, 0));
}
