using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Immutable map module
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// [en.wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
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
        public static Map<OrdK, K, V> clear<OrdK, K, V>(Map<OrdK, K, V> map) where OrdK : struct, Ord<K> =>
            Map<OrdK, K, V>.Empty;

        /// <summary>
        /// Creates a new empty Map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> empty<OrdK, K, V>() where OrdK : struct, Ord<K> =>
            Map<OrdK, K, V>.Empty;

        /// <summary>
        /// Creates a new empty Map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> create<OrdK, K, V>() where OrdK : struct, Ord<K> =>
            Map<OrdK, K, V>.Empty;

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> create<OrdK, K, V>(Tuple<K,V> head, params Tuple<K, V>[] tail) where OrdK : struct, Ord<K> =>
            empty<OrdK, K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> create<OrdK, K, V>(KeyValuePair<K, V> head, params KeyValuePair<K, V>[] tail) where OrdK : struct, Ord<K> =>
            empty<OrdK, K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> create<OrdK, K, V>((K, V) head, params (K, V)[] tail) where OrdK : struct, Ord<K> =>
            empty<OrdK, K, V>().AddRange(head.Cons(tail));

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> createRange<OrdK, K, V>(IEnumerable<Tuple<K, V>> keyValues) where OrdK : struct, Ord<K> =>
            empty<OrdK, K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> createRange<OrdK, K, V>(IEnumerable<(K, V)> keyValues) where OrdK : struct, Ord<K> =>
            empty<OrdK, K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> createRange<OrdK, K, V>(IEnumerable<KeyValuePair<K, V>> keyValues) where OrdK : struct, Ord<K> =>
            empty<OrdK, K, V>().AddRange(keyValues);

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
        public static Map<OrdK, K, V> add<OrdK, K, V>(Map<OrdK, K, V> map, K key, V value) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> tryAdd<OrdK, K, V>(Map<OrdK, K, V> map, K key, V value) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> tryAdd<OrdK, K, V>(Map<OrdK, K, V> map, K key, V value, Func<Map<OrdK, K, V>, V, Map<OrdK, K, V>> Fail) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> addOrUpdate<OrdK, K, V>(Map<OrdK, K, V> map, K key, V value) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> addOrUpdate<OrdK, K, V>(Map<OrdK, K, V> map, K key, Func<V, V> Some, Func<V> None) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> addOrUpdate<OrdK, K, V>(Map<OrdK, K, V> map, K key, Func<V, V> Some, V None) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> addRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<Tuple<K, V>> keyValues) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> addRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<(K, V)> keyValues) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> addRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> tryAddRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<Tuple<K, V>> keyValues) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> tryAddRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<(K, V)> keyValues) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> tryAddRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) where OrdK : struct, Ord<K> =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static Map<OrdK, K, V> addOrUpdateRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<Tuple<K, V>> range) where OrdK : struct, Ord<K> =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static Map<OrdK, K, V> addOrUpdateRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<(K, V)> range) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> addOrUpdateRange<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<KeyValuePair<K, V>> range) where OrdK : struct, Ord<K> =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public static Map<OrdK, K, V> remove<OrdK, K, V>(Map<OrdK, K, V> map, K key) where OrdK : struct, Ord<K> =>
            map.Remove(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool containsKey<OrdK, K, V>(Map<OrdK, K, V> map, K key) where OrdK : struct, Ord<K> =>
            map.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<OrdK, K, V>(Map<OrdK, K, V> map, KeyValuePair<K, V> kv) where OrdK : struct, Ord<K> =>
            map.Contains(kv.Key, kv.Value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<OrdK, K, V>(Map<OrdK, K, V> map, Tuple<K, V> kv) where OrdK : struct, Ord<K> =>
            map.Contains(kv.Item1, kv.Item2);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<OrdK, K, V>(Map<OrdK, K, V> map, (K, V) kv) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> setItem<OrdK, K, V>(Map<OrdK, K, V> map, K key, V value) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> trySetItem<OrdK, K, V>(Map<OrdK, K, V> map, K key, V value) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> trySetItem<OrdK, K, V>(Map<OrdK, K, V> map, K key, Func<V, V> Some) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> trySetItem<OrdK, K, V>(Map<OrdK, K, V> map, K key, Func<V, V> Some, Func<Map<K, V>, Map<K, V>> None) where OrdK : struct, Ord<K> =>
            map.TrySetItem(key, Some, None);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<OrdK, K, V> setItems<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<Tuple<K, V>> items) where OrdK : struct, Ord<K> =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<OrdK, K, V> setItems<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<(K, V)> items) where OrdK : struct, Ord<K> =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<OrdK, K, V> setItems<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<KeyValuePair<K, V>> items) where OrdK : struct, Ord<K> =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<OrdK, K, V> trySetItems<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<Tuple<K, V>> items) where OrdK : struct, Ord<K> =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<OrdK, K, V> trySetItems<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<(K, V)> items) where OrdK : struct, Ord<K> =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static Map<OrdK, K, V> trySetItems<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<KeyValuePair<K, V>> items) where OrdK : struct, Ord<K> =>
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
        public static Map<OrdK, K, V> trySetItems<OrdK, K, V>(Map<OrdK, K, V> map, IEnumerable<K> keys, Func<V, V> Some) where OrdK : struct, Ord<K> =>
            map.TrySetItems(keys, Some);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static Option<V> find<OrdK, K, V>(Map<OrdK, K, V> map, K key) where OrdK : struct, Ord<K> =>
            map.Find(key);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static IEnumerable<V> findSeq<OrdK, K, V>(Map<OrdK, K, V> map, K key) where OrdK : struct, Ord<K> =>
            map.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static R find<OrdK, K, V, R>(Map<OrdK, K, V> map, K key, Func<V, R> Some, Func<R> None) where OrdK : struct, Ord<K> =>
            map.Find(key, Some, None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public static Map<OrdK, K, V> setItem<OrdK, K, V>(Map<OrdK, K, V> map, K key, Func<V, V> mapper) where OrdK : struct, Ord<K> =>
            map.SetItem(key, mapper);

        /// <summary>
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        [Pure]
        public static IEnumerable<V> findRange<OrdK, K, V>(Map<OrdK, K, V> map, K keyFrom, K keyTo) where OrdK : struct, Ord<K> =>
            map.FindRange(keyFrom, keyTo);

        /// <summary>
        /// Skips 'amount' values and returns a new tree without the 
        /// skipped values.
        /// </summary>
        /// <param name="amount">Amount to skip</param>
        /// <returns>Enumerable of map items</returns>
        [Pure]
        public static IEnumerable<(K Key, V Value)> skip<OrdK, K, V>(Map<OrdK, K, V> map, int amount) where OrdK : struct, Ord<K> =>
            map.Skip(amount);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<OrdK, K, V>(Map<OrdK, K, V> map, Action<V> action) where OrdK : struct, Ord<K> =>
            map.Iter(action);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<OrdK, K, V>(Map<OrdK, K, V> map, Action<K, V> action) where OrdK : struct, Ord<K> =>
            map.Iter(action);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<OrdK, K, V>(Map<OrdK, K, V> map, Func<V, bool> pred) where OrdK : struct, Ord<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<OrdK, K, V>(Map<OrdK, K, V> map, Func<K, V, bool> pred) where OrdK : struct, Ord<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<OrdK, K, V>(Map<OrdK, K, V> map, Func<Tuple<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<OrdK, K, V>(Map<OrdK, K, V> map, Func<(K Key, V Value), bool> pred) where OrdK : struct, Ord<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<OrdK, K, V>(Map<OrdK, K, V> map, Func<KeyValuePair<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            map.ForAll(pred);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<OrdK, K, U> map<OrdK, K, T, U>(Map<OrdK, K, T> map, Func<T, U> f) where OrdK : struct, Ord<K> =>
            (Map<OrdK, K, U>)map.Select(f);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<OrdK, K, U> map<OrdK, K, T, U>(Map<OrdK, K, T> map, Func<K, T, U> f) where OrdK : struct, Ord<K> =>
            map.Select(f);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<OrdK, K, V> filter<OrdK, K, V>(Map<OrdK, K, V> map, Func<V, bool> predicate) where OrdK : struct, Ord<K> =>
            map.Filter(predicate);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<OrdK, K, V> filter<OrdK, K, V>(Map<OrdK, K, V> map, Func<K, V, bool> predicate) where OrdK : struct, Ord<K> =>
            map.Filter(predicate);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public static Map<OrdK, K, R> choose<OrdK, K, T, R>(Map<OrdK, K, T> map, Func<T, Option<R>> selector) where OrdK : struct, Ord<K> =>
            map.Choose(selector);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public static Map<OrdK, K, R> choose<OrdK, K, T, R>(Map<OrdK, K, T> map, Func<K, T, Option<R>> selector) where OrdK : struct, Ord<K> =>
            map.Choose(selector);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public static int length<OrdK, K, T>(Map<OrdK, K, T> map) where OrdK : struct, Ord<K> =>
            map.Count;

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<OrdK, S, K, V>(Map<OrdK, K, V> map, S state, Func<S, K, V, S> folder) where OrdK : struct, Ord<K> =>
            map.Fold(state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<OrdK, S, K, V>(Map<OrdK, K, V> map, S state, Func<S, V, S> folder) where OrdK : struct, Ord<K> =>
            map.Fold(state, folder);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<OrdK, K, V>(Map<OrdK, K, V> map, Func<K, V, bool> pred) where OrdK : struct, Ord<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<OrdK, K, V>(Map<OrdK, K, V> map, Func<Tuple<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<OrdK, K, V>(Map<OrdK, K, V> map, Func<(K Key, V Value), bool> pred) where OrdK : struct, Ord<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<OrdK, K, V>(Map<OrdK, K, V> map, Func<KeyValuePair<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<OrdK, K, V>(Map<OrdK, K, V> map, Func<V, bool> pred) where OrdK : struct, Ord<K> =>
            map.Exists(pred);

        /// <summary>
        /// Convert any IDictionary into an immutable Map K V
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> freeze<OrdK, K, V>(IDictionary<K, V> dict) where OrdK : struct, Ord<K> =>
            toMap<OrdK, K, V>(dict.AsEnumerable());

        /// <summary>
        /// Convert any IDictionary into an immutable Map K V
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> Freeze<OrdK, K, V>(this IDictionary<K, V> dict) where OrdK : struct, Ord<K> =>
            toMap<OrdK, K, V>(dict.AsEnumerable());

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> union<OrdK, K, V>(Map<OrdK, K, V> left, Map<OrdK, K, V> right, WhenMatched<K, V, V, V> Merge) where OrdK : struct, Ord<K> =>
            left.Union(right, (k, v) => v, (k, v) => v, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<OrdK, K, A> union<OrdK, K, A, B>(Map<OrdK, K, A> left, Map<OrdK, K, B> right, WhenMissing<K, B, A> MapRight, WhenMatched<K, A, B, A> Merge) where OrdK : struct, Ord<K> =>
            left.Union(right, (k, v) => v, MapRight, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<OrdK, K, B> union<OrdK, K, A, B>(Map<OrdK, K, A> left, Map<OrdK, K, B> right, WhenMissing<K, A, B> MapLeft, WhenMatched<K, A, B, B> Merge) where OrdK : struct, Ord<K> =>
            left.Union(right, MapLeft, (k, v) => v, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public static Map<OrdK, K, C> union<OrdK, K, A, B, C>(Map<OrdK, K, A> left, Map<OrdK, K, B> right, WhenMissing<K, A, C> MapLeft, WhenMissing<K, B, C> MapRight, WhenMatched<K, A, B, C> Merge) where OrdK : struct, Ord<K> =>
            left.Union(right, MapLeft, MapRight, Merge);

        /// <summary>
        /// Intersect two maps.  Only keys that are in both maps are
        /// returned.  The merge function is called for every resulting
        /// key.
        [Pure]
        public static Map<OrdK, K, R> intersect<OrdK, K, A, B, R>(Map<OrdK, K, A> left, Map<OrdK, K, B> right, WhenMatched<K, A, B, R> merge) where OrdK : struct, Ord<K> =>
            left.Intersect(right, merge);

        /// <summary>
        /// Map differencing based on key.  this - other.
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> except<OrdK, K, V>(Map<OrdK, K, V> left, Map<OrdK, K, V> right) where OrdK : struct, Ord<K> =>
            left.Except(right);

        /// <summary>
        /// Keys that are in both maps are dropped and the remaining
        /// items are merged and returned.
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> symmetricExcept<OrdK, K, V>(Map<OrdK, K, V> left, Map<OrdK, K, V> right) where OrdK : struct, Ord<K> =>
            left.SymmetricExcept(right);
    }
}
