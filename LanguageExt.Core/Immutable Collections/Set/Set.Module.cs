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
    public static partial class Set
    {
        /// <summary>
        /// True if the set has no elements
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>True if the set has no elements</returns>
        [Pure]
        public static bool isEmpty<T>(Set<T> set) =>
            set.IsEmpty;

        /// <summary>
        /// Create a new empty set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Empty set</returns>
        [Pure]
        public static Set<T> create<T>() =>
            Set<T>.Empty;

        /// <summary>
        /// Create a new set pre-populated with the items in range
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="range">Range of items</param>
        /// <returns>Set</returns>
        [Pure]
        public static Set<T> createRange<T>(IEnumerable<T> range) =>
            new Set<T>(range);

        /// <summary>
        /// Create a new empty set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Empty set</returns>
        [Pure]
        public static Set<T> empty<T>() =>
            Set<T>.Empty;

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public static Set<T> add<T>(Set<T> set, T value) =>
            set.Add(value);

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public static Set<T> tryAdd<T>(Set<T> set, T value) =>
            set.TryAdd(value);

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public static Set<T> addOrUpdate<T>(Set<T> set, T value) =>
            set.AddOrUpdate(value);


        /// <summary>
        /// Atomically adds a range of items to the set.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New Set with the items added</returns>
        [Pure]
        public static Set<A> addRange<A>(Set<A> set, IEnumerable<A> range) =>
            set.AddRange(range);

        /// <summary>
        /// Atomically adds a range of items to the set.  If an item already exists, it's ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New Set with the items added</returns>
        [Pure]
        public static Set<A> tryAddRange<A>(Set<A> set, IEnumerable<A> range) =>
            set.TryAddRange(range);

        /// <summary>
        /// Atomically adds a range of items to the set.  If an item already exists then replace it.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New Set with the items added</returns>
        [Pure]
        public static Set<A> addOrUpdateRange<A>(Set<A> set, IEnumerable<A> range) =>
            set.AddOrUpdateRange(range);

        /// <summary>
        /// Attempts to find an item in the set.  
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="value">Value to find</param>
        /// <returns>Some(T) if found, None otherwise</returns>
        [Pure]
        public static Option<T> find<T>(Set<T> set, T value) =>
            set.Find(value);

        /// <summary>
        /// Check the existence of an item in the set using a 
        /// predicate.
        /// </summary>
        /// <remarks>Note this scans the entire set.</remarks>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if predicate returns true for any item</returns>
        [Pure]
        public static bool exists<T>(Set<T> set, Func<T, bool> pred) =>
            set.Exists(pred);

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        [Pure]
        public static bool equals<T>(Set<T> setA, Set<T> setB) =>
            setA.SetEquals(setB);

        /// <summary>
        /// Get the number of elements in the set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <returns>Number of elements</returns>
        [Pure]
        public static int length<T>(Set<T> set) =>
            set.Count();

        /// <summary>
        /// Returns setA - setB.  Only the items in setA that are not in 
        /// setB will be returned.
        /// </summary>
        [Pure]
        public static Set<T> subtract<T>(Set<T> setA, Set<T> setB) =>
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
        public static Set<T> intersect<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.Intersect(setB);

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public static Set<T> except<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.Except(setB);

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public static Set<T> symmetricExcept<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.SymmetricExcept(setB);

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static Set<R> map<T, R>(Set<T> set, Func<T, R> mapper) =>
            set.Map(mapper);

        /// <summary>
        /// Returns True if the value is in the set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the item 'value' is in the Set 'set'</returns>
        [Pure]
        public static bool contains<T>(Set<T> set, T value) =>
            set.Contains(value);

        /// <summary>
        /// Removes an item from the set (if it exists)
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set</param>
        /// <param name="value">Value to check</param>
        /// <returns>New set with item removed</returns>
        [Pure]
        public static Set<T> remove<T>(Set<T> set, T value) =>
            set.Remove(value);

        /// <summary>
        /// Returns True if setB is a subset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a subset of setA</returns>
        [Pure]
        public static bool isSubset<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.IsSubsetOf(setB);

        /// <summary>
        /// Returns True if setB is a superset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a superset of setA</returns>
        [Pure]
        public static bool isSuperset<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.IsSupersetOf(setB);

        /// <summary>
        /// Returns True if setB is a proper subset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a proper subset of setA</returns>
        [Pure]
        public static bool isProperSubset<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.IsProperSubsetOf(setB);

        /// <summary>
        /// Returns True if setB is a proper superset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a proper subset of setA</returns>
        [Pure]
        public static bool isProperSuperset<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.IsProperSupersetOf(setB);

        /// <summary>
        /// Returns True if setA overlaps setB
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if setA overlaps setB</returns>
        [Pure]
        public static bool overlaps<T>(Set<T> setA, IEnumerable<T> setB) =>
            setA.Overlaps(setB);
    }
}
