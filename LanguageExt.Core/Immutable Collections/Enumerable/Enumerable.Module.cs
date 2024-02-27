#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using static LanguageExt.Trait;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
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
        new (Range(0, count).Map(generator));

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> repeat<A>(A item, int count) =>
        new (Range(0, count).Map(_ => item));

    /// <summary>
    /// Get the item at the head (first) of the sequence or None if the sequence is empty
    /// </summary>
    /// <param name="list">sequence</param>
    /// <returns>Optional head item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> headOrNone<A>(EnumerableM<A> list) =>
        list.HeadOrNone();

    /// <summary>
    /// Get the item at the head (first) of the sequence or Fail if the sequence is empty
    /// </summary>
    /// <param name="list">sequence</param>
    /// <param name="fail">Fail case</param>
    /// <returns>Validated head item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<Fail, A> headOrInvalid<Fail, A>(EnumerableM<A> list, Fail fail) 
        where Fail : Monoid<Fail> =>
        list.HeadOrInvalid(fail);

    /// <summary>
    /// Get the item at the head (first) of the sequence or Fail if the sequence is empty
    /// </summary>
    /// <param name="list">sequence</param>
    /// <param name="fail">Fail case</param>
    /// <returns>Validated head item</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<Fail, A> headOrInvalid<Fail, A>(EnumerableM<A> list) 
        where Fail : Monoid<Fail> =>
        list.HeadOrInvalid(Fail.Empty);

    /// <summary>
    /// Get the item at the head (first) of the sequence or Left if the sequence is empty
    /// </summary>
    /// <param name="list">sequence</param>
    /// <param name="left">Left case</param>
    /// <returns>Either head item or left</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<L, A> headOrLeft<L, A>(EnumerableM<A> list, L left) =>
        list.HeadOrLeft(left);

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
    public static EnumerableM<B> map<A, B>(EnumerableM<A> list, Func<A, B> map) =>
        list.Select(map);

    /// <summary>
    /// Removes items from the sequence that do not match the given predicate (Where in LINQ)
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence to filter</param>
    /// <param name="predicate">Predicate function</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> filter<A>(EnumerableM<A> list, Func<A, bool> predicate) =>
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
    public static EnumerableM<B> choose<A, B>(EnumerableM<A> list, Func<A, Option<B>> selector) =>
        map(filter(map(list, selector), t => t.IsSome), t => t.Value!);

    /// <summary>
    /// Returns the sum total of all the items in the list (Sum in LINQ)
    /// </summary>
    /// <param name="list">List to sum</param>
    /// <returns>Sum total</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A sum<A>(EnumerableM<A> list) where A : INumber<A> =>
        fold(list, A.Zero, (s, x) => s + x);

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
    /// Concatenate two sequences (Concat in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="lhs">First sequence</param>
    /// <param name="rhs">Second sequence</param>
    /// <returns>Concatenated sequence</returns>
    [Pure]
    public static EnumerableM<T> append<T>(EnumerableM<T> lhs, EnumerableM<T> rhs) =>
        lhs.Concat(rhs);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence, threading an accumulator 
    /// argument through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the sequence. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result. (Aggregate in LINQ)
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static S fold<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder) =>
        list.Fold(state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence (from last element to first), 
    /// threading an aggregate state through the computation. The fold function takes the state 
    /// argument, and applies the function 'folder' to it and the first element of the sequence. Then, 
    /// it feeds this result into the function 'folder' along with the second element, and so on. It 
    /// returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static S foldBack<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder) =>
        list.FoldBack(state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence whilst the predicate function 
    /// returns True for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the sequence. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S foldWhile<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
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
    /// Applies a function 'folder' to each element of the sequence, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns True when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the sequence. Then, it feeds this result into the 
    /// function 'folder' along with the second element, and so on. It returns the final result. 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S foldWhile<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
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
    /// Applies a function 'folder' to each element of the sequence (from last element to first)
    /// whilst the predicate function returns True for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the sequence. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static S foldBackWhile<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        foldWhile(rev(list), state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns True when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the sequence. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static S foldBackWhile<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        foldWhile(rev(list), state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence whilst the predicate function 
    /// returns False for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the sequence. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S foldUntil<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
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
    /// Applies a function 'folder' to each element of the sequence, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns False when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the sequence. Then, it feeds this result into the 
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
    public static S foldUntil<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
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
    /// Applies a function 'folder' to each element of the sequence (from last element to first)
    /// whilst the predicate function returns False for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the sequence. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static S foldBackUntil<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        foldUntil(rev(list), state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns False when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the sequence. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static S foldBackUntil<S, T>(EnumerableM<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        foldUntil(rev(list), state, folder, predstate: predstate);


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
    /// Returns Some(x) for the first item in the sequence that matches the predicate 
    /// provided, None otherwise.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to search</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Some(x) for the first item in the sequence that matches the predicate 
    /// provided, None otherwise.</returns>
    [Pure]
    public static Option<T> find<T>(EnumerableM<T> list, Func<T, bool> pred)
    {
        foreach (var item in list)
        {
            if (pred(item)) return Some(item);
        }
        return None;
    }

    /// <summary>
    /// Returns [x] for the first item in the sequence that matches the predicate 
    /// provided, [] otherwise.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to search</param>
    /// <param name="pred">Predicate</param>
    /// <returns>[x] for the first item in the sequence that matches the predicate 
    /// provided, [] otherwise.</returns>
    [Pure]
    public static EnumerableM<T> findEnumerableM<T>(EnumerableM<T> list, Func<T, bool> pred)
    {
        IEnumerable<T> Yield()
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
        return new (Yield());
    }

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
    /// Returns the number of items in the sequence
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to count</param>
    /// <returns>The number of items in the sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int length<T>(EnumerableM<T> list) =>
        list.Count();

    /// <summary>
    /// Invokes an action for each item in the sequence in order
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit iter<T>(EnumerableM<T> list, Action<T> action)
    {
        foreach (var item in list)
        {
            action(item);
        }
        return unit;
    }

    /// <summary>
    /// Invokes an action for each item in the sequence in order and supplies
    /// a running index value.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit iter<T>(EnumerableM<T> list, Action<int, T> action)
    {
        int i = 0;
        foreach (var item in list)
        {
            action(i++, item);
        }
        return unit;
    }

    /// <summary>
    /// Returns true if all items in the sequence match a predicate (Any in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to test</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the sequence match the predicate</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool forall<T>(EnumerableM<T> list, Func<T, bool> pred) =>
        list.ForAll(pred);

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
    /// Returns true if any item in the sequence matches the predicate provided
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to test</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if any item in the sequence matches the predicate provided</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool exists<T>(EnumerableM<T> list, Func<T, bool> pred) =>
        list.Exists(pred);

    /// <summary>
    /// Apply a sequence of values to a sequence of functions
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence of argument values</param>
    /// <returns>Returns the result of applying the sequence argument values to the sequence functions</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<B> apply<A, B>(EnumerableM<Func<A, B>> fabc, EnumerableM<A> fa) =>
        new (fabc.Apply(fa));

    /// <summary>
    /// Apply a sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// IEnumerable of functions: a sequence of functions of arity 1</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<Func<B, C>> apply<A, B, C>(EnumerableM<Func<A, B, C>> fabc, EnumerableM<A> fa) =>
        new (fabc.Apply(fa));

    /// <summary>
    /// Apply sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<C> apply<A, B, C>(EnumerableM<Func<A, B, C>> fabc, EnumerableM<A> fa, EnumerableM<B> fb) =>
        new (fabc.Apply(fa, fb));

    /// <summary>
    /// Apply a sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// sequence of functions: a sequence of functions of arity 1</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<Func<B, C>> apply<A, B, C>(EnumerableM<Func<A, Func<B, C>>> fabc, EnumerableM<A> fa) =>
        new (fabc.Apply(fa));

    /// <summary>
    /// Apply sequence of values to an sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<C> apply<A, B, C>(EnumerableM<Func<A, Func<B, C>>> fabc, EnumerableM<A> fa, EnumerableM<B> fb) =>
        new (fabc.Apply(fa, fb));

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<B> action<A, B>(EnumerableM<A> fa, EnumerableM<B> fb) =>
        new (fa.Action(fb));

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
