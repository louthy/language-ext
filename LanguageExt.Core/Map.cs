using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Map
    {
        /// <summary>
        /// Clears all items from the map
        /// </summary>
        /// <param name="map">Map to clear</param>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        public static Map<K, V> clear<K, V>(Map<K, V> map) where K : IComparable<K> =>
            map.Clear();

        /// <summary>
        /// Creates a new empty Map
        /// </summary>
        public static Map<K, V> empty<K, V>() where K : IComparable<K> =>
            new Empty<K, V>();

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        public static Map<K, V> create<K, V>(params Tuple<K, V>[] keyValues) where K : IComparable<K> =>
            empty<K,V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        public static Map<K, V> createRange<K, V>(IEnumerable<Tuple<K, V>> keyValues) where K : IComparable<K> =>
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
        public static Map<K, V> add<K, V>(Map<K, V> map, K key, V value) where K : IComparable<K> =>
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
        public static Map<K, V> tryAdd<K, V>(Map<K, V> map, K key, V value) where K : IComparable<K> =>
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
        public static Map<K, V> addOrUpdate<K, V>(Map<K, V> map, K key, V value) where K : IComparable<K> =>
            map.AddOrUpdate(key, value);

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public static Map<K, V> addRange<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> keyValues) where K : IComparable<K> =>
            map.AddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public static Map<K, V> addRange<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) where K : IComparable<K> =>
            map.AddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public static Map<K, V> tryAddRange<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> keyValues) where K : IComparable<K> =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public static Map<K, V> tryAddRange<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) where K : IComparable<K> =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        public static Map<K, V> addOrUpdateRange<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> range) where K : IComparable<K> =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public static Map<K, V> addOrUpdateRange<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> range) where K : IComparable<K> =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        public static Map<K, V> remove<K, V>(Map<K, V> map, K key) where K : IComparable<K> =>
            map.Remove(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        public static bool containsKey<K, V>(Map<K, V> map, K key) where K : IComparable<K> =>
            map.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        public static bool contains<K, V>(Map<K, V> map, KeyValuePair<K, V> kv) where K : IComparable<K> =>
            map.Contains(tuple(kv.Key,kv.Value));

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        public static bool contains<K, V>(Map<K, V> map, Tuple<K, V> kv) where K : IComparable<K> =>
            map.Contains(kv);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        public static Map<K, V> setItem<K, V>(Map<K, V> map, K key, V value) where K : IComparable<K> =>
            map.SetItem(key, value);

        /// <summary>
        /// Atomically updates an existing item, unless it already exists, in which case 
        /// it is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        public static Map<K, V> trySetItem<K, V>(Map<K, V> map, K key, V value) where K : IComparable<K> =>
            map.TrySetItem(key, value);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        public static Option<V> find<K, V>(Map<K, V> map, K key) where K : IComparable<K> =>
            map.Find(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        public static R find<K, V, R>(Map<K, V> map, K key, Func<V, R> Some, Func<R> None) where K : IComparable<K> =>
            map.Find(key,Some,None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        public static Map<K,V> setItem<K, V>(Map<K, V> map, K key, Func<V, V> mapper) where K : IComparable<K> =>
            map.SetItem(key, mapper);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        public static Map<K, V> setItem<K, V>(Map<K, V> map, K key, Func<K, V, V> mapper) where K : IComparable<K> =>
            map.SetItem(key, mapper);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        public static Map<K, V> addOrUpdate<K, V>(Map<K, V> map, K key, Func<V, V> Some, Func<V> None) where K : IComparable<K> =>
            map.AddOrUpdate(key, Some, None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        public static Map<K, V> addOrUpdate<K, V>(Map<K, V> map, K key, Func<V, V> Some, V None) where K : IComparable<K> =>
            map.AddOrUpdate(key, Some, None);

        /// <summary>
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        public static IEnumerable<V> findRange<K, V>(Map<K, V> map, K keyFrom, K keyTo) where K : IComparable<K> =>
            map.FindRange(keyFrom, keyTo);

        /// <summary>
        /// Skips 'amount' values and returns a new tree without the 
        /// skipped values.
        /// </summary>
        /// <param name="amount">Amount to skip</param>
        /// <returns>New tree</returns>
        public static Map<K, V> skip<K, V>(Map<K, V> map, int amount) where K : IComparable<K> =>
            map.Skip(amount);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(Map<K, V> map, Action<V> action) where K : IComparable<K> =>
            map.Iter(action);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(Map<K, V> map, Action<K, V> action) where K : IComparable<K> =>
            map.Iter(action);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool forall<K, V>(Map<K, V> map, Func<V, bool> pred) where K : IComparable<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool forall<K, V>(Map<K, V> map, Func<K, bool> pred) where K : IComparable<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool forall<K, V>(Map<K, V> map, Func<K, V, bool> pred) where K : IComparable<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool forall<K, V>(Map<K, V> map, Func<Tuple<K, V>, bool> pred) where K : IComparable<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool forall<K, V>(Map<K, V> map, Func<KeyValuePair<K, V>, bool> pred) where K : IComparable<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        public static Map<K, U> map<K, T, U>(Map<K, T> map, Func<T, U> f) where K : IComparable<K> =>
            map.Select(f);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        public static Map<K, U> map<K, T, U>(Map<K, T> map, Func<K, T, U> f) where K : IComparable<K> =>
            map.Select(f);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        public static Map<K, V> filter<K, V>(Map<K, V> map, Func<V, bool> predicate) where K : IComparable<K> =>
            map.Filter(predicate);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        public static Map<K, V> filter<K, V>(Map<K, V> map, Func<K, bool> predicate) where K : IComparable<K> =>
            map.Filter(predicate);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        public static Map<K, V> filter<K, V>(Map<K, V> map, Func<K, V, bool> predicate) where K : IComparable<K> =>
            map.Filter(predicate);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        public static Map<K, T> choose<K, T>(Map<K, T> map, Func<T, Option<T>> selector) where K : IComparable<K> =>
            map.Choose(selector);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        public static Map<K, T> choose<K, T>(Map<K, T> map, Func<K, T, Option<T>> selector) where K : IComparable<K> =>
            map.Choose(selector);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        public static int length<K, T>(Map<K, T> map) where K : IComparable<K> =>
            map.Count;

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, K, V, S> folder) where K : IComparable<K> =>
            map.Fold(state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, K, S> folder) where K : IComparable<K> =>
            map.Fold(state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, V, S> folder) where K : IComparable<K> =>
            map.Fold(state, folder);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool exists<K, V>(Map<K, V> map, Func<K, V, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool exists<K, V>(Map<K, V> map, Func<Tuple<K, V>, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool exists<K, V>(Map<K, V> map, Func<KeyValuePair<K, V>, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool exists<K, V>(Map<K, V> map, Func<K, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public static bool exists<K, V>(Map<K, V> map, Func<V, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);
    }
}

public static class __MapExt
{
    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    public static Map<K, U> Map<K, V, U>(this Map<K, V> self, Func<V, U> mapper) where K : IComparable<K> =>
        self.Select(mapper);

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    public static Map<K, U> Map<K, V, U>(this Map<K, V> self, Func<K, V, U> mapper) where K : IComparable<K> =>
        self.Select(mapper);

    /// <summary>
    /// Number of items in the map
    /// </summary>
    public static int Count<K, V>(this Map<K, V> self) where K : IComparable<K> =>
        self.Count;
}