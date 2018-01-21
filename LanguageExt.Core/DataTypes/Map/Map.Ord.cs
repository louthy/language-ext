﻿using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable map
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// http://en.wikipedia.org/wiki/AVL_tree
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    [Serializable]
    public struct Map<OrdK, K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<Map<OrdK, K, V>>,
        IComparable<Map<OrdK, K, V>>
        where OrdK : struct, Ord<K>
    {
        readonly MapInternal<OrdK, K, V> value;

        internal static Map<OrdK, K, V> Wrap(MapInternal<OrdK, K, V> map) =>
            new Map<OrdK, K, V>(map);

        public Map(IEnumerable<(K Key, V Value)> items)
        {
            var map = Map<OrdK, K, V>.Empty;
            foreach (var item in items)
            {
                map = map.Add(item.Key, item.Value);
            }
            this.value = map.value;
        }

        internal Map(MapInternal<OrdK, K, V> value)
        {
            this.value = value;
        }

        internal Map(MapItem<K, V> root, bool rev)
        {
            this.value = new MapInternal<OrdK, K, V>(root, rev);
        }

        internal MapInternal<OrdK, K, V> Value =>
            value ?? MapInternal<OrdK, K, V>.Empty;

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public V this[K key] => Value[key];

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty => Value.IsEmpty;

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count => Value.Count;

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length => Value.Length;

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
        public Map<OrdK, K, V> Add(K key, V value) => Wrap(Value.Add(key,value));

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
        public Map<OrdK, K, V> TryAdd(K key, V value) => Wrap(Value.TryAdd(key, value));

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
        public Map<OrdK, K, V> TryAdd(K key, V value, Func<Map<OrdK, K, V>, V, Map<OrdK, K, V>> Fail) => 
            Wrap(Value.TryAdd(key, value, (m,v) => Fail(Wrap(m),v).Value));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> AddRange(IEnumerable<Tuple<K, V>> range) => Wrap(Value.AddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> AddRange(IEnumerable<(K, V)> range) => Wrap(Value.AddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> TryAddRange(IEnumerable<Tuple<K, V>> range) => Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> TryAddRange(IEnumerable<(K, V)> range) => Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range) => Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range) => Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> AddOrUpdateRange(IEnumerable<(K, V)> range) => Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range) => Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public Map<OrdK, K, V> Remove(K key) => Wrap(Value.Remove(key));

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<V> Find(K key) => Value.Find(key);
        
        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public IEnumerable<V> FindSeq(K key) => Value.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(K key, Func<V, R> Some, Func<R> None) => Value.Find(key, Some, None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public Map<OrdK, K, V> SetItem(K key, V value) => Wrap(Value.SetItem(key, value));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the item isn't found</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<OrdK, K, V> SetItem(K key, Func<V, V> Some) => Wrap(Value.SetItem(key, Some));

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
        public Map<OrdK, K, V> TrySetItem(K key, V value) => Wrap(Value.TrySetItem(key, value));

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
        public Map<OrdK, K, V> TrySetItem(K key, Func<V, V> Some) => Wrap(Value.TrySetItem(key, Some));

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
        public Map<OrdK, K, V> TrySetItem(K key, Func<V, V> Some, Func<Map<K, V>, Map<K, V>> None) => Wrap(Value.TrySetItem(key, Some, None));

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
        public Map<OrdK, K, V> AddOrUpdate(K key, V value) => Wrap(Value.AddOrUpdate(key,value));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<OrdK, K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) => Wrap(Value.AddOrUpdate(key, Some, None));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<OrdK, K, V> AddOrUpdate(K key, Func<V, V> Some, V None) => Wrap(Value.AddOrUpdate(key, Some, None));

        /// <summary>
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        [Pure]
        public IEnumerable<V> FindRange(K keyFrom, K keyTo) => Value.FindRange(keyFrom, keyTo);

        /// <summary>
        /// Skips 'amount' values and returns a new tree without the 
        /// skipped values.
        /// </summary>
        /// <param name="amount">Amount to skip</param>
        /// <returns>New tree</returns>
        [Pure]
        public IEnumerable<(K Key, V Value)> Skip(int amount) => Value.Skip(amount);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool ContainsKey(K key) => Value.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains(K key, V value) => Value.Contains(key, value);

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public Map<OrdK, K, V> Clear() => Wrap(Value.Clear());

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<OrdK, K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) => Wrap(Value.AddRange(pairs));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<OrdK, K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items) => Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<OrdK, K, V> SetItems(IEnumerable<Tuple<K, V>> items) => Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<OrdK, K, V> SetItems(IEnumerable<(K, V)> items) => Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<OrdK, K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items) => Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<OrdK, K, V> TrySetItems(IEnumerable<Tuple<K, V>> items) => Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<OrdK, K, V> TrySetItems(IEnumerable<(K, V)> items) => Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<OrdK, K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some) => Wrap(Value.TrySetItems(keys, Some));

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New map with the items removed</returns>
        [Pure]
        public Map<OrdK, K, V> RemoveRange(IEnumerable<K> keys) => Wrap(Value.RemoveRange(keys));

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) => Value.Contains(pair);

        /// <summary>
        /// Enumerable of map keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys => Value.Keys;

        /// <summary>
        /// Enumerable of map values
        /// </summary>
        [Pure]
        public IEnumerable<V> Values => Value.Values;

        /// <summary>
        /// Convert the map to an `IReadOnlyDictionary<K, V>`
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IReadOnlyDictionary<K, V> ToDictionary() => Value.ToDictionary();

        /// <summary>
        /// Map the map the a dictionary
        /// </summary>
        [Pure]
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<(K Key, V Value), KR> keySelector, Func<(K Key, V Value), VR> valueSelector)
            => Value.ToDictionary(keySelector, valueSelector);

        /// <summary>
        /// Get a IReadOnlyDictionary for this map.  No mapping is required, so this is very fast.
        /// </summary>
        [Pure]
        public IReadOnlyDictionary<K, V> ToReadOnlyDictionary() =>
            value;

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<Tuple<K, V>> Tuples => 
            Value.Tuples;

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<(K Key, V Value)> ValueTuples =>
            Value.ValueTuples;

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [Pure]
        public IEnumerator<(K Key, V Value)> GetEnumerator() => 
            Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => 
            Value.GetEnumerator();

        [Pure]
        public Seq<(K Key, V Value)> ToSeq() =>
            Seq(this);

        [Pure]
        public IEnumerable<(K Key, V Value)> AsEnumerable() => 
            Value.AsEnumerable();

        internal Map<OrdK, K, V> SetRoot(MapItem<K, V> root) =>
            new Map<OrdK, K, V>(new MapInternal<OrdK, K, V>(root, Value.Rev));

        public static Map<OrdK, K, V> Empty = 
            new Map<OrdK, K, V>(MapInternal<OrdK, K, V>.Empty);

        [Pure]
        public bool Equals(Map<OrdK, K, V> y) =>
            Value == y.Value;

        [Pure]
        public static bool operator ==(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            lhs.Value == rhs.Value;

        [Pure]
        public static bool operator !=(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static bool operator <(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            lhs.CompareTo(rhs) < 0;

        [Pure]
        public static bool operator <=(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public static bool operator >(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            lhs.CompareTo(rhs) > 0;

        [Pure]
        public static bool operator >=(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        [Pure]
        public static Map<K, V> operator +(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            new Map<K, V>(lhs.Value + rhs.Value);

        [Pure]
        public static Map<K, V> operator -(Map<OrdK, K, V> lhs, Map<OrdK, K, V> rhs) =>
            new Map<K, V>(lhs.Value - rhs.Value);

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) && obj is Map<K, V> && Equals((Map<K, V>)obj);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();












        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map<OrdK, K, U> Select<U>(Func<V, U> mapper) =>
            new Map<OrdK, K, U>(MapModule.Map(Value.Root, mapper), Value.Rev);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map<OrdK, K, U> Select<U>(Func<K, V, U> mapper) =>
            new Map<OrdK, K, U>(MapModule.Map(Value.Root, mapper), Value.Rev);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map<OrdK, K, V> Where(Func<V, bool> pred) =>
            new Map<OrdK, K, V>(MapModule.Filter(Value.Root, pred), Value.Rev);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map<OrdK, K, V> Where(Func<K, V, bool> pred) =>
            SetRoot(MapModule.Filter(Value.Root, pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public Map<OrdK, K, V> Filter(Func<V, bool> pred) =>
            SetRoot(MapModule.Filter(Value.Root, pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public Map<OrdK, K, V> Filter(Func<K, V, bool> pred) =>
            SetRoot(MapModule.Filter(Value.Root, pred));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<K, V, bool> pred) =>
            MapModule.ForAll(Value.Root, pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<Tuple<K, V>, bool> pred) =>
            MapModule.ForAll(Value.Root, (k, v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<(K Key, V Value), bool> pred) =>
            MapModule.ForAll(Value.Root, (k, v) => pred((k, v)));

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.ForAll(Value.Root, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<V, bool> pred) =>
            MapModule.ForAll(Value.Root, (k, v) => pred(v));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<K, V, bool> pred) =>
            MapModule.Exists(Value.Root, pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<Tuple<K, V>, bool> pred) =>
            MapModule.Exists(Value.Root, (k, v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<(K, V), bool> pred) =>
            MapModule.Exists(Value.Root, (k, v) => pred((k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.Exists(Value.Root, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<V, bool> pred) =>
            MapModule.Exists(Value.Root, (_, v) => pred(v));

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
        public Unit Iter(Action<(K, V)> action) 
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
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public Map<OrdK, K, U> Choose<U>(Func<K, V, Option<U>> selector) =>
            new Map<OrdK, K, U>(MapModule.Choose(Value.Root, selector), Value.Rev);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public Map<OrdK, K, U> Choose<U>(Func<V, Option<U>> selector) =>
            new Map<OrdK, K, U>(MapModule.Choose(Value.Root, selector), Value.Rev);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, K, V, S> folder) =>
            MapModule.Fold(Value.Root, state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, V, S> folder) =>
            MapModule.Fold(Value.Root, state, folder);

        [Pure]
        public static implicit operator Map<OrdK, K, V>(ValueTuple<(K, V)> items) =>
            new Map<OrdK, K, V>(new[] { items.Item1 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14, items.Item15 });

        [Pure]
        public static implicit operator Map<OrdK, K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new Map<OrdK, K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14, items.Item15, items.Item16 });


        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public Map<OrdK, K, V> Union(Map<OrdK, K, V> other, WhenMatched<K, V, V, V> Merge) =>
            Union(other, (k, v) => v, (k, v) => v, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public Map<OrdK, K, V> Union<V2>(Map<OrdK, K, V2> other, WhenMissing<K, V2, V> MapRight, WhenMatched<K, V, V2, V> Merge) =>
            Union(other, (k, v) => v, MapRight, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public Map<OrdK, K, V2> Union<V2>(Map<OrdK, K, V2> other, WhenMissing<K, V, V2> MapLeft, WhenMatched<K, V, V2, V2> Merge) =>
            Union(other, MapLeft, (k, v) => v, Merge);

        /// <summary>
        /// Union two maps.  The merge function is called keys are
        /// present in both map.
        /// </summary>
        [Pure]
        public Map<OrdK, K, R> Union<V2, R>(Map<OrdK, K, V2> other, WhenMissing<K, V, R> MapLeft, WhenMissing<K, V2, R> MapRight, WhenMatched<K, V, V2, R> Merge)
        {
            // TODO: Look into more optimal solution

            if (MapLeft == null) throw new ArgumentNullException(nameof(MapLeft));
            if (MapRight == null) throw new ArgumentNullException(nameof(MapRight));
            if (Merge == null) throw new ArgumentNullException(nameof(Merge));

            var result = Map<OrdK, K, R>.Empty;
            foreach (var right in other)
            {
                var key = right.Key;
                var left = Find(key);
                if (left.IsSome)
                {
                    result = result.Add(key, Merge(key, left.Value, right.Value));
                }
                else
                {
                    result = result.Add(key, MapRight(key, right.Value));
                }
            }
            foreach (var left in this)
            {
                var key = left.Key;
                var right = other.Find(key);
                if (right.IsNone)
                {
                    result = result.Add(key, MapLeft(key, left.Value));
                }
            }
            return result;
        }

        /// <summary>
        /// Intersect two maps.  Only keys that are in both maps are
        /// returned.  The merge function is called for every resulting
        /// key.
        [Pure]
        public Map<OrdK, K, R> Intersect<V2, R>(Map<OrdK, K, V2> other, WhenMatched<K, V, V2, R> Merge)
        {
            // TODO: Look into more optimal solution

            if (Merge == null) throw new ArgumentNullException(nameof(Merge));

            var map = Map<OrdK, K, R>.Empty;
            foreach (var right in other)
            {
                var left = Find(right.Key);
                if (left.IsSome)
                {
                    map = map.Add(right.Key, Merge(right.Key, left.Value, right.Value));
                }
            }
            return map;
        }

        /// <summary>
        /// Map differencing based on key.  this - other.
        /// </summary>
        [Pure]
        public Map<OrdK, K, V> Except(Map<OrdK, K, V> other)
        {
            // TODO: Look into more optimal solution

            var map = this;
            foreach (var right in other)
            {
                if (map.ContainsKey(right.Key))
                {
                    map = map.Remove(right.Key);
                }
            }
            return map;
        }

        /// <summary>
        /// Keys that are in both maps are dropped and the remaining
        /// items are merged and returned.
        /// </summary>
        [Pure]
        public Map<OrdK, K, V> SymmetricExcept(Map<OrdK, K, V> other)
        {
            // TODO: Look into more optimal solution

            var map = Map<OrdK, K, V>.Empty;
            foreach (var left in this)
            {
                if (!other.ContainsKey(left.Key))
                {
                    map = map.Add(left.Key, left.Value);
                }
            }
            foreach (var right in other)
            {
                if (!ContainsKey(right.Key))
                {
                    map = map.Add(right.Key, right.Value);
                }
            }
            return map;
        }

        [Pure]
        public int CompareTo(Map<OrdK, K, V> other) =>
            Value.CompareTo(other.Value);
    }
}
