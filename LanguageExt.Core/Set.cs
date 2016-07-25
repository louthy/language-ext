using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable set module
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// http://en.wikipedia.org/wiki/AVL_tree
    /// </summary>
    public static class Set
    {
        /// <summary>
        /// True if the set has no elements
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <returns>True if the set has no elements</returns>
        [Pure]
        public static bool isEmpty<A>(Set<A> set) =>
            set.IsEmpty;

        /// <summary>
        /// Create a new empty set
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <returns>Empty set</returns>
        [Pure]
        public static Set<A> create<A>() =>
            Set<A>.Empty;

        /// <summary>
        /// Create a new set pre-populated with the items in range
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="range">Range of items</param>
        /// <returns>Set</returns>
        [Pure]
        public static Set<A> createRange<A>(IEnumerable<A> range) =>
            new Set<A>(range);

        /// <summary>
        /// Create a new empty set
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <returns>Empty set</returns>
        [Pure]
        public static Set<A> empty<A>() =>
            Set<A>.Empty;

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public static Set<A> add<A>(Set<A> set, A value) =>
            set.Add(value);

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public static Set<A> tryAdd<A>(Set<A> set, A value) =>
            set.TryAdd(value);

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public static Set<A> addOrUpdate<A>(Set<A> set, A value) =>
            set.AddOrUpdate(value);

        /// <summary>
        /// Attempts to find an item in the set.  
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="value">Value to find</param>
        /// <returns>Some(T) if found, None otherwise</returns>
        [Pure]
        public static Option<A> find<A>(Set<A> set, A value) =>
            set.Find(value);

        /// <summary>
        /// Check the existence of an item in the set using a 
        /// predicate.
        /// </summary>
        /// <remarks>Note this scans the entire set.</remarks>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if predicate returns true for any item</returns>
        [Pure]
        public static bool exists<A>(Set<A> set, Func<A, bool> pred) =>
            set.Exists(pred);

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        [Pure]
        public static bool equals<A>(Set<A> setA, Set<A> setB) =>
            setA.SetEquals(setB);

        /// <summary>
        /// Get the number of elements in the set
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <returns>Number of elements</returns>
        [Pure]
        public static int length<A>(Set<A> set) =>
            set.Count();

        /// <summary>
        /// Returns setA - setB.  Only the items in setA that are not in 
        /// setB will be returned.
        /// </summary>
        [Pure]
        public static Set<A> difference<A>(Set<A> setA, Set<A> setB) =>
            setA.Except(setB);

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set A</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public static Set<T> union<T>(Set<T> setA, Set<T> setB) =>
            setA.Union(setB);

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public static Set<T> filter<T>(Set<T> set, Func<T, bool> pred) =>
            set.Filter(pred);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection, threading an accumulator 
        /// argument through the computation. The fold function takes the state argument, and 
        /// applies the function 'folder' to it and the first element of the set. Then, it feeds this 
        /// result into the function 'folder' along with the second element, and so on. It returns the 
        /// final result. (Aggregate in LINQ)
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Set element type</typeparam>
        /// <param name="set">Set to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S fold<T, S>(Set<T> set, S state, Func<S, T, S> folder) =>
            set.Fold(state, folder);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection (from last element to first), 
        /// threading an aggregate state through the computation. The fold function takes the state 
        /// argument, and applies the function 'folder' to it and the first element of the set. Then, 
        /// it feeds this result into the function 'folder' along with the second element, and so on. It 
        /// returns the final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Set element type</typeparam>
        /// <param name="set">Set to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldBack<T, S>(Set<T> set, S state, Func<S, T, S> folder) =>
            set.FoldBack(state, folder);

        /// <summary>
        /// Returns the elements that are in both setA and setB
        /// </summary>
        [Pure]
        public static Set<A> intersect<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.Intersect(setB);

        /// <summary>
        /// Returns the elements that are in both setA and setB
        /// </summary>
        [Pure]
        public static Set<A> except<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.Except(setB);

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public static Set<A> symmetricExcept<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.Except(setB);

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <typeparam name="B">Mapped element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static Set<B> map<A, B>(Set<A> set, Func<A, B> mapper) =>
            set.Map(mapper);

        /// <summary>
        /// Returns True if the value is in the set
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the item 'value' is in the Set 'set'</returns>
        [Pure]
        public static bool contains<A>(Set<A> set, A value) =>
            set.Contains(value);

        /// <summary>
        /// Removes an item from the set (if it exists)
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="value">Value to check</param>
        /// <returns>New set with item removed</returns>
        [Pure]
        public static Set<A> remove<A>(Set<A> set, A value) =>
            set.Remove(value);

        /// <summary>
        /// Returns True if setB is a subset of setA
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a subset of setA</returns>
        [Pure]
        public static bool isSubset<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.IsSubsetOf(setB);

        /// <summary>
        /// Returns True if setB is a superset of setA
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a superset of setA</returns>
        [Pure]
        public static bool isSuperset<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.IsSupersetOf(setB);

        /// <summary>
        /// Returns True if setB is a proper subset of setA
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a proper subset of setA</returns>
        [Pure]
        public static bool isProperSubset<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.IsProperSubsetOf(setB);

        /// <summary>
        /// Returns True if setB is a proper superset of setA
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a proper subset of setA</returns>
        [Pure]
        public static bool isProperSuperset<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.IsProperSupersetOf(setB);

        /// <summary>
        /// Returns True if setA overlaps setB
        /// </summary>
        /// <typeparam name="A">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if setA overlaps setB</returns>
        [Pure]
        public static bool overlaps<A>(Set<A> setA, IEnumerable<A> setB) =>
            setA.Overlaps(setB);
    }
}
