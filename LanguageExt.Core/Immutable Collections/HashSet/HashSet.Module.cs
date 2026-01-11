using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

/// <summary>
/// Immutable hash-set module
/// </summary>
public partial class HashSet
{
    /// <summary>
    /// True if the set has no elements
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <returns>True if the set has no elements</returns>
    [Pure]
    public static bool isEmpty<A>(HashSet<A> set) =>
        set.IsEmpty;

    /// <summary>
    /// Create a new empty set
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <returns>Empty HSet</returns>
    [Pure]
    public static HashSet<A> create<A>() =>
        HashSet<A>.Empty;

    /// <summary>
    /// Create a singleton collection
    /// </summary>
    /// <param name="value">Single value</param>
    /// <returns>Collection with a single item in it</returns>
    [Pure]
    public static HashSet<A> singleton<A>(A value) =>
        [value];

    /// <summary>
    /// Create a new set pre-populated with the items in range
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="range">Range of items</param>
    /// <returns>HSet</returns>
    [Pure]
    public static HashSet<A> createRange<A>(IEnumerable<A> range) =>
        new (range);

    /// <summary>
    /// Create a new set pre-populated with the items in range
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="range">Range of items</param>
    /// <returns>HSet</returns>
    [Pure]
    public static HashSet<A> createRange<A>(Iterator<A> range) =>
        new (range);

    /// <summary>
    /// Create a new set pre-populated with the items in range
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="range">Range of items</param>
    /// <returns>HSet</returns>
    [Pure]
    public static HashSet<A> createRange<A>(ReadOnlySpan<A> range) =>
        range.IsEmpty 
            ? HashSet<A>.Empty 
            : new (range);

    /// <summary>
    /// Create a new empty set
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <returns>Empty HSet</returns>
    [Pure]
    public static HashSet<A> empty<A>() =>
        HashSet<A>.Empty;

    /// <summary>
    /// Add an item to the set
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">Set to add item to</param>
    /// <param name="value">Value to add to the HSet</param>
    /// <returns>New set with the item added</returns>
    [Pure]
    public static HashSet<A> add<A>(HashSet<A> set, A value) =>
        set.Add(value);

    /// <summary>
    /// Attempt to add an item to the set.  If an item already
    /// exists then return the Set as-is.
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">Set to add item to</param>
    /// <param name="value">Value to add to the HSet</param>
    /// <returns>New set with the item maybe added</returns>
    [Pure]
    public static HashSet<A> tryAdd<A>(HashSet<A> set, A value) =>
        set.TryAdd(value);

    /// <summary>
    /// Add an item to the set.  If an item already
    /// exists then replace it.
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">Set to add item to</param>
    /// <param name="value">Value to add to the HSet</param>
    /// <returns>New set with the item maybe added</returns>
    [Pure]
    public static HashSet<A> addOrUpdate<A>(HashSet<A> set, A value) =>
        set.AddOrUpdate(value);

    /// <summary>
    /// Attempts to find an item in the set.  
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">HSet</param>
    /// <param name="value">Value to find</param>
    /// <returns>Some(T) if found, None otherwise</returns>
    [Pure]
    public static Option<A> find<A>(HashSet<A> set, A value) =>
        set.Find(value);

    /// <summary>
    /// Check the existence of an item in the set using a 
    /// predicate.
    /// </summary>
    /// <remarks>Note this scans the entire set.</remarks>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">HSet</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if predicate returns true for any item</returns>
    [Pure]
    public static bool exists<A>(HashSet<A> set, Func<A, bool> pred) =>
        set.Exists(pred);

    /// <summary>
    /// Returns true if both sets contain the same elements
    /// </summary>
    [Pure]
    public static bool equals<A>(HashSet<A> setA, HashSet<A> setB) =>
        setA.Equals(setB);

    /// <summary>
    /// Get the number of elements in the set
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">HSet</param>
    /// <returns>Number of elements</returns>
    [Pure]
    public static int length<A>(HashSet<A> set) =>
        set.Count();

    /// <summary>
    /// Returns setA - setB.  Only the items in setA that are not in 
    /// setB will be returned.
    /// </summary>
    [Pure]
    public static HashSet<A> subtract<A>(HashSet<A> setA, HashSet<A> setB) =>
        setA.Except(setB);

    /// <summary>
    /// Finds the union of two sets and produces a new set with 
    /// the results
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set A</param>
    /// <returns>A set which contains all items from both sets</returns>
    [Pure]
    public static HashSet<A> union<A>(HashSet<A> setA, HashSet<A> setB) =>
        setA.Union(setB);

    /// <summary>
    /// Filters items from the set using the predicate.  If the predicate
    /// returns True for any item then it remains in the set, otherwise
    /// it's dropped.
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">HSet</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Filtered enumerable</returns>
    [Pure]
    public static HashSet<A> filter<A>(HashSet<A> set, Func<A, bool> pred) =>
        set.Filter(pred);

    /// <summary>
    /// Returns the elements that are in both setA and setB
    /// </summary>
    [Pure]
    public static HashSet<A> intersect<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.Intersect(setB);

    /// <summary>
    /// Returns the elements that are in both setA and setB
    /// </summary>
    [Pure]
    public static HashSet<A> except<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.Except(setB);

    /// <summary>
    /// Only items that are in one set or the other will be returned.
    /// If an item is in both, it is dropped.
    /// </summary>
    [Pure]
    public static HashSet<A> symmetricExcept<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.SymmetricExcept(setB);

    /// <summary>
    /// Maps the values of this set into a new set of values using the
    /// mapper function to tranform the source values.
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <typeparam name="B">Mapped element type</typeparam>
    /// <param name="set">HSet</param>
    /// <param name="mapper">Mapping function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static HashSet<B> map<A, B>(HashSet<A> set, Func<A, B> mapper) =>
        set.Map(mapper);

    /// <summary>
    /// Returns True if the value is in the set
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">HSet</param>
    /// <param name="value">Value to check</param>
    /// <returns>True if the item 'value' is in the Set 'set'</returns>
    [Pure]
    public static bool contains<A>(HashSet<A> set, A value) =>
        set.Contains(value);

    /// <summary>
    /// Removes an item from the set (if it exists)
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="set">HSet</param>
    /// <param name="value">Value to check</param>
    /// <returns>New set with item removed</returns>
    [Pure]
    public static HashSet<A> remove<A>(HashSet<A> set, A value) =>
        set.Remove(value);

    /// <summary>
    /// Returns True if setB is a subset of setA
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set B</param>
    /// <returns>True is setB is a subset of setA</returns>
    [Pure]
    public static bool isSubHSet<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.IsSubsetOf(setB);

    /// <summary>
    /// Returns True if setB is a superset of setA
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set B</param>
    /// <returns>True is setB is a superset of setA</returns>
    [Pure]
    public static bool isSuperHSet<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.IsSupersetOf(setB);

    /// <summary>
    /// Returns True if setB is a proper subset of setA
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set B</param>
    /// <returns>True is setB is a proper subset of setA</returns>
    [Pure]
    public static bool isProperSubset<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.IsProperSubsetOf(setB);

    /// <summary>
    /// Returns True if setB is a proper superset of setA
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set B</param>
    /// <returns>True is setB is a proper subset of setA</returns>
    [Pure]
    public static bool isProperSuperset<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.IsProperSupersetOf(setB);

    /// <summary>
    /// Returns True if setA overlaps setB
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set B</param>
    /// <returns>True if setA overlaps setB</returns>
    [Pure]
    public static bool overlaps<A>(HashSet<A> setA, IEnumerable<A> setB) =>
        setA.Overlaps(setB);
}
