#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable tracking hash-map module
    /// </summary>
    public static partial class TrackingHashMap
    {
        /// <summary>
        /// Clears all items from the map
        /// </summary>
        /// <param name="map">Map to clear</param>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public static TrackingHashMap<K, V> clear<K, V>(TrackingHashMap<K, V> map) =>
            TrackingHashMap<K, V>.Empty;

        /// <summary>
        /// Creates a new empty TrackingHashMap 
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> empty<K, V>() =>
            TrackingHashMap<K, V>.Empty;

        /// <summary>
        /// Creates a new empty TrackingHashMap 
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> create<K, V>() =>
            TrackingHashMap<K, V>.Empty;

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> create<K, V>(Tuple<K, V> head, params Tuple<K, V>[] tail) =>
            empty<K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> create<K, V>((K, V) head, params (K, V)[] tail) =>
            empty<K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> create<K, V>(KeyValuePair<K, V> head, params KeyValuePair<K,V>[] tail) =>
            empty<K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> createRange<K, V>(IEnumerable<Tuple<K, V>> keyValues) =>
            empty<K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> createRange<K, V>(IEnumerable<(K, V)> keyValues) =>
            empty<K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static TrackingHashMap<K, V> createRange<K, V>(IEnumerable<KeyValuePair<K, V>> keyValues) =>
            empty<K, V>().AddRange(keyValues);

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
        public static TrackingHashMap<K, V> add<K, V>(TrackingHashMap<K, V> map, K key, V value) =>
            map.Add(key, value);

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
        public static TrackingHashMap<K, V> tryAdd<K, V>(TrackingHashMap<K, V> map, K key, V value) =>
            map.TryAdd(key, value);

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
        public static TrackingHashMap<K, V> addOrUpdate<K, V>(TrackingHashMap<K, V> map, K key, V value) =>
            map.AddOrUpdate(key, value);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public static TrackingHashMap<K, V> addOrUpdate<K, V>(TrackingHashMap<K, V> map, K key, Func<V, V> Some, Func<V> None) =>
            map.AddOrUpdate(key, Some, None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public static TrackingHashMap<K, V> addOrUpdate<K, V>(TrackingHashMap<K, V> map, K key, Func<V, V> Some, V None) =>
            map.AddOrUpdate(key, Some, None);

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> addRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
            map.AddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> addRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<(K, V)> keyValues) =>
            map.AddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> addRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) =>
            map.AddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> tryAddRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> tryAddRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<(K, V)> keyValues) =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> tryAddRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> addOrUpdateRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<Tuple<K, V>> range) =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> addOrUpdateRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<(K, V)> range) =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static TrackingHashMap<K, V> addOrUpdateRange<K, V>(TrackingHashMap<K, V> map, IEnumerable<KeyValuePair<K, V>> range) =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public static TrackingHashMap<K, V> remove<K, V>(TrackingHashMap<K, V> map, K key) =>
            map.Remove(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool containsKey<K, V>(TrackingHashMap<K, V> map, K key) =>
            map.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(TrackingHashMap<K, V> map, KeyValuePair<K, V> kv) =>
            map.Contains(kv.Key, kv.Value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(TrackingHashMap<K, V> map, Tuple<K, V> kv) =>
            map.Contains(kv.Item1, kv.Item2);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(TrackingHashMap<K, V> map, (K, V) kv) =>
            map.Contains(kv.Item1, kv.Item2);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public static TrackingHashMap<K, V> setItem<K, V>(TrackingHashMap<K, V> map, K key, V value) =>
            map.SetItem(key, value);

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
        public static TrackingHashMap<K, V> trySetItem<K, V>(TrackingHashMap<K, V> map, K key, V value) =>
            map.TrySetItem(key, value);

        /// <summary>
        /// Atomically sets an item by first retrieving it, applying a map (Some), and then putting 
        /// it back. Silently fails if the value doesn't exist.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <param name="Some">delegate to map the existing value to a new one before setting</param>
        /// <returns>New map with the item set</returns>
        [Pure]
        public static TrackingHashMap<K, V> trySetItem<K, V>(TrackingHashMap<K, V> map, K key, Func<V, V> Some) =>
            map.TrySetItem(key, Some);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static TrackingHashMap<K, V> setItems<K, V>(TrackingHashMap<K, V> map, IEnumerable<Tuple<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static TrackingHashMap<K, V> setItems<K, V>(TrackingHashMap<K, V> map, IEnumerable<(K, V)> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static TrackingHashMap<K, V> setItems<K, V>(TrackingHashMap<K, V> map, IEnumerable<KeyValuePair<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static TrackingHashMap<K, V> trySetItems<K, V>(TrackingHashMap<K, V> map, IEnumerable<Tuple<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static TrackingHashMap<K, V> trySetItems<K, V>(TrackingHashMap<K, V> map, IEnumerable<(K, V)> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static TrackingHashMap<K, V> trySetItems<K, V>(TrackingHashMap<K, V> map, IEnumerable<KeyValuePair<K, V>> items) =>
            map.TrySetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static TrackingHashMap<K, V> trySetItems<K, V>(TrackingHashMap<K, V> map, IEnumerable<K> keys, Func<V, V> Some) =>
            map.TrySetItems(keys, Some);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static Option<V> find<K, V>(TrackingHashMap<K, V> map, K key) =>
            map.Find(key);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static IEnumerable<V> findSeq<K, V>(TrackingHashMap<K, V> map, K key) =>
            map.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static R find<K, V, R>(TrackingHashMap<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            map.Find(key, Some, None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public static TrackingHashMap<K, V> setItem<K, V>(TrackingHashMap<K, V> map, K key, Func<V, V> mapper) =>
            map.SetItem(key, mapper);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(TrackingHashMap<K, V> map, Action<V> action) =>
            map.Iter(action);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(TrackingHashMap<K, V> map, Action<K, V> action) =>
            map.Iter(action);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(TrackingHashMap<K, V> map, Func<V, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(TrackingHashMap<K, V> map, Func<K, V, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(TrackingHashMap<K, V> map, Func<Tuple<K, V>, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(TrackingHashMap<K, V> map, Func<(K Key, V Value), bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(TrackingHashMap<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static TrackingHashMap<K, V> filter<K, V>(TrackingHashMap<K, V> map, Func<V, bool> predicate) =>
            map.Filter(predicate);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static TrackingHashMap<K, V> filter<K, V>(TrackingHashMap<K, V> map, Func<K, V, bool> predicate) =>
            map.Filter(predicate);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public static int length<K, T>(TrackingHashMap<K, T> map) =>
            map.Count;

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, K, V>(TrackingHashMap<K, V> map, S state, Func<S, K, V, S> folder) =>
            map.Fold(state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, K, V>(TrackingHashMap<K, V> map, S state, Func<S, V, S> folder) =>
            map.Fold(state, folder);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(TrackingHashMap<K, V> map, Func<K, V, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(TrackingHashMap<K, V> map, Func<Tuple<K, V>, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(TrackingHashMap<K, V> map, Func<(K Key, V Value), bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(TrackingHashMap<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(TrackingHashMap<K, V> map, Func<V, bool> pred) =>
            map.Exists(pred);
    }
}
