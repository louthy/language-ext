#nullable enable
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Unsorted immutable hash-map that tracks changes.
    /// </summary>
    /// <remarks>
    /// Changes are accessible via the `Changes` property.  It is a `HashMap` of `Change` values from either the initial
    /// empty state of the collection, or since the last call to `Snapshot()`.
    ///
    /// The fact that the changes are represented as a single-value `HashMap` shows that the tracked changes are not an
    /// ever increasing log of changes, but instead a morphism between one previous state of the `TrackingHashMap` and
    /// another.  Therefore there's at most one morphism for each key, and potentially none.
    ///
    /// The morphisms are:
    ///
    ///     * `EntryAdded`
    ///     * `EntryMapped`
    ///     * `EntryRemoved`
    ///
    /// A new 'zero-changes starting-state' can be created by calling `Snapshot()`.  `Snapshot` creates the first
    /// snapshot (effectively clears the `Changes` to zero), and `Changes` will collect the difference from this point
    /// to any morphed future-state as collection-transforming operations are performed
    /// </remarks>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public readonly struct TrackingHashMap<EqK, K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<TrackingHashMap<EqK, K, V>>
        where EqK : struct, Eq<K>
    {
        public static readonly TrackingHashMap<EqK, K, V> Empty = new TrackingHashMap<EqK, K,V>(TrieMap<EqK, K, V>.Empty);

        readonly TrieMap<EqK, K, V> value;
        readonly TrieMap<EqK, K, Change<V>> changes;

        internal TrieMap<EqK, K, V> Value => 
            value ?? TrieMap<EqK, K, V>.Empty;

        internal TrieMap<EqK, K, Change<V>> ChangesInternal => 
            changes ?? TrieMap<EqK, K, Change<V>>.Empty;

        public HashMap<EqK, K, Change<V>> Changes => 
            new HashMap<EqK, K, Change<V>>(ChangesInternal);

        internal TrackingHashMap(TrieMap<EqK, K, V> value, TrieMap<EqK, K, Change<V>> changes)
        {
            this.value = value;
            this.changes = changes;
        }

        public TrackingHashMap(IEnumerable<(K Key, V Value)> items) 
            : this(items, true)
        { }

        public TrackingHashMap(IEnumerable<(K Key, V Value)> items, bool tryAdd)
        {
            this.value = new TrieMap<EqK, K, V>(items, tryAdd);
            this.changes = TrieMap<EqK, K, Change<V>>.Empty;
        }

        /// <summary>
        /// Creates a 'zero change' snapshot.  *The data does not change*!   
        /// </summary>
        /// <remarks>Useful for creating new starting points for capturing the difference between two snapshots of the
        /// `TrackingHashMap`.  `Snapshot` creates the first snapshot (effectively clears the `Changes` to zero), and
        /// `Changes` will collect the difference from this point to any morphed future point as collection-
        /// transforming operations are performed</remarks>
        /// <returns>Map with changes zeroed</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> Snapshot() =>
            new (Value, TrieMap<EqK, K, Change<V>>.Empty);

        /// <summary>
        /// Item at index lens
        /// </summary>
        [Pure]
        public static Lens<TrackingHashMap<EqK, K, V>, V> item(K key) => Lens<TrackingHashMap<EqK, K, V>, V>.New(
            Get: la => la[key],
            Set: a => la => la.AddOrUpdate(key, a)
            );

        /// <summary>
        /// Item or none at index lens
        /// </summary>
        [Pure]
        public static Lens<TrackingHashMap<EqK, K, V>, Option<V>> itemOrNone(K key) => Lens<TrackingHashMap<EqK, K, V>, Option<V>>.New(
            Get: la => la.Find(key),
            Set: a => la => a.Match(Some: x => la.AddOrUpdate(key, x), None: () => la.Remove(key))
            );

        TrackingHashMap<EqK, K, V> Wrap((TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) pair) =>
            new (pair.Map, ChangesInternal.Merge<MChange<V>>(pair.Changes));

        TrackingHashMap<EqK, K, V> Wrap(K key, (TrieMap<EqK, K, V> Map, Change<V> Change) pair) =>
            new (pair.Map, ChangesInternal.AddOrUpdate(key, Some: ex => default(MChange<V>).Append(ex, pair.Change), pair.Change));

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public V this[K key] =>
            Value[key];

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.IsEmpty ?? true;
        }

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.Count ?? 0;
        }

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.Count ?? 0;
        }

        /// <summary>
        /// Get a IReadOnlyDictionary for this map.  No mapping is required, so this is very fast.
        /// </summary>
        [Pure]
        public IReadOnlyDictionary<K, V> ToReadOnlyDictionary() =>
            Value;

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> Filter(Func<V, bool> pred) =>
            Wrap(Value.FilterWithLog(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> Filter(Func<K, V, bool> pred) =>
            Wrap(Value.FilterWithLog(pred));

        /// <summary>
        /// Atomically adds a new item to the map
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the key already exists</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> Add(K key, V value) =>
            Wrap(key, Value.AddWithLog(key, value));

        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists, then the new item is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TryAdd(K key, V value) =>
            Wrap(key, Value.TryAddWithLog(key, value));

        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists, the new item replaces it.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddOrUpdate(K key, V value) =>
            Wrap(key, Value.AddOrUpdateWithLog(key, value));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) =>
            Wrap(key, Value.AddOrUpdateWithLog(key, Some, None));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, V None) =>
            Wrap(key, Value.AddOrUpdateWithLog(key, Some, None));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.AddRangeWithLog(range));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddRange(IEnumerable<(K Key, V Value)> range) =>
            Wrap(Value.AddRangeWithLog(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TryAddRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.TryAddRangeWithLog(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TryAddRange(IEnumerable<(K Key, V Value)> range) =>
            Wrap(Value.TryAddRangeWithLog(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range) =>
            Wrap(Value.TryAddRangeWithLog(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.AddOrUpdateRangeWithLog(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddOrUpdateRange(IEnumerable<(K Key, V Value)> range) =>
            Wrap(Value.AddOrUpdateRangeWithLog(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range) =>
            Wrap(Value.AddOrUpdateRangeWithLog(range));

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> Remove(K key) =>
            Wrap(key, Value.RemoveWithLog(key));

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<V> Find(K key) =>
            Value.Find(key);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public IEnumerable<V> FindSeq(K key) =>
            Value.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(K key, Func<V, R> Some, Func<R> None) =>
            Value.Find(key, Some, None);

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (TrackingHashMap<EqK, K, V> Map, V Value) FindOrAdd(K key, Func<V> None)
        {
            var (x, y, cs) = Value.FindOrAddWithLog(key, None);
            return (Wrap(key, (x, cs)), y);
        }

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="value">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (TrackingHashMap<EqK, K, V>, V Value) FindOrAdd(K key, V value)
        {
            var (x, y, cs) = Value.FindOrAddWithLog(key, value);
            return (Wrap(key, (x, cs)), y);
        }

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (TrackingHashMap<EqK, K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Func<Option<V>> None)
        {
            var (x, y, cs) = Value.FindOrMaybeAddWithLog(key, None);
            return (Wrap(key, (x, cs)), y);
        }

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (TrackingHashMap<EqK, K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Option<V> None)
        {
            var (x, y, cs) = Value.FindOrMaybeAddWithLog(key, None);
            return (Wrap(key, (x, cs)), y);
        }

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> SetItem(K key, V value) =>
            Wrap(key, Value.SetItemWithLog(key, value));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the item isn't found</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> SetItem(K key, Func<V, V> Some) =>
            Wrap(key, Value.SetItemWithLog(key, Some));

        /// <summary>
        /// Atomically updates an existing item, unless it doesn't exist, in which case 
        /// it is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the value is null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TrySetItem(K key, V value) =>
            Wrap(key, Value.TrySetItemWithLog(key, value));

        /// <summary>
        /// Atomically sets an item by first retrieving it, applying a map, and then putting it back.
        /// Silently fails if the value doesn't exist
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <param name="Some">delegate to map the existing value to a new one before setting</param>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New map with the item set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TrySetItem(K key, Func<V, V> Some) =>
            Wrap(key, Value.TrySetItemWithLog(key, Some));

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool ContainsKey(K key) =>
            Value.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains(K key, V value) =>
            Value.Contains(key, value);

        /// <summary>
        /// Checks for existence of a value in the map
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if an item with the value supplied is in the map</returns>
        [Pure]
        public bool Contains(V value) =>
            Value.Contains(value);

        /// <summary>
        /// Checks for existence of a value in the map
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if an item with the value supplied is in the map</returns>
        [Pure]
        public bool Contains<EqV>(V value) where EqV : struct, Eq<V> =>
            Value.Contains<EqV>(value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains<EqV>(K key, V value) where EqV : struct, Eq<V> =>
            Value.Contains<EqV>(key, value);

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> Clear() =>
            Wrap(Value.ClearWithLog());

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            Wrap(Value.AddRangeWithLog(pairs));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items) =>
            Wrap(Value.SetItemsWithLog(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> SetItems(IEnumerable<Tuple<K, V>> items) =>
            Wrap(Value.SetItemsWithLog(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> SetItems(IEnumerable<(K Key, V Value)> items) =>
            Wrap(Value.SetItemsWithLog(items));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items) =>
            Wrap(Value.TrySetItemsWithLog(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TrySetItems(IEnumerable<Tuple<K, V>> items) =>
            Wrap(Value.TrySetItemsWithLog(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TrySetItems(IEnumerable<(K Key, V Value)> items) =>
            Wrap(Value.TrySetItemsWithLog(items));

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some) =>
            Wrap(Value.TrySetItemsWithLog(keys, Some));

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New map with the items removed</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> RemoveRange(IEnumerable<K> keys) =>
            Wrap(Value.RemoveRangeWithLog(keys));

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) =>
            Value.Contains(pair.Key, pair.Value);

        /// <summary>
        /// Enumerable of map keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys =>
            Value.Keys;

        /// <summary>
        /// Enumerable of map values
        /// </summary>
        [Pure]
        public IEnumerable<V> Values =>
            Value.Values;

        /// <summary>
        /// Convert the map to an IDictionary
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IReadOnlyDictionary<K, V> ToDictionary() =>
            Value;

        /// <summary>
        /// Map the map the a dictionary
        /// </summary>
        [Pure]
        public IDictionary<KR, VR> ToDictionary<KR, VR>(
            Func<(K Key, V Value), KR> keySelector, 
            Func<(K Key, V Value), VR> valueSelector) 
            where KR : notnull =>
            AsEnumerable().ToDictionary(x => keySelector(x), x => valueSelector(x));

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        [Pure]
        public Seq<(K Key, V Value)> ToSeq() =>
            toSeq(AsEnumerable());

        /// <summary>
        /// Allocation free conversion to a HashMap
        /// </summary>
        [Pure]
        public HashMap<EqK, K, V> ToHashMap() =>
            new (value);

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// The elipsis is used for collections over 50 items
        /// To get a formatted string with all the items, use `ToFullString`
        /// or `ToFullArrayString`.
        /// </summary>
        [Pure]
        public override string ToString() =>
            CollectionFormat.ToShortArrayString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), Count);

        /// <summary>
        /// Format the collection as `(key: value), (key: value), (key: value), ...`
        /// </summary>
        [Pure]
        public string ToFullString(string separator = ", ") =>
            CollectionFormat.ToFullString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), separator);

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// </summary>
        [Pure]
        public string ToFullArrayString(string separator = ", ") =>
            CollectionFormat.ToFullArrayString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), separator);

        [Pure]
        public IEnumerable<(K Key, V Value)> AsEnumerable() =>
            Value;

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TrackingHashMap<EqK, K, V>(SeqEmpty _) =>
            Empty;

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator ==(TrackingHashMap<EqK, K, V> lhs, TrackingHashMap<EqK, K, V> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// In-equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator !=(TrackingHashMap<EqK, K, V> lhs, TrackingHashMap<EqK, K, V> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static TrackingHashMap<EqK, K, V> operator +(TrackingHashMap<EqK, K, V> lhs, TrackingHashMap<EqK, K, V> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public TrackingHashMap<EqK, K, V> Append(TrackingHashMap<EqK, K, V> rhs) =>
            Wrap(Value.AppendWithLog(rhs.Value));

        [Pure]
        public static TrackingHashMap<EqK, K, V> operator -(TrackingHashMap<EqK, K, V> lhs, TrackingHashMap<EqK, K, V> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public TrackingHashMap<EqK, K, V> Subtract(TrackingHashMap<EqK, K, V> rhs) =>
            Wrap(Value.SubtractWithLog(rhs.Value));

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<K> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<K> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<K> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(TrackingHashMap<EqK, K, V> other) =>
            Value.IsSubsetOf(other.Value);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<K> rhs) =>
            Value.IsSupersetOf(rhs);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public TrackingHashMap<EqK, K, V> Intersect(IEnumerable<K> rhs) =>
            Wrap(Value.IntersectWithLog(rhs));

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public TrackingHashMap<EqK, K, V> Intersect(IEnumerable<(K Key, V Value)> rhs) =>
            Wrap(Value.IntersectWithLog(rhs));

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrackingHashMap<EqK, K, V> Intersect(IEnumerable<(K Key, V Value)> rhs, WhenMatched<K, V, V, V> Merge) =>
            Wrap(Value.IntersectWithLog(new TrieMap<EqK, K, V>(rhs), Merge));

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrackingHashMap<EqK, K, V> Intersect(HashMap<EqK, K, V> rhs, WhenMatched<K, V, V, V> Merge) =>
            Wrap(Value.IntersectWithLog(rhs.Value, Merge));

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrackingHashMap<EqK, K, V> Intersect(TrackingHashMap<EqK, K, V> rhs, WhenMatched<K, V, V, V> Merge) =>
            Wrap(Value.IntersectWithLog(rhs.Value, Merge));

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        public bool Overlaps(IEnumerable<(K Key, V Value)> other) =>
            Value.Overlaps(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        public bool Overlaps(IEnumerable<K> other) =>
            Value.Overlaps(other);

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public TrackingHashMap<EqK, K, V> Except(IEnumerable<K> rhs) =>
            Wrap(Value.ExceptWithLog(rhs));

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public TrackingHashMap<EqK, K, V> Except(IEnumerable<(K Key, V Value)> rhs) =>
            Wrap(Value.ExceptWithLog(rhs));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public TrackingHashMap<EqK, K, V> SymmetricExcept(TrackingHashMap<EqK, K, V> rhs) =>
            Wrap(Value.SymmetricExceptWithLog(rhs.Value));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public TrackingHashMap<EqK, K, V> SymmetricExcept(IEnumerable<(K Key, V Value)> rhs) =>
            Wrap(Value.SymmetricExceptWithLog(rhs));

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public TrackingHashMap<EqK, K, V> Union(IEnumerable<(K, V)> rhs) =>
            this.TryAddRange(rhs);

        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrackingHashMap<EqK, K, V> Union(IEnumerable<(K Key, V Value)> other, WhenMatched<K, V, V, V> Merge) =>
            Wrap(Value.UnionWithLog(other, static (_, v) => v, static (_, v) => v, Merge));

        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing` function is called when there is a key in the right-hand side, but not the left-hand-side.
        /// This allows the `V2` value-type to be mapped to the target `V` value-type. 
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrackingHashMap<EqK, K, V> Union<W>(IEnumerable<(K Key, W Value)> other, WhenMissing<K, W, V> MapRight, WhenMatched<K, V, W, V> Merge) =>
            Wrap(Value.UnionWithLog(other, static (_, v) => v, MapRight, Merge));
        
        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public override bool Equals(object? obj) =>
            obj is TrackingHashMap<EqK, K, V> hm && Equals(hm);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public bool Equals(TrackingHashMap<EqK, K, V> other) =>
            Value.Equals<EqDefault<V>>(other.Value);

        /// <summary>
        /// Equality of keys and values with `EqV` used for values
        /// </summary>
        [Pure]
        public bool Equals<EqV>(TrackingHashMap<EqK, K, V> other) where EqV : struct, Eq<V> =>
            Value.Equals<EqV>(other.Value);

        /// <summary>
        /// Equality of keys only
        /// </summary>
        [Pure]
        public bool EqualsKeys(TrackingHashMap<EqK, K, V> other) =>
            Value.Equals<EqTrue<V>>(other.Value);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public TrackingHashMap<EqK, K, V> Do(Action<V> f)
        {
            this.Iter(f);
            return this;
        }

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TrackingHashMap<EqK, K, V> Where(Func<V, bool> pred) =>
            Filter(pred);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TrackingHashMap<EqK, K, V> Where(Func<K, V, bool> pred) =>
            Filter(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<K, V, bool> pred)
        {
            foreach (var item in AsEnumerable())
            {
                if (!pred(item.Key, item.Value)) return false;
            }
            return true;
        }

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<Tuple<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<(K Key, V Value), bool> pred) =>
            AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<KeyValuePair<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<V, bool> pred) =>
            Values.ForAll(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool Exists(Func<K, V, bool> pred)
        {
            foreach (var item in AsEnumerable())
            {
                if (pred(item.Key, item.Value)) return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<Tuple<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<(K Key, V Value), bool> pred) =>
            AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<KeyValuePair<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<V, bool> pred) =>
            Values.Exists(pred);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<K, V> action)
        {
            foreach (var item in this)
            {
                action(item.Key, item.Value);
            }
            return unit;
        }

        /// <summary>
        /// Atomically iterate through all values in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<V> action)
        {
            foreach (var item in this)
            {
                action(item.Value);
            }
            return unit;
        }

        /// <summary>
        /// Atomically iterate through all key/value pairs (as tuples) in the map (in order) 
        /// and execute an action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<Tuple<K, V>> action)
        {
            foreach (var item in this)
            {
                action(new Tuple<K, V>(item.Key, item.Value));
            }
            return unit;
        }

        /// <summary>
        /// Atomically iterate through all key/value pairs (as tuples) in the map (in order) 
        /// and execute an action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<(K Key, V Value)> action)
        {
            foreach (var item in this)
            {
                action(item);
            }
            return unit;
        }

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<KeyValuePair<K, V>> action)
        {
            foreach (var item in this)
            {
                action(new KeyValuePair<K, V>(item.Key, item.Value));
            }
            return unit;
        }

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, K, V, S> folder) =>
            AsEnumerable().Fold(state, (s, x) => folder(s, x.Key, x.Value));

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, V, S> folder) =>
            Values.Fold(state, folder);

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(ValueTuple<(K, V)> items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14, items.Item15 });

        [Pure]
        public static implicit operator TrackingHashMap<EqK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new TrackingHashMap<EqK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14, items.Item15, items.Item16 });
    }
}
