#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// <para>
    /// Versioned hash-map.  Each value has a vector-clock attached to it which understands the causality of updates
    /// from multiple actors.  Actors are just unique keys that represent individual contributors.  They could be client
    /// connections, nodes in a network, users, or whatever is appropriate to discriminate between commits.
    /// </para>
    /// <para>
    /// Deleted items are not removed from the hash-map, they are merely marked as deleted.  This allows conflicts between
    /// writes and deletes to be resolved.
    /// </para> 
    /// <para>
    /// Run `RemoveDeletedItemsOlderThan` to clean up items that have been deleted and are now just hanging around.  Use
    /// a big enough delay that it won't conflict with other commits (this could be seconds, minutes, or longer:
    /// depending on the expected latency of writes to the hash-map).
    /// </para>
    /// </summary>
    public class VersionHashMap<K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<VersionHashMap<K, V>>
    {
        readonly VersionHashMap<LastWriteWins<V>, TString, EqDefault<K>, string, K, V> Items;

        /// <summary>
        /// Empty version map
        /// </summary>
        public static VersionHashMap<K, V> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new VersionHashMap<K, V>(VersionHashMap<LastWriteWins<V>, TString, EqDefault<K>, string, K, V>.Empty);
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">Trie map</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        VersionHashMap(VersionHashMap<LastWriteWins<V>, TString, EqDefault<K>, string, K, V> items) =>
            this.Items = items;

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Version - this may be in a state of never existing, but won't ever fail</returns>
        [Pure]
        public Version<string, K, V> this[K key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.FindVersion(key);
        }

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.IsEmpty;
        }

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.Count;
        }

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.Count;
        }

        /// <summary>
        /// Atomically swap a key in the map.  Allows for multiple operations on the hash-map in an entirely
        /// transactional and atomic way.
        /// </summary>
        /// <param name="swap">Swap function, maps the current state of the value associated with the key to a new state</param>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit SwapKey(K key, Func<Version<string, K, V>, Version<string, K, V>> swap) =>
            Items.SwapKey(key, swap);

        /// <summary>
        /// Atomically updates a new item in the map.  If the key already exists, then the vector clocks, within the version
        /// vector, are compared to ascertain if the proposed version was caused-by, causes, or conflicts with the current
        /// version:
        ///
        ///     * If the proposed version was caused-by the current version, then it is ignored.
        ///     * If the proposed version causes the current version, then it is accepted and updated as the latest version
        ///     * If the proposed version is in conflict with the current version, then values from both versions are
        ///       passed to ConflictV.Append(v1, v2) for value resolution, and the pairwise-maximum of the two vector-clocks
        ///       is taken to be the new 'time.
        /// 
        /// </summary>
        /// <param name="version">Version to update</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Update(Version<string, K, V> version) =>
            Items.Update(version);

        /// <summary>
        /// Remove items that are older than the specified time-stamp
        /// </summary>
        /// <param name="cutoff">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveOlderThan(DateTime cutoff) =>
            Items.RemoveOlderThan(cutoff);

        /// <summary>
        /// Remove items that are older than the specified time-stamp
        /// </summary>
        /// <param name="timeStamp">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveOlderThan(long timeStamp) =>
            Items.RemoveOlderThan(timeStamp);

        /// <summary>
        /// Remove deleted items that are older than the specified time-stamp
        /// </summary>
        /// <param name="cutoff">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveDeletedItemsOlderThan(DateTime cutoff) =>
            Items.RemoveDeletedItemsOlderThan(cutoff);

        /// <summary>
        /// Remove deleted items that are older than the specified time-stamp
        /// </summary>
        /// <param name="timeStamp">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveDeletedItemsOlderThan(long timeStamp) =>
            Items.RemoveDeletedItemsOlderThan(timeStamp);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<V> Find(K value) =>
            Items.Find(value);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Version<string, K, V> FindVersion(K value) =>
            Items.FindVersion(value);

        /// <summary>
        /// Enumerable of keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.Keys;
        }

        /// <summary>
        /// Enumerable of value
        /// </summary>
        [Pure]
        public IEnumerable<V> Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.Values;
        }

        /// <summary>
        /// Convert to a HashMap
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashMap<K, V> ToHashMap() =>
            Items.ToHashMap();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            Items.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            Items.GetEnumerator();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<(K Key, V Value)> ToSeq() =>
            Items.ToSeq();

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// The ellipsis is used for collections over 50 items
        /// To get a formatted string with all the items, use `ToFullString`
        /// or `ToFullArrayString`.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() =>
            Items.ToString();

        /// <summary>
        /// Format the collection as `(key: value), (key: value), (key: value), ...`
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToFullString(string separator = ", ") =>
            Items.ToFullString(separator);

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToFullArrayString(string separator = ", ") =>
            Items.ToFullArrayString(separator);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(K Key, V Value)> AsEnumerable() =>
            Items.AsEnumerable();

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSubsetOf(IEnumerable<K> other) =>
            Items.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSupersetOf(IEnumerable<K> other) =>
            Items.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubsetOf(IEnumerable<K> other) =>
            Items.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubsetOf(HashMap<K, V> other) =>
            Items.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSupersetOf(IEnumerable<K> rhs) =>
            Items.IsSupersetOf(rhs);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Intersect(IEnumerable<K> rhs) =>
            Items.Intersect(rhs);
        
        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(IEnumerable<(K Key, V Value)> other) =>
            Items.Overlaps(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(IEnumerable<K> other) =>
            Items.Overlaps(other);

        /// <summary>
        /// Returns this - rhs.  Only the items in this that are not in 
        /// rhs will be returned.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Except(IEnumerable<K> rhs) =>
            Items.Except(rhs);
        
        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) =>
            obj is VersionHashMap<K, V> hm && Equals(hm);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(VersionHashMap<K, V>? other) =>
            other is not null && Items.Equals(other.Items);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            Items.GetHashCode();

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<K, V> Where(Func<long, Option<V>, bool> pred) =>
            new VersionHashMap<K, V>(Items.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<K, V> Where(Func<K, long, Option<V>, bool> pred) =>
            new VersionHashMap<K, V>(Items.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<K, V> Filter(Func<long, Option<V>, bool> pred) =>
            new VersionHashMap<K, V>(Items.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<K, V> Filter(Func<K, long, Option<V>, bool> pred) =>
            new VersionHashMap<K, V>(Items.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit FilterInPlace(Func<long, Option<V>, bool> pred) =>
            Items.FilterInPlace(pred);
        
        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit FilterInPlace(Func<K, long, Option<V>, bool> pred) =>
            Items.FilterInPlace(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<K, V, bool> pred) =>
            Items.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<(K Key, V Value), bool> pred) =>
            Items.ForAll(pred);

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<KeyValuePair<K, V>, bool> pred) =>
            Items.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<V, bool> pred) =>
            Items.ForAll(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(Func<K, V, bool> pred) =>
            Items.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(Func<(K Key, V Value), bool> pred) =>
            Items.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(Func<V, bool> pred) =>
            Items.Exists(pred);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Iter(Action<K, V> action) =>
            Items.Iter(action);

        /// <summary>
        /// Atomically iterate through all values in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Iter(Action<V> action) =>
            Items.Iter(action);

        /// <summary>
        /// Atomically iterate through all key/value pairs (as tuples) in the map (in order) 
        /// and execute an action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Iter(Action<(K Key, V Value)> action) =>
            Items.Iter(action);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Iter(Action<KeyValuePair<K, V>> action) =>
            Items.Iter(action);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S Fold<S>(S state, Func<S, K, V, S> folder) =>
            Items.Fold(state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S Fold<S>(S state, Func<S, V, S> folder) =>
            Items.Fold(state, folder);
    }
}
#nullable disable
