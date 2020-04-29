using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Cons sequence module
    /// Represents a sequence of values in a similar way to IEnumerable, but without the
    /// issues of multiple evaluation for key LINQ operators like Skip, Count, etc.
    /// </summary>
    /// <typeparam name="A">Type of the values in the sequence</typeparam>
    public static class Seq
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
        public static Seq<A> createRange<A>(IEnumerable<A> items) =>
            new Seq<A>(items);

        /// <summary>
        /// Generates a sequence of A using the provided delegate to initialise
        /// each item.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<A> generate<A>(int count, Func<int, A> generator) =>
            Seq(Range(0, count).Map(generator));

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<A> repeat<A>(A item, int count) =>
            Seq(Range(0, count).Map(_ => item));

        /// <summary>
        /// Get the item at the head (first) of the sequence
        /// </summary>
        /// <param name="list">sequence</param>
        /// <returns>Head item</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A head<A>(Seq<A> list) => 
            list.Head;

        /// <summary>
        /// Get the item at the head (first) of the sequence or None if the sequence is empty
        /// </summary>
        /// <param name="list">sequence</param>
        /// <returns>Optional head item</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<A> headOrNone<A>(Seq<A> list) =>
            list.HeadOrNone();

        /// <summary>
        /// Get the item at the head (first) of the sequence or Fail if the sequence is empty
        /// </summary>
        /// <param name="list">sequence</param>
        /// <param name="fail">Fail case</param>
        /// <returns>Validated head item</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Validation<Fail, A> headOrInvalid<Fail, A>(Seq<A> list, Fail fail) =>
            list.HeadOrInvalid(fail);

        /// <summary>
        /// Get the item at the head (first) of the sequence or Fail if the sequence is empty
        /// </summary>
        /// <param name="list">sequence</param>
        /// <param name="fail">Fail case</param>
        /// <returns>Validated head item</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Validation<MonoidFail, Fail, A> headOrInvalid<MonoidFail, Fail, A>(Seq<A> list, Fail fail) where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            list.HeadOrInvalid<MonoidFail, Fail, A>(fail);

        /// <summary>
        /// Get the item at the head (first) of the sequence or Left if the sequence is empty
        /// </summary>
        /// <param name="list">sequence</param>
        /// <param name="left">Left case</param>
        /// <returns>Either head item or left</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, A> headOrLeft<L, A>(Seq<A> list, L left) =>
            list.HeadOrLeft(left);

        /// <summary>
        /// Get the last item of the sequence
        /// </summary>
        /// <param name="list">sequence</param>
        /// <returns>Last item</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A last<A>(Seq<A> list) =>
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
        /// Projects the values in the sequence using a map function into a new sequence (Select in LINQ).
        /// An index value is passed through to the map function also.
        /// </summary>
        /// <typeparam name="A">sequence item type</typeparam>
        /// <typeparam name="B">Return sequence item type</typeparam>
        /// <param name="list">sequence to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped sequence</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<B> map<A, B>(Seq<A> list, Func<int, A, B> map) =>
            Seq(zip(list, Seq(Range(0, Int32.MaxValue)), (t, i) => map(i, t)));

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
            map(filter(map(list, selector), t => t.IsSome), t => t.Value);

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
            map(filter(map(list, selector), t => t.IsSome), t => t.Value);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A sum<MonoidA, A>(Seq<A> list) where MonoidA : struct, Monoid<A> =>
            mconcat<MonoidA, A>(list);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int sum(Seq<int> list) =>
            fold(list, 0, (s, x) => s + x);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float sum(Seq<float> list) =>
            fold(list, 0.0f, (s, x) => s + x);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double sum(Seq<double> list) =>
            fold(list, 0.0, (s, x) => s + x);

        /// <summary>
        /// Returns the sum total of all the items in the list (Sum in LINQ)
        /// </summary>
        /// <param name="list">List to sum</param>
        /// <returns>Sum total</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal sum(Seq<decimal> list) =>
            fold(list, (decimal)0, (s, x) => s + x);

        /// <summary>
        /// Reverses the sequence (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence to reverse</param>
        /// <returns>Reversed sequence</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<T> rev<T>(Seq<T> list) =>
            Seq(list.Reverse());

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
            headOrNone(xs).IsNone
                ? x
                : append(x, append(xs.Head, xs.Skip(1)));

        /// <summary>
        /// Concatenate N sequences
        /// </summary>
        /// <typeparam name="T">sequence type</typeparam>
        /// <param name="lists">sequences to concatenate</param>
        /// <returns>A single sequence with all of the items concatenated</returns>
        [Pure]
        public static Seq<T> append<T>(params Seq<T>[] lists) =>
            lists.Length == 0
                ? Seq<T>.Empty
                : lists.Length == 1
                    ? lists[0]
                    : append(lists[0], Seq(lists).Skip(1));

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
        public static S fold<S, T>(Seq<T> list, S state, Func<S, T, S> folder) =>
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
        public static S foldBack<S, T>(Seq<T> list, S state, Func<S, T, S> folder) =>
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
        public static S foldWhile<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
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
        public static S foldWhile<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
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
        public static S foldBackWhile<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
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
        public static S foldBackWhile<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
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
        public static S foldUntil<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
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
        public static S foldUntil<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
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
        public static S foldBackUntil<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
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
        public static S foldBackUntil<S, T>(Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
            foldUntil(rev(list), state, folder, predstate: predstate);

        /// <summary>
        /// Applies a function to each element of the sequence (from last element to first), threading 
        /// an accumulator argument through the computation. This function first applies the function 
        /// to the first two elements of the sequence. Then, it passes this result into the function along 
        /// with the third element and so on. Finally, it returns the final result.
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence to reduce</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static T reduce<T>(Seq<T> list, Func<T, T, T> reducer) =>
            match(headOrNone(list),
                Some: x => fold(tail(list), x, reducer),
                None: () => failwith<T>("Input list was empty")
            );

        /// <summary>
        /// Applies a function to each element of the sequence, threading an accumulator argument 
        /// through the computation. This function first applies the function to the first two 
        /// elements of the sequence. Then, it passes this result into the function along with the third 
        /// element and so on. Finally, it returns the final result.
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence to reduce</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T reduceBack<T>(Seq<T> list, Func<T, T, T> reducer) =>
            reduce(rev(list), reducer);

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
            return Seq(Yield());
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
        /// Returns Some(x) for the first item in the sequence that matches the predicate 
        /// provided, None otherwise.
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence to search</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Some(x) for the first item in the sequence that matches the predicate 
        /// provided, None otherwise.</returns>
        [Pure]
        public static Option<T> find<T>(Seq<T> list, Func<T, bool> pred)
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
        public static Seq<T> findSeq<T>(Seq<T> list, Func<T, bool> pred)
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
            return Seq(Yield());
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
        public static Seq<V> zip<T, U, V>(Seq<T> list, Seq<U> other, Func<T, U, V> zipper) =>
            Seq(list.Zip(other, zipper));

        /// <summary>
        /// Joins two sequences together either into an sequence of tuples
        /// </summary>
        /// <param name="list">First sequence to join</param>
        /// <param name="other">Second sequence to join</param>
        /// <param name="zipper">Join function</param>
        /// <returns>Joined sequence of tuples</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<(T Left, U Right)> zip<T, U>(Seq<T> list, Seq<U> other) =>
            Seq(list.Zip(other, (t, u) => (t, u)));

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence to count</param>
        /// <returns>The number of items in the sequence</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int length<T>(Seq<T> list) =>
           list.Count;

        /// <summary>
        /// Invokes an action for each item in the sequence in order
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence to iterate</param>
        /// <param name="action">Action to invoke with each item</param>
        /// <returns>Unit</returns>
        public static Unit iter<T>(Seq<T> list, Action<T> action)
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
        public static Unit iter<T>(Seq<T> list, Action<int, T> action)
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
            Seq(list.Distinct());

        /// <summary>
        /// Return a new sequence with all duplicate values removed
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence</param>
        /// <returns>A new sequence with all duplicate values removed</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<T> distinct<EQ, T>(Seq<T> list) where EQ : struct, Eq<T> =>
            Seq(list.Distinct(new EqCompare<T>((x, y) => default(EQ).Equals(x, y))));

        /// <summary>
        /// Return a new sequence with all duplicate values removed
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence</param>
        /// <returns>A new sequence with all duplicate values removed</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<T> distinct<T, K>(Seq<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default(Option<Func<K, K, bool>>)) =>
             Seq(list.Distinct(new EqCompare<T>((a, b) => compare.IfNone(EqualityComparer<K>.Default.Equals)(keySelector(a), keySelector(b)), a => keySelector(a)?.GetHashCode() ?? 0)));

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
        /// Returns true if any item in the sequence matches the predicate provided
        /// </summary>
        /// <typeparam name="T">sequence item type</typeparam>
        /// <param name="list">sequence to test</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if any item in the sequence matches the predicate provided</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool exists<T>(Seq<T> list, Func<T, bool> pred) =>
            list.Exists(pred);

        /// <summary>
        /// Apply a sequence of values to a sequence of functions
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence of argument values</param>
        /// <returns>Returns the result of applying the sequence argument values to the sequence functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<B> apply<A, B>(Seq<Func<A, B>> fabc, Seq<A> fa) =>
            ApplSeq<A, B>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply a sequence of values to a sequence of functions
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence of argument values</param>
        /// <returns>Returns the result of applying the sequence argument values to the sequence functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<B> apply<A, B>(Func<A, B> fabc, Seq<A> fa) =>
            ApplSeq<A, B>.Inst.Apply(fabc.Cons(), fa);

        /// <summary>
        /// Apply a sequence of values to a sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of argument values to the 
        /// IEnumerable of functions: a sequence of functions of arity 1</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Func<B, C>> apply<A, B, C>(Seq<Func<A, B, C>> fabc, Seq<A> fa) =>
            ApplSeq<A, B, C>.Inst.Apply(fabc.Map(curry), fa);

        /// <summary>
        /// Apply a sequence of values to a sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of argument values to the 
        /// sequence of functions: a sequence of functions of arity 1</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, Seq<A> fa) =>
            ApplSeq<A, B, C>.Inst.Apply(curry(fabc).Cons(), fa);

        /// <summary>
        /// Apply sequence of values to a sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <param name="fb">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<C> apply<A, B, C>(Seq<Func<A, B, C>> fabc, Seq<A> fa, Seq<B> fb) =>
            ApplSeq<A, B, C>.Inst.Apply(fabc.Map(curry), fa, fb);

        /// <summary>
        /// Apply sequence of values to an sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <param name="fb">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<C> apply<A, B, C>(Func<A, B, C> fabc, Seq<A> fa, Seq<B> fb) =>
            ApplSeq<A, B, C>.Inst.Apply(curry(fabc).Cons(), fa, fb);

        /// <summary>
        /// Apply a sequence of values to a sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of argument values to the 
        /// sequence of functions: a sequence of functions of arity 1</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Func<B, C>> apply<A, B, C>(Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa) =>
            ApplSeq<A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply an sequence of values to an sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of argument values to the 
        /// sequence of functions: a sequence of functions of arity 1</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, Seq<A> fa) =>
            ApplSeq<A, B, C>.Inst.Apply(fabc.Cons(), fa);

        /// <summary>
        /// Apply sequence of values to an sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <param name="fb">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<C> apply<A, B, C>(Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa, Seq<B> fb) =>
            ApplSeq<A, B, C>.Inst.Apply(fabc, fa, fb);

        /// <summary>
        /// Apply sequence of values to a sequence of functions of arity 2
        /// </summary>
        /// <param name="fabc">sequence of functions</param>
        /// <param name="fa">sequence argument values</param>
        /// <param name="fb">sequence argument values</param>
        /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<C> apply<A, B, C>(Func<A, Func<B, C>> fabc, Seq<A> fa, Seq<B> fb) =>
            ApplSeq<A, B, C>.Inst.Apply(fabc.Cons(), fa, fb);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<B> action<A, B>(Seq<A> fa, Seq<B> fb) =>
            ApplSeq<A, B>.Inst.Action(fa, fb);

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
            new Seq<A>(SeqStrict<A>.FromSingleValue(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Seq<A> FromArray<A>(A[] value) =>
            new Seq<A>(new SeqStrict<A>(value, 0, value.Length, 0, 0));
    }
}
