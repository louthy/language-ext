﻿using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable hash-map module
    /// </summary>
    public static partial class HashMap
    {
        /// <summary>
        /// Clears all items from the map
        /// </summary>
        /// <param name="map">Map to clear</param>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public static HMap<K, V> clear<K, V>(HMap<K, V> map) =>
            map.Clear();

        /// <summary>
        /// Creates a new empty Map
        /// </summary>
        [Pure]
        public static HMap<K, V> empty<K, V>() =>
            HMap<K, V>.Empty;

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static HMap<K, V> create<K, V>(params Tuple<K, V>[] keyValues) =>
            empty<K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static HMap<K, V> createRange<K, V>(IEnumerable<Tuple<K, V>> keyValues) =>
            empty<K, V>().AddRange(keyValues);

        /// <summary>
        /// Creates a new Map seeded with the keyValues provided
        /// </summary>
        [Pure]
        public static HMap<K, V> createRange<K, V>(IEnumerable<KeyValuePair<K, V>> keyValues) =>
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
        public static HMap<K, V> add<K, V>(HMap<K, V> map, K key, V value) =>
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
        public static HMap<K, V> tryAdd<K, V>(HMap<K, V> map, K key, V value) =>
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
        public static HMap<K, V> addOrUpdate<K, V>(HMap<K, V> map, K key, V value) =>
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
        public static HMap<K, V> addOrUpdate<K, V>(HMap<K, V> map, K key, Func<V, V> Some, Func<V> None) =>
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
        public static HMap<K, V> addOrUpdate<K, V>(HMap<K, V> map, K key, Func<V, V> Some, V None) =>
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
        public static HMap<K, V> addRange<K, V>(HMap<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
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
        public static HMap<K, V> addRange<K, V>(HMap<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) =>
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
        public static HMap<K, V> tryAddRange<K, V>(HMap<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
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
        public static HMap<K, V> tryAddRange<K, V>(HMap<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) =>
            map.TryAddRange(keyValues);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <param name="range">Range of tuples to add</param>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public static HMap<K, V> addOrUpdateRange<K, V>(HMap<K, V> map, IEnumerable<Tuple<K, V>> range) =>
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
        public static HMap<K, V> addOrUpdateRange<K, V>(HMap<K, V> map, IEnumerable<KeyValuePair<K, V>> range) =>
            map.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public static HMap<K, V> remove<K, V>(HMap<K, V> map, K key) =>
            map.Remove(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool containsKey<K, V>(HMap<K, V> map, K key) =>
            map.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(HMap<K, V> map, KeyValuePair<K, V> kv) =>
            map.Contains(kv.Key, kv.Value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public static bool contains<K, V>(HMap<K, V> map, Tuple<K, V> kv) =>
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
        public static HMap<K, V> setItem<K, V>(HMap<K, V> map, K key, V value) =>
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
        public static HMap<K, V> trySetItem<K, V>(HMap<K, V> map, K key, V value) =>
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
        public static HMap<K, V> trySetItem<K, V>(HMap<K, V> map, K key, Func<V, V> Some) =>
            map.TrySetItem(key, Some);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static HMap<K, V> setItems<K, V>(HMap<K, V> map, IEnumerable<Tuple<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static HMap<K, V> setItems<K, V>(HMap<K, V> map, IEnumerable<KeyValuePair<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static HMap<K, V> trySetItems<K, V>(HMap<K, V> map, IEnumerable<Tuple<K, V>> items) =>
            map.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public static HMap<K, V> trySetItems<K, V>(HMap<K, V> map, IEnumerable<KeyValuePair<K, V>> items) =>
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
        public static HMap<K, V> trySetItems<K, V>(HMap<K, V> map, IEnumerable<K> keys, Func<V, V> Some) =>
            map.TrySetItems(keys, Some);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static Option<V> find<K, V>(HMap<K, V> map, K key) =>
            map.Find(key);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static IEnumerable<V> findSeq<K, V>(HMap<K, V> map, K key) =>
            map.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public static R find<K, V, R>(HMap<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            map.Find(key, Some, None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public static HMap<K, V> setItem<K, V>(HMap<K, V> map, K key, Func<V, V> mapper) =>
            map.SetItem(key, mapper);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(HMap<K, V> map, Action<V> action) =>
            map.Iter(action);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit iter<K, V>(HMap<K, V> map, Action<K, V> action) =>
            map.Iter(action);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(HMap<K, V> map, Func<V, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(HMap<K, V> map, Func<K, V, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(HMap<K, V> map, Func<Tuple<K, V>, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool forall<K, V>(HMap<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
            map.ForAll(pred);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static HMap<K, U> map<K, T, U>(HMap<K, T> map, Func<T, U> f) =>
            map.Select(f);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static HMap<K, U> map<K, T, U>(HMap<K, T> map, Func<K, T, U> f) =>
            map.Select(f);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static HMap<K, V> filter<K, V>(HMap<K, V> map, Func<V, bool> predicate) =>
            map.Filter(predicate);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static HMap<K, V> filter<K, V>(HMap<K, V> map, Func<K, V, bool> predicate) =>
            map.Filter(predicate);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public static int length<K, T>(HMap<K, T> map) =>
            map.Count;

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, K, V>(HMap<K, V> map, S state, Func<S, K, V, S> folder) =>
            map.Fold(state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, K, V>(HMap<K, V> map, S state, Func<S, V, S> folder) =>
            map.Fold(state, folder);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(HMap<K, V> map, Func<K, V, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(HMap<K, V> map, Func<Tuple<K, V>, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(HMap<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
            map.Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool exists<K, V>(HMap<K, V> map, Func<V, bool> pred) =>
            map.Exists(pred);
    }
}

public static class HMapExtensions
{
    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    public static int Count<K, V>(this HMap<K, V> self) =>
        self.Count;

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HMap<K, U> Select<K, V, U>(this HMap<K, V> self, Func<V, U> mapper) =>
        self.Map(mapper);

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HMap<K, U> Select<K, V, U>(this HMap<K, V> self, Func<K, V, U> mapper) =>
        self.Map(mapper);

    /// <summary>
    /// Atomically filter out items that return false when a predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>New map with items filtered</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HMap<K, V> Where<K, V>(this HMap<K, V> self, Func<V, bool> pred) =>
        self.Filter(pred);

    /// <summary>
    /// Atomically filter out items that return false when a predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>New map with items filtered</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HMap<K, V> Where<K, V>(this HMap<K, V> self, Func<K, V, bool> pred) =>
        self.Filter(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<K, V>(this HMap<K, V> self, Func<K, V, bool> pred)
    {
        foreach(var item in self.AsEnumerable())
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
    public static bool ForAll<K, V>(this HMap<K, V> self, Func<Tuple<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if *all* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<K, V>(this HMap<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<K, V>(this HMap<K, V> self, Func<V, bool> pred) =>
        self.Values.ForAll(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    public static bool Exists<K, V>(this HMap<K, V> self, Func<K, V, bool> pred)
    {
        foreach (var item in self.AsEnumerable())
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
    public static bool Exists<K, V>(this HMap<K, V> self, Func<Tuple<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<K, V>(this HMap<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => new KeyValuePair<K,V>(kv.Key, kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<K, V>(this HMap<K, V> self, Func<V, bool> pred) =>
        self.Values.Exists(pred);

    /// <summary>
    /// Atomically iterate through all key/value pairs in the map (in order) and execute an
    /// action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<K, V>(this HMap<K, V> self, Action<K, V> action)
    {
        foreach (var item in self)
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
    public static Unit Iter<K, V>(this HMap<K, V> self, Action<V> action)
    {
        foreach (var item in self)
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
    public static Unit Iter<K, V>(this HMap<K, V> self, Action<Tuple<K, V>> action)
    {
        foreach (var item in self)
        {
            action(new Tuple<K, V>(item.Key, item.Value));
        }
        return unit;
    }

    /// <summary>
    /// Atomically iterate through all key/value pairs in the map (in order) and execute an
    /// action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<K, V>(this HMap<K, V> self, Action<KeyValuePair<K, V>> action)
    {
        foreach (var item in self)
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
    public static S Fold<K, V, S>(this HMap<K, V> self, S state, Func<S, K, V, S> folder) =>
        self.AsEnumerable().Fold(state, (s,x) => folder(s, x.Key,x.Value));

    /// <summary>
    /// Atomically folds all items in the map (in order) using the folder function provided.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<K, V, S>(this HMap<K, V> self, S state, Func<S, V, S> folder) =>
        self.Values.Fold(state, folder);

    [Pure]
    public static Map<K, U> Bind<K, T, U>(this HMap<K, T> self, Func<T, Map<K, U>> binder) =>
        failwith<Map<K, U>>("Map<K,V> doesn't support Bind.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Map<K, U> SelectMany<K, T, U>(this HMap<K, T> self, Func<T, Map<K, U>> binder) =>
        failwith<Map<K, U>>("Map<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Map<K, V> SelectMany<K, T, U, V>(this HMap<K, T> self, Func<T, Map<K, U>> binder, Func<T, U, V> project) =>
        failwith<Map<K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    public static int Sum<K>(this HMap<K, int> self) =>
        self.Values.Sum();

    [Pure]
    public static Option<T> Find<A, B, T>(this HMap<A, HMap<B, T>> self, A outerKey, B innerKey) =>
        self.Find(outerKey, b => b.Find(innerKey), () => None);

    [Pure]
    public static Option<T> Find<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, A aKey, B bKey, C cKey) =>
        self.Find(aKey, b => b.Find(bKey, c => c.Find(cKey), () => None), () => None);

    [Pure]
    public static R Find<A, B, T, R>(this HMap<A, HMap<B, T>> self, A outerKey, B innerKey, Func<T, R> Some, Func<R> None) =>
        self.Find(outerKey, b => b.Find(innerKey, Some, None), None);

    [Pure]
    public static R Find<A, B, C, T, R>(this HMap<A, HMap<B, HMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey, Some, None),
                None),
            None);

    [Pure]
    public static R Find<A, B, C, D, T, R>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey,
                    d => d.Find(dKey, Some, None),
                    None),
                None),
            None);

    [Pure]
    public static HMap<A, HMap<B, T>> AddOrUpdate<A, B, T>(this HMap<A, HMap<B, T>> self, A outerKey, B innerKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, Some, None),
            () => Prelude.HashMap(Tuple(innerKey, None()))
        );

    [Pure]
    public static HMap<A, HMap<B, T>> AddOrUpdate<A, B, T>(this HMap<A, HMap<B, T>> self, A outerKey, B innerKey, T value) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, _ => value, value),
            () => Prelude.HashMap(Tuple(innerKey, value))
        );

    [Pure]
    public static HMap<A, HMap<B, HMap<C, T>>> AddOrUpdate<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, A aKey, B bKey, C cKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, _ => value, value),
            () => Prelude.HashMap(Tuple(cKey, value))
        );

    [Pure]
    public static HMap<A, HMap<B, HMap<C, T>>> AddOrUpdate<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, Some, None),
            () => Prelude.HashMap(Tuple(cKey, None()))
        );

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, _ => value, value),
            () => Prelude.HashMap(Tuple(dKey, value))
        );

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, Some, None),
            () => Prelude.HashMap(Tuple(dKey, None()))
        );

    [Pure]
    public static HMap<A, HMap<B, T>> Remove<A, B, T>(this HMap<A, HMap<B, T>> self, A outerKey, B innerKey)
    {
        var b = self.Find(outerKey);
        if (b.IsSome)
        {
            var bv = b.Value.Remove(innerKey);
            if (bv.Count() == 0)
            {
                return self.Remove(outerKey);
            }
            else
            {
                return self.SetItem(outerKey, bv);
            }
        }
        else
        {
            return self;
        }
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, T>>> Remove<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, A aKey, B bKey, C cKey)
    {
        var b = self.Find(aKey);
        if (b.IsSome)
        {
            var c = b.Value.Find(bKey);
            if (c.IsSome)
            {
                var cv = c.Value.Remove(cKey);
                if (cv.Count() == 0)
                {
                    var bv = b.Value.Remove(bKey);
                    if (b.Value.Count() == 0)
                    {
                        return self.Remove(aKey);
                    }
                    else
                    {
                        return self.SetItem(aKey, bv);
                    }
                }
                else
                {
                    return self.SetItem(aKey, b.Value.SetItem(bKey, cv));
                }
            }
            else
            {
                return self;
            }
        }
        else
        {
            return self;
        }
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, T>>>> Remove<A, B, C, D, T>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey)
    {
        var res = self.Find(aKey, bKey, cKey);
        if (res.IsSome && res.CountT() > 1)
        {
            return self.SetItemT(aKey, bKey, cKey, res.Lift().Remove(dKey));
        }
        else
        {
            if (res.IsSome)
            {
                if (res.MapT(d => d.ContainsKey(dKey)).Lift())
                {
                    return Remove(self, aKey, bKey, cKey);
                }
                else
                {
                    return self;
                }
            }
            else
            {
                return Remove(self, aKey, bKey, cKey);
            }
        }
    }

    [Pure]
    public static HMap<A, HMap<B, V>> MapRemoveT<A, B, T, V>(this HMap<A, HMap<B, T>> self, Func<HMap<B, T>, HMap<B, V>> map)
    {
        return self.Map((ka, va) => map(va))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, V>>> MapRemoveT<A, B, C, T, V>(this HMap<A, HMap<B, HMap<C, T>>> self, Func<HMap<C, T>, HMap<C, V>> map)
    {
        return self.Map((ka, va) => va.Map((kb, vb) => map(vb))
                                      .Filter((kb, vb) => vb.Count > 0))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, V>>>> MapRemoveT<A, B, C, D, T, V>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, Func<HMap<D, T>, HMap<D, V>> map)
    {
        return self.Map((ka, va) => va.Map((kb, vb) => vb.Map((kc, vc) => map(vc))
                                                         .Filter((kc, vc) => vc.Count > 0))
                                      .Filter((kb, vb) => vb.Count > 0))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HMap<A, HMap<B, V>> MapT<A, B, T, V>(this HMap<A, HMap<B, T>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.Map(map));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, V>>> MapT<A, B, C, T, V>(this HMap<A, HMap<B, HMap<C, T>>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.MapT(map));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, V>>>> MapT<A, B, C, D, T, V>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.MapT(map));
    }

    [Pure]
    public static HMap<A, HMap<B, T>> FilterT<A, B, T>(this HMap<A, HMap<B, T>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.Filter(pred));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, T>>> FilterT<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.FilterT(pred));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, T>>>> FilterT<A, B, C, D, T>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.FilterT(pred));
    }

    [Pure]
    public static HMap<A, HMap<B, T>> FilterRemoveT<A, B, T>(this HMap<A, HMap<B, T>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, T>>> FilterRemoveT<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, T>>>> FilterRemoveT<A, B, C, D, T>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static bool Exists<A, B, T>(this HMap<A, HMap<B, T>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool Exists<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool Exists<A, B, C, D, T>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool ForAll<A, B, T>(this HMap<A, HMap<B, T>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static bool ForAll<A, B, C, T>(this HMap<A, HMap<B, HMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static bool ForAll<A, B, C, D, T>(this HMap<A, HMap<B, HMap<C, HMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static HMap<A, HMap<B, V>> SetItemT<A, B, V>(this HMap<A, HMap<B, V>> map, A aKey, B bKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;
        return map.SetItem(aKey, av.SetItem(bKey, value));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, V>>> SetItemT<A, B, C, V>(this HMap<A, HMap<B, HMap<C, V>>> map, A aKey, B bKey, C cKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, value));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, V>>>> SetItemT<A, B, C, D, V>(this HMap<A, HMap<B, HMap<C, HMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, value));
    }

    [Pure]
    public static HMap<A, HMap<B, V>> SetItemT<A, B, V>(this HMap<A, HMap<B, V>> map, A aKey, B bKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;
        return map.SetItem(aKey, av.SetItem(bKey, Some));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, V>>> SetItemT<A, B, C, V>(this HMap<A, HMap<B, HMap<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, Some));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, V>>>> SetItemT<A, B, C, D, V>(this HMap<A, HMap<B, HMap<C, HMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, Some));
    }

    [Pure]
    public static HMap<A, HMap<B, V>> TrySetItemT<A, B, V>(this HMap<A, HMap<B, V>> map, A aKey, B bKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;
        return map.SetItem(aKey, av.TrySetItem(bKey, value));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, V>>> TrySetItemT<A, B, C, V>(this HMap<A, HMap<B, HMap<C, V>>> map, A aKey, B bKey, C cKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, value));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, V>>>> TrySetItemT<A, B, C, D, V>(this HMap<A, HMap<B, HMap<C, HMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, value));
    }

    [Pure]
    public static HMap<A, HMap<B, V>> TrySetItemT<A, B, V>(this HMap<A, HMap<B, V>> map, A aKey, B bKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;
        return map.SetItem(aKey, av.TrySetItem(bKey, Some));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, V>>> TrySetItemT<A, B, C, V>(this HMap<A, HMap<B, HMap<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, Some));
    }

    [Pure]
    public static HMap<A, HMap<B, HMap<C, HMap<D, V>>>> TrySetItemT<A, B, C, D, V>(this HMap<A, HMap<B, HMap<C, HMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, Some));
    }

    [Pure]
    public static S FoldT<A, B, S, V>(this HMap<A, HMap<B, V>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.Fold(s, folder));
    }

    [Pure]
    public static S FoldT<A, B, C, S, V>(this HMap<A, HMap<B, HMap<C, V>>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.FoldT(s, folder));
    }

    [Pure]
    public static S FoldT<A, B, C, D, S, V>(this HMap<A, HMap<B, HMap<C, HMap<D, V>>>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.FoldT(s, folder));
    }

    [Pure]
    public static HMap<K, U> Bind<K, T, U>(this HMap<K, T> self, Func<T, HMap<K, U>> binder) =>
        failwith<HMap<K, U>>("HMap<K,V> doesn't support Bind.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HMap<K, U> SelectMany<K, T, U>(this HMap<K, T> self, Func<T, HMap<K, U>> binder) =>
        failwith<HMap<K, U>>("HMap<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HMap<K, V> SelectMany<K, T, U, V>(this HMap<K, T> self, Func<T, HMap<K, U>> binder, Func<T, U, V> project) =>
        failwith<HMap<K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

}