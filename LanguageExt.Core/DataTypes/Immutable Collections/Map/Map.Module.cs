using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable map module
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// [wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
    /// </summary>
    public static partial class Map
    {
        /// <summary>
        /// Clears all items from the map
        /// </summary>
        /// <param name="map">Map to clear</param>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public static Map<K, V> clear<K, V>(Map<K, V> map) =>
            Map<K, V>.Empty;

        /// <summary>
        /// Creates a new empty Map
        /// </summary>
        [Pure]
        public static Map<K, V> empty<K, V>() =>
            Map<K, V>.Empty;

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<K, V> create<K, V>() =>
            Map<K, V>.Empty;

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<K, V> create<K, V>(Tuple<K, V> head, params Tuple<K, V>[] tail) =>
            empty<K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<K, V> create<K, V>(KeyValuePair<K, V> head, params KeyValuePair<K, V>[] tail) =>
            empty<K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<K, V> create<K, V>((K, V) head, params (K, V)[] tail) =>
            empty<K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<K, V> createRange<K, V>(IEnumerable<Tuple<K, V>> keyValues) =>
            empty<K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<K, V> createRange<K, V>(IEnumerable<(K, V)> keyValues) =>
            empty<K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<K, V> createRange<K, V>(IEnumerable<KeyValuePair<K, V>> keyValues) =>
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
        public static Map<K, V> add<K, V>(Map<K, V> map, K key, V value) =>
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
        public static Map<K, V> tryAdd<K, V>(Map<K, V> map, K key, V value) =>
            map.TryAdd(key, value);

        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists then the Fail handler is called with the unaltered map 
        /// and the value already set for the key, it expects a new map returned.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="Fail">Delegate to handle failure, you're given the unaltered map 
        /// and the value already set for the key</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public static Map<K, V> tryAdd<K, V>(Map<K, V> map, K key, V value, Func<Map<K, V>, V, Map<K, V>> Fail) =>
            map.TryAdd(key, value, Fail);

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
        public static Map<K, V> addOrUpdate<K, V>(Map<K, V> map, K key, V value) =>
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
        public static Map<K, V> addOrUpdate<K, V>(Map<K, V> map, K key, Func<V, V> Some, Func<V> None) =>
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
        public static Map<K, V> addOrUpdate<K, V>(Map<K, V> map, K key, Func<V, V> Some, V None) =>
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
        public static Map<K, V> addRange<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
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
        public static Map<K, V> addRange<K, V>(Map<K, V> map, IEnumerable<(K, V)> keyValues) =>
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
        public static Map<K, V> addRange<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) =>
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
        public static Map<K, V> tryAddRange<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
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
        public static Map<K, V> tryAddRange<K, V>(Map<K, V> map, IEnumerable<(K, V)> keyValues) =>
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
        public static Map<K, V> tryAddRange<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static Map<K, V> addOrUpdateRange<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> range) =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static Map<K, V> addOrUpdateRange<K, V>(Map<K, V> map, IEnumerable<(K, V)> range) =>
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
        public static Map<K, V> addOrUpdateRange<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> range) =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public static Map<K, V> remove<K, V>(Map<K, V> map, K key) =>
            map.Remove(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool containsKey<K, V>(Map<K, V> map, K key) =>
            map.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(Map<K, V> map, KeyValuePair<K, V> kv) =>
            map.Contains(kv.Key, kv.Value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(Map<K, V> map, Tuple<K, V> kv) =>
            map.Contains(kv.Item1, kv.Item2);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(Map<K, V> map, (K, V) kv) =>
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
        public static Map<K, V> setItem<K, V>(Map<K, V> map, K key, V value) =>
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
        public static Map<K, V> trySetItem<K, V>(Map<K, V> map, K key, V value) =>
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
        public static Map<K, V> trySetItem<K, V>(Map<K, V> map, K key, Func<V, V> Some) =>
            map.TrySetItem(key, Some);

        /// <summary>
        /// Atomically sets an item by first retrieving it, applying a map, and then putting it back.
        /// Calls the None delegate to return a new map if the item can't be found
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="Some">delegate to map the existing value to a new one before setting</param>
        /// <param name="None">delegate to return a new map if the item can't be found</param>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <returns>New map with the item set</returns>
        [Pure]
        public static Map<K, V> trySetItem<K, V>(Map<K, V> map, K key, Func<V, V> Some, Func<Map<K, V>, Map<K, V>> None) =>
            map.TrySetItem(key, Some, None);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<K, V> setItems<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<K, V> setItems<K, V>(Map<K, V> map, IEnumerable<(K, V)> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<K, V> setItems<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<K, V> trySetItems<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<K, V> trySetItems<K, V>(Map<K, V> map, IEnumerable<(K, V)> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<K, V> trySetItems<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> items) =>
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
        public static Map<K, V> trySetItems<K, V>(Map<K, V> map, IEnumerable<K> keys, Func<V, V> Some) =>
            map.TrySetItems(keys, Some);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static Option<V> find<K, V>(Map<K, V> map, K key) =>
            map.Find(key);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static IEnumerable<V> findSeq<K, V>(Map<K, V> map, K key) =>
            map.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static R find<K, V, R>(Map<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            map.Find(key, Some, None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public static Map<K, V> setItem<K, V>(Map<K, V> map, K key, Func<V, V> mapper) =>
            map.SetItem(key, mapper);

        /// <summary>
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        [Pure]
        public static IEnumerable<V> findRange<K, V>(Map<K, V> map, K keyFrom, K keyTo) =>
            map.FindRange(keyFrom, keyTo);

        /// <summary>
        /// Skips 'amount' values and returns a new tree without the 
        /// skipped values.
        /// </summary>
        /// <param name="amount">Amount to skip</param>
        /// <returns>Enumerable of map items</returns>
        [Pure]
        public static IEnumerable<(K Key, V Value)> skip<K, V>(Map<K, V> map, int amount) =>
            map.Skip(amount);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(Map<K, V> map, Action<V> action) =>
            map.Iter(action);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(Map<K, V> map, Action<K, V> action) =>
            map.Iter(action);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(Map<K, V> map, Func<V, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(Map<K, V> map, Func<K, V, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(Map<K, V> map, Func<Tuple<K, V>, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(Map<K, V> map, Func<(K Key, V Value), bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(Map<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<K, U> map<K, T, U>(Map<K, T> map, Func<T, U> f) =>
            (Map<K, U>)map.Select(f);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<K, U> map<K, T, U>(Map<K, T> map, Func<K, T, U> f) =>
            map.Select(f);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<K, V> filter<K, V>(Map<K, V> map, Func<V, bool> predicate) =>
            map.Filter(predicate);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<K, V> filter<K, V>(Map<K, V> map, Func<K, V, bool> predicate) =>
            map.Filter(predicate);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public static Map<K, R> choose<K, T, R>(Map<K, T> map, Func<T, Option<R>> selector) =>
            map.Choose(selector);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public static Map<K, R> choose<K, T, R>(Map<K, T> map, Func<K, T, Option<R>> selector) =>
            map.Choose(selector);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public static int length<K, T>(Map<K, T> map) =>
            map.Count;

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, K, V, S> folder) =>
            map.Fold(state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, V, S> folder) =>
            map.Fold(state, folder);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(Map<K, V> map, Func<K, V, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(Map<K, V> map, Func<Tuple<K, V>, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(Map<K, V> map, Func<(K Key, V Value), bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(Map<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(Map<K, V> map, Func<V, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Convert any IDictionary into an immutable Map K V
        /// </summary>
        [Pure]
        public static Map<K, V> freeze<K, V>(IDictionary<K, V> dict) =>
            toMap(dict.AsEnumerable());

        /// <summary>
        /// Convert any IDictionary into an immutable Map K V
        /// </summary>
        [Pure]
        public static Map<K, V> Freeze<K, V>(this IDictionary<K, V> dict) =>
            toMap(dict.AsEnumerable());

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<K, V> union<K, V>(Map<K, V> left, Map<K, V> right, WhenMatched<K, V, V, V> Merge) =>
            left.Union(right, (k, v) => v, (k, v) => v, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<K, A> union<K, A, B>(Map<K, A> left, Map<K, B> right, WhenMissing<K, B, A> MapRight, WhenMatched<K, A, B, A> Merge) =>
            left.Union(right, (k, v) => v, MapRight, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<K, B> union<K, A, B>(Map<K, A> left, Map<K, B> right, WhenMissing<K, A, B> MapLeft, WhenMatched<K, A, B, B> Merge) =>
            left.Union(right, MapLeft, (k, v) => v, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<K, C> union<K, A, B, C>(Map<K, A> left, Map<K, B> right, WhenMissing<K, A, C> MapLeft, WhenMissing<K, B, C> MapRight, WhenMatched<K, A, B, C> Merge) =>
            left.Union(right, MapLeft, MapRight, Merge);

        /// <summary>
        /// Intersect two maps.  Only keys that are in both maps are
        /// returned.  The merge function is called for every resulting
        /// key.
        [Pure]
        public static Map<K, R> intersect<K, A, B, R>(Map<K, A> left, Map<K, B> right, WhenMatched<K, A, B, R> merge) =>
            left.Intersect(right, merge);

        /// <summary>
        /// Map differencing based on key.  this - other.
        /// </summary>
        [Pure]
        public static Map<K, V> except<K, V>(Map<K, V> left, Map<K, V> right) =>
            left.Except(right);

        /// <summary>
        /// Keys that are in both maps are dropped and the remaining
        /// items are merged and returned.
        /// </summary>
        [Pure]
        public static Map<K, V> symmetricExcept<K, V>(Map<K, V> left, Map<K, V> right) =>
            left.SymmetricExcept(right);
    }
}
