using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Immutable hash-set module
    /// </summary>
    public static partial class HashSet
    {
        /// <summary>
        /// True if the set has no elements
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>True if the set has no elements</returns>
        [Pure]
        public static bool isEmpty<EqT, T>(HashSet<EqT, T> set) where EqT : struct, Eq<T> =>
            set.IsEmpty;

        /// <summary>
        /// Create a new empty set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Empty HSet</returns>
        [Pure]
        public static HashSet<EqT, T> create<EqT, T>() where EqT : struct, Eq<T> =>
            HashSet<EqT, T>.Empty;

        /// <summary>
        /// Create a new set pre-populated with the items in range
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="range">Range of items</param>
        /// <returns>HSet</returns>
        [Pure]
        public static HashSet<EqT, T> createRange<EqT, T>(IEnumerable<T> range) where EqT : struct, Eq<T> =>
            new HashSet<EqT, T>(range);

        /// <summary>
        /// Create a new empty set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Empty HSet</returns>
        [Pure]
        public static HashSet<EqT, T> empty<EqT, T>() where EqT : struct, Eq<T> =>
            HashSet<EqT, T>.Empty;

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the HSet</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public static HashSet<EqT, T> add<EqT, T>(HashSet<EqT, T> set, T value) where EqT : struct, Eq<T> =>
            set.Add(value);

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the HSet</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public static HashSet<EqT, T> tryAdd<EqT, T>(HashSet<EqT, T> set, T value) where EqT : struct, Eq<T> =>
            set.TryAdd(value);

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">Set to add item to</param>
        /// <param name="value">Value to add to the HSet</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public static HashSet<EqT, T> addOrUpdate<EqT, T>(HashSet<EqT, T> set, T value) where EqT : struct, Eq<T> =>
            set.AddOrUpdate(value);

        /// <summary>
        /// Attempts to find an item in the set.  
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="value">Value to find</param>
        /// <returns>Some(T) if found, None otherwise</returns>
        [Pure]
        public static Option<T> find<EqT, T>(HashSet<EqT, T> set, T value) where EqT : struct, Eq<T> =>
            set.Find(value);

        /// <summary>
        /// Check the existence of an item in the set using a 
        /// predicate.
        /// </summary>
        /// <remarks>Note this scans the entire set.</remarks>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if predicate returns true for any item</returns>
        [Pure]
        public static bool exists<EqT, T>(HashSet<EqT, T> set, Func<T, bool> pred) where EqT : struct, Eq<T> =>
            set.Exists(pred);

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        [Pure]
        public static bool equals<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.Equals(setB);

        /// <summary>
        /// Get the number of elements in the set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <returns>Number of elements</returns>
        [Pure]
        public static int length<EqT, T>(HashSet<EqT, T> set) where EqT : struct, Eq<T> =>
            set.Count();

        /// <summary>
        /// Returns setA - setB.  Only the items in setA that are not in 
        /// setB will be returned.
        /// </summary>
        [Pure]
        public static HashSet<EqT, T> subtract<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
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
        public static HashSet<EqT, T> union<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.Union(setB);

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public static HashSet<EqT, T> filter<EqT, T>(HashSet<EqT, T> set, Func<T, bool> pred) where EqT : struct, Eq<T> =>
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
        public static S fold<EqT, T, S>(HashSet<EqT, T> set, S state, Func<S, T, S> folder) where EqT : struct, Eq<T> =>
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
        public static S foldBack<EqT, T, S>(HashSet<EqT, T> set, S state, Func<S, T, S> folder) where EqT : struct, Eq<T> =>
            set.FoldBack(state, folder);

        /// <summary>
        /// Returns the elements that are in both setA and setB
        /// </summary>
        [Pure]
        public static HashSet<EqT, T> intersect<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.Intersect(setB);

        /// <summary>
        /// Returns the elements that are in both setA and setB
        /// </summary>
        [Pure]
        public static HashSet<EqT, T> except<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.Except(setB);

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public static HashSet<EqT, T> symmetricExcept<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.SymmetricExcept(setB);

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static HashSet<EqR, R> map<EqT, EqR, T, R>(HashSet<EqT, T> set, Func<T, R> mapper) 
            where EqT : struct, Eq<T>
            where EqR : struct, Eq<R>  =>
            set.Map<EqR, R>(mapper);

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static HashSet<EqT, T> map<EqT, T>(HashSet<EqT, T> set, Func<T, T> mapper) where EqT : struct, Eq<T> =>
            set.Map(mapper);

        /// <summary>
        /// Returns True if the value is in the set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if the item 'value' is in the Set 'set'</returns>
        [Pure]
        public static bool contains<EqT, T>(HashSet<EqT, T> set, T value) where EqT : struct, Eq<T> =>
            set.Contains(value);

        /// <summary>
        /// Removes an item from the set (if it exists)
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="value">Value to check</param>
        /// <returns>New set with item removed</returns>
        [Pure]
        public static HashSet<EqT, T> remove<EqT, T>(HashSet<EqT, T> set, T value) where EqT : struct, Eq<T> =>
            set.Remove(value);

        /// <summary>
        /// Returns True if setB is a subset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a subset of setA</returns>
        [Pure]
        public static bool isSubHSet<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.IsSubsetOf(setB);

        /// <summary>
        /// Returns True if setB is a superset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a superset of setA</returns>
        [Pure]
        public static bool isSuperHSet<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.IsSupersetOf(setB);

        /// <summary>
        /// Returns True if setB is a proper subset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a proper subset of setA</returns>
        [Pure]
        public static bool isProperSubHSet<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.IsProperSubsetOf(setB);

        /// <summary>
        /// Returns True if setB is a proper superset of setA
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True is setB is a proper subset of setA</returns>
        [Pure]
        public static bool isProperSuperHSet<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.IsProperSupersetOf(setB);

        /// <summary>
        /// Returns True if setA overlaps setB
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if setA overlaps setB</returns>
        [Pure]
        public static bool overlaps<EqT, T>(HashSet<EqT, T> setA, HashSet<EqT, T> setB) where EqT : struct, Eq<T> =>
            setA.Overlaps(setB);
    }
}
