using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;
using System.Runtime.CompilerServices;
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
#if !COREFX
    [Serializable]
#endif
    public class Map<K, V> : 
        IEnumerable<IMapItem<K, V>>, 
        IReadOnlyDictionary<K,V>, 
        IAppendable<Map<K, V>>, 
        ISubtractable<Map<K, V>>
    {
        public static readonly Map<K, V> Empty = new Map<K, V>();

        internal readonly MapItem<K, V> Root;
        internal readonly bool Rev;

        /// <summary>
        /// Ctor
        /// </summary>
        internal Map()
        {
            Root = MapItem<K,V>.Empty;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal Map(MapItem<K,V> root, bool rev)
        {
            Root = root;
            Rev = rev;
        }

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public V this[K key]
        {
            get
            {
                return Find(key).IfNone(() => failwith<V>("Key doesn't exist in map"));
            }
        }

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty =>
            Count == 0;

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count => 
            Root.Count;

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length =>
            Count;

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
        public Map<K, V> Add(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            return SetRoot(MapModule.Add(Root, key, value, Comparer<K>.Default));
        }

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
        public Map<K, V> TryAdd(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            return SetRoot(MapModule.TryAdd(Root, key, value, Comparer<K>.Default));
        }

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
        public Map<K, V> TryAdd(K key, V value, Func<Map<K, V>, V, Map<K, V>> Fail)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            return Find(key, v => Fail(this, v), () => Add(key, value));
        }

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.Add(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> TryAddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.TryAdd(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Key)) throw new ArgumentNullException(nameof(item.Key));
                self = MapModule.TryAdd(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.AddOrUpdate(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Key)) throw new ArgumentNullException(nameof(item.Key));
                self = MapModule.AddOrUpdate(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public Map<K, V> Remove(K key) =>
            isnull(key)
                ? this
                : SetRoot(MapModule.Remove(Root, key, Comparer<K>.Default));

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<V> Find(K key) =>
            isnull(key)
                ? None
                : MapModule.TryFind(Root, key, Comparer<K>.Default);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public IEnumerable<V> FindSeq(K key) =>
            Find(key).AsEnumerable();

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(K key, Func<V, R> Some, Func<R> None) =>
            isnull(key)
                ? None()
                : match(MapModule.TryFind(Root, key, Comparer<K>.Default), Some, None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public Map<K, V> SetItem(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            return SetRoot(MapModule.SetItem(Root, key, value, Comparer<K>.Default));
        }

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the item isn't found</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<K, V> SetItem(K key, Func<V, V> Some) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind(Root, key, Comparer<K>.Default),
                        Some: x => SetItem(key, Some(x)),
                        None: () => raise<Map<K, V>>(new ArgumentException("Key not found in Map")));

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
        public Map<K, V> TrySetItem(K key, V value)
        {
            if (isnull(key)) return this;
            return SetRoot(MapModule.TrySetItem(Root, key, value, Comparer<K>.Default));
        }

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
        public Map<K, V> TrySetItem(K key, Func<V, V> Some) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind(Root, key, Comparer<K>.Default),
                        Some: x => SetItem(key, Some(x)),
                        None: () => this);

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
        public Map<K, V> TrySetItem(K key, Func<V, V> Some, Func<Map<K, V>, Map<K, V>> None) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind(Root, key, Comparer<K>.Default),
                        Some: x => SetItem(key, Some(x)),
                        None: () => None(this));

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
        public Map<K, V> AddOrUpdate(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            return SetRoot(MapModule.AddOrUpdate(Root, key, value, Comparer<K>.Default));
        }


        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind(Root, key, Comparer<K>.Default),
                        Some: x  => SetItem(key, Some(x)),
                        None: () => Add(key, None()));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<K, V> AddOrUpdate(K key, Func<V, V> Some, V None)
        {
            if (isnull(None)) throw new ArgumentNullException(nameof(None));

            return isnull(key)
                ? this
                : match(MapModule.TryFind(Root, key, Comparer<K>.Default),
                        Some: x => SetItem(key, Some(x)),
                        None: () => Add(key, None));
        }

        /// <summary>
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        [Pure]
        public IEnumerable<V> FindRange(K keyFrom, K keyTo)
        {
            if (isnull(keyFrom)) throw new ArgumentNullException(nameof(keyFrom));
            if (isnull(keyTo)) throw new ArgumentNullException(nameof(keyTo));
            return Comparer<K>.Default.Compare(keyFrom, keyTo) > 0
                ? MapModule.FindRange(Root, keyTo, keyFrom, Comparer<K>.Default)
                : MapModule.FindRange(Root, keyFrom, keyTo, Comparer<K>.Default);
        }

        /// <summary>
        /// Skips 'amount' values and returns a new tree without the 
        /// skipped values.
        /// </summary>
        /// <param name="amount">Amount to skip</param>
        /// <returns>New tree</returns>
        [Pure]
        public IEnumerable<IMapItem<K, V>> Skip(int amount)
        {
            var enumer = new MapModule.MapEnumerator<K, V>(Root, Rev, amount);
            while (enumer.MoveNext())
            {
                yield return enumer.Current;
            }
        }

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool ContainsKey(K key) =>
            isnull(key)
                ? false
                : Find(key)
                    ? true
                    : false;

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains(K key, V value) =>
            match(Find(key),
                Some: v => ReferenceEquals(v, value),
                None: () => false
                );

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public Map<K, V> Clear() =>
            Empty;

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            AddRange(from kv in pairs
                     select Tuple(kv.Key, kv.Value));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            if (items == null) return this;
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Key)) continue;
                self = MapModule.SetItem(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        {
            if (items == null) return this;
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = MapModule.SetItem(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Key)) continue;
                self = MapModule.TrySetItem(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> TrySetItems(IEnumerable<Tuple<K, V>> items)
        {
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = MapModule.TrySetItem(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some)
        {
            var self = this;
            foreach (var key in keys)
            {
                if (isnull(key)) continue;
                self = TrySetItem(key, Some);
            }
            return self;
        }

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New map with the items removed</returns>
        [Pure]
        public Map<K, V> RemoveRange(IEnumerable<K> keys)
        {
            var self = Root;
            foreach (var key in keys)
            {
                self = MapModule.Remove(self, key, Comparer<K>.Default);
            }
            return SetRoot(self);
        }

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) =>
            match(MapModule.TryFind(Root, pair.Key, Comparer<K>.Default),
                  Some: v => ReferenceEquals(v, pair.Value),
                  None: () => false);

        /// <summary>
        /// TryGetValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("TryGetValue is obsolete, use TryFind instead")]
        public bool TryGetValue(K key, out V value)
        {
            var res = match(Find(key),
                            Some: x => Tuple(x, true),
                            None: () => Tuple(default(V), false));
            value = res.Item1;
            return res.Item2;
        }

        /// <summary>
        /// Enumerable of map keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys
        {
            get
            {
                return from x in MapModule.AsEnumerable(this)
                       select x.Key;
            }
        }

        /// <summary>
        /// Enumerable of map values
        /// </summary>
        [Pure]
        public IEnumerable<V> Values
        {
            get
            {
                return from x in MapModule.AsEnumerable(this)
                       select x.Value;
            }
        }

        /// <summary>
        /// Convert the map to an IDictionary
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IDictionary<K, V> ToDictionary() =>
            new Dictionary<K, V>((IDictionary<K, V>)this);

        /// <summary>
        /// Map the map the a dictionary
        /// </summary>
        [Pure]
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<IMapItem<K, V>, KR> keySelector, Func<IMapItem<K, V>, VR> valueSelector) =>
            AsEnumerable().ToDictionary(x => keySelector(x), x => valueSelector(x));

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<Tuple<K, V>> Tuples =>
            AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value));

        #region IEnumerable interface
        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<IMapItem<K, V>> GetEnumerator() =>
            new MapModule.MapEnumerator<K, V>(Root, Rev, 0);

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            MapModule.AsEnumerable(this).GetEnumerator();

        public IEnumerable<IMapItem<K, V>> AsEnumerable() =>
            MapModule.AsEnumerable(this);

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() =>
            (from x in MapModule.AsEnumerable(this)
             select new KeyValuePair<K, V>(x.Key, x.Value)).GetEnumerator();
        #endregion


        internal Map<K, V> SetRoot(MapItem<K, V> root) =>
            new Map<K, V>(root, Rev);

        public bool TryGetKey(K equalKey, out K actualKey)
        {
            // TODO: Not sure of the behaviour here
            throw new NotImplementedException();
        }

        [Pure]
        public static Map<K,V> operator +(Map<K, V> lhs, Map<K, V> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public Map<K, V> Append(Map<K, V> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                if (!self.ContainsKey(item.Key))
                {
                    self = self.Add(item.Key, item.Value);
                }
            }
            return self;
        }

        [Pure]
        public static Map<K, V> operator -(Map<K, V> lhs, Map<K, V> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public Map<K, V> Subtract(Map<K, V> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item.Key);
            }
            return self;
        }
    }

    public interface IMapItem<K, V>
    {
        K Key
        {
            get;
        }

        V Value
        {
            get;
        }
    }

#if !COREFX
    [Serializable]
#endif
    class MapItem<K, V> : IMapItem<K, V>
    {
        public static readonly MapItem<K, V> Empty = new MapItem<K, V>(0, 0, default(K), default(V), null, null);

        public bool IsEmpty => Count == 0;
        public readonly int Count;
        public readonly byte Height;
        public readonly MapItem<K, V> Left;
        public readonly MapItem<K, V> Right;

        /// <summary>
        /// Ctor
        /// </summary>
        internal MapItem(byte height, int count, K key, V value, MapItem<K, V> left, MapItem<K, V> right)
        {
            Count = count;
            Height = height;
            Key = key;
            Value = value;
            Left = left;
            Right = right;
        }

        internal int BalanceFactor =>
            Count == 0 
                ? 0
                : ((int)Left.Height) - ((int)Right.Height);

        public K Key
        {
            get;
            private set;
        }

        public V Value
        {
            get;
            private set;
        }

    }

    static class MapModule
    {
        public static S Fold<S, K, V>(MapItem<K, V> node, S state, Func<S, K, V, S> folder)
        {
            if (node.IsEmpty)
            {
                return state;
            }

            state = Fold(node.Left, state, folder);
            state = folder(state, node.Key, node.Value);
            state = Fold(node.Right, state, folder);
            return state;
        }

        public static S Fold<S, K, V>(MapItem<K, V> node, S state, Func<S, V, S> folder)
        {
            if (node.IsEmpty)
            {
                return state;
            }

            state = Fold(node.Left, state, folder);
            state = folder(state, node.Value);
            state = Fold(node.Right, state, folder);
            return state;
        }

        public static MapItem<K, V> Choose<K, V>(MapItem<K, V> node, Func<K, V, Option<V>> selector) =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static MapItem<K, V> Choose<K, V>(MapItem<K, V> node, Func<V, Option<V>> selector) =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static bool ForAll<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
            node.IsEmpty
                ? true
                : pred(node.Key, node.Value)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        public static bool Exists<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
            node.IsEmpty
                ? false
                : pred(node.Key, node.Value)
                    ? true
                    : Exists(node.Left, pred) || Exists(node.Right, pred);

        public static MapItem<K, V> Filter<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
            node.IsEmpty
                ? node
                : pred(node.Key, node.Value)
                    ? Balance(Make(node.Key, node.Value, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        public static MapItem<K, V> Filter<K, V>(MapItem<K, V> node, Func<V, bool> pred) =>
            node.IsEmpty
                ? node
                : pred(node.Value)
                    ? Balance(Make(node.Key, node.Value, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        public static MapItem<K, U> Map<K, V, U>(MapItem<K, V> node, Func<V, U> mapper) =>
            node.IsEmpty
                ? MapItem<K, U>.Empty
                : new MapItem<K, U>(node.Height, node.Count, node.Key, mapper(node.Value), Map(node.Left, mapper), Map(node.Right, mapper));

        public static MapItem<K, U> Map<K, V, U>(MapItem<K, V> node, Func<K, V, U> mapper) =>
            node.IsEmpty
                ? MapItem<K,U>.Empty
                : new MapItem<K, U>(node.Height, node.Count, node.Key, mapper(node.Key, node.Value), Map(node.Left, mapper), Map(node.Right, mapper));

        public static MapItem<K, V> Add<K, V>(MapItem<K, V> node, K key, V value, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return new MapItem<K, V>(1, 1, key, value, MapItem<K, V>.Empty, MapItem<K, V>.Empty);
            }
            var cmp = comparer.Compare(key,node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, Add(node.Left, key, value, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, Add(node.Right, key, value, comparer)));
            }
            else
            {
                throw new ArgumentException("An element with the same key already exists in the Map");
            }
        }

        public static MapItem<K, V> SetItem<K, V>(MapItem<K, V> node, K key, V value, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Key not found in Map");
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, SetItem(node.Left, key, value, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, SetItem(node.Right, key, value, comparer)));
            }
            else
            {
                return new MapItem<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static MapItem<K, V> TrySetItem<K, V>(MapItem<K, V> node, K key, V value, Comparer<K> comparer) 
        {
            if (node.IsEmpty)
            {
                return node;
            }
            var cmp = comparer.Compare(key,node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, TrySetItem(node.Left, key, value, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, TrySetItem(node.Right, key, value, comparer)));
            }
            else
            {
                return new MapItem<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static MapItem<K, V> TryAdd<K, V>(MapItem<K, V> node, K key, V value, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return new MapItem<K, V>(1, 1, key, value, MapItem<K, V>.Empty, MapItem<K, V>.Empty);
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, TryAdd(node.Left, key, value, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, TryAdd(node.Right, key, value, comparer)));
            }
            else
            {
                return node;
            }
        }

        public static MapItem<K, V> AddOrUpdate<K, V>(MapItem<K, V> node, K key, V value, Comparer<K> comparer) 
        {
            if (node.IsEmpty)
            {
                return new MapItem<K, V>(1, 1, key, value, MapItem<K, V>.Empty, MapItem<K, V>.Empty);
            }
            var cmp = comparer.Compare(key,node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, AddOrUpdate(node.Left, key, value, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, AddOrUpdate(node.Right, key, value, comparer)));
            }
            else
            {
                return new MapItem<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static MapItem<K, V> AddTreeToRight<K, V>(MapItem<K, V> node, MapItem<K, V> toAdd) =>
            node.IsEmpty
                ? toAdd
                : Balance(Make(node.Key, node.Value, node.Left, AddTreeToRight(node.Right, toAdd)));

        public static MapItem<K, V> Remove<K, V>(MapItem<K, V> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return node;
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, Remove(node.Left, key, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, Remove(node.Right, key, comparer)));
            }
            else
            {
                return Balance(AddTreeToRight(node.Left, node.Right));
            }
        }

        public static V Find<K, V>(MapItem<K, V> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Key not found in Map");
            }
            var cmp = comparer.Compare(key,node.Key);
            if (cmp < 0)
            {
                return Find(node.Left, key, comparer);
            }
            else if (cmp > 0)
            {
                return Find(node.Right, key, comparer);
            }
            else
            {
                return node.Value;
            }
        }

        public static IEnumerable<IMapItem<K, V>> AsEnumerable<K, V>(Map<K, V> node)
        {
            return node;
        }

        /// <summary>
        /// TODO: I suspect this is suboptimal, it would be better with a customer Enumerator 
        /// that maintains a stack of nodes to retrace.
        /// </summary>
        public static IEnumerable<V> FindRange<K, V>(MapItem<K, V> node, K a, K b, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                yield break;
            }
            if (comparer.Compare(node.Key,a) < 0)
            {
                foreach (var item in FindRange(node.Right, a, b, comparer))
                {
                    yield return item;
                }
            }
            else if (comparer.Compare(node.Key, b) > 0)
            {
                foreach (var item in FindRange(node.Left, a, b, comparer))
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in FindRange(node.Left, a, b, comparer))
                {
                    yield return item;
                }
                yield return node.Value;
                foreach (var item in FindRange(node.Right, a, b, comparer))
                {
                    yield return item;
                }
            }
        }

        public static Option<V> TryFind<K, V>(MapItem<K, V> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return None;
            }
            var cmp = comparer.Compare(key,node.Key);
            if (cmp < 0)
            {
                return TryFind(node.Left, key, comparer);
            }
            else if (cmp > 0)
            {
                return TryFind(node.Right, key, comparer);
            }
            else
            {
                return Some(node.Value);
            }
        }

        public static MapItem<K, V> Skip<K, V>(MapItem<K, V> node, int amount)
        {
            if (amount == 0 || node.IsEmpty)
            {
                return node;
            }
            if (amount > node.Count)
            {
                return MapItem<K, V>.Empty;
            }
            if (!node.Left.IsEmpty && node.Left.Count == amount)
            {
                return Balance(Make(node.Key, node.Value, MapItem<K, V>.Empty, node.Right));
            }
            if (!node.Left.IsEmpty && node.Left.Count == amount - 1)
            {
                return node.Right;
            }
            if (node.Left.IsEmpty)
            {
                return Skip(node.Right, amount - 1);
            }

            var newleft = Skip(node.Left, amount);
            var remaining = amount - node.Left.Count - newleft.Count;
            if (remaining > 0)
            {
                return Skip(Balance(Make(node.Key, node.Value, newleft, node.Right)), remaining);
            }
            else
            {
                return Balance(Make(node.Key, node.Value, newleft, node.Right));
            }
        }

        public static MapItem<K, V> Make<K, V>(K k, V v, MapItem<K, V> l, MapItem<K, V> r) =>
            new MapItem<K, V>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, k, v, l, r);

        public static MapItem<K, V> Balance<K, V>(MapItem<K, V> node) =>
            node.BalanceFactor >= 2
                ? node.Left.BalanceFactor >= 1
                    ? RotRight(node)
                    : DblRotRight(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor <= -1
                        ? RotLeft(node)
                        : DblRotLeft(node)
                    : node;

        public static MapItem<K, V> RotRight<K, V>(MapItem<K, V> node) =>
            node.IsEmpty || node.Left.IsEmpty
                ? node
                : Make(node.Left.Key, node.Left.Value, node.Left.Left, Make(node.Key, node.Value, node.Left.Right, node.Right));

        public static MapItem<K, V> RotLeft<K, V>(MapItem<K, V> node) =>
            node.IsEmpty || node.Right.IsEmpty
                ? node
                : Make(node.Right.Key, node.Right.Value, Make(node.Key, node.Value, node.Left, node.Right.Left), node.Right.Right);

        public static MapItem<K, V> DblRotRight<K, V>(MapItem<K, V> node) =>
            node.IsEmpty
                ? node
                : RotRight(Make(node.Key, node.Value, RotLeft(node.Left), node.Right));

        public static MapItem<K, V> DblRotLeft<K, V>(MapItem<K, V> node) =>
            node.IsEmpty
                ? node
                : RotLeft(Make(node.Key, node.Value, node.Left, RotRight(node.Right)));

        public class MapEnumerator<K, V> : IEnumerator<IMapItem<K, V>>
        {
            static ObjectPool<Stack<MapItem<K, V>>> pool = new ObjectPool<Stack<MapItem<K, V>>>(32, () => new Stack<MapItem<K, V>>(32));

            Stack<MapItem<K, V>> stack;
            MapItem<K, V> map;
            int left;
            bool rev;
            int start;

            public MapEnumerator(MapItem<K, V> root, bool rev, int start)
            {
                this.rev = rev;
                this.start = start;
                map = root;
                stack = pool.GetItem();
                Reset();
            }

            private MapItem<K, V> NodeCurrent
            {
                get;
                set;
            }

            public IMapItem<K, V> Current => NodeCurrent;
            object IEnumerator.Current => NodeCurrent;

            public void Dispose()
            {
                if (stack != null)
                {
                    pool.Release(stack);
                    stack = null;
                }
            }

            private MapItem<K, V> Next(MapItem<K, V> node) =>
                rev ? node.Left : node.Right;

            private MapItem<K, V> Prev(MapItem<K, V> node) =>
                rev ? node.Right : node.Left;

            private void Push(MapItem<K, V> node)
            {
                while (!node.IsEmpty)
                {
                    stack.Push(node);
                    node = Prev(node);
                }
            }

            public bool MoveNext()
            {
                if (left > 0 && stack.Count > 0)
                {
                    NodeCurrent = stack.Pop();
                    Push(Next(NodeCurrent));
                    left--;
                    return true;
                }

                NodeCurrent = null;
                return false;
            }

            public void Reset()
            {
                var skip = rev ? map.Count - start - 1 : start;

                stack.Clear();
                NodeCurrent = map;
                left = map.Count;

                while (!NodeCurrent.IsEmpty && skip != Prev(NodeCurrent).Count)
                {
                    if (skip < Prev(NodeCurrent).Count)
                    {
                        stack.Push(NodeCurrent);
                        NodeCurrent = Prev(NodeCurrent);
                    }
                    else
                    {
                        skip -= Prev(NodeCurrent).Count + 1;
                        NodeCurrent = Next(NodeCurrent);
                    }
                }

                if (!NodeCurrent.IsEmpty)
                {
                    stack.Push(NodeCurrent);
                }
            }
        }
    }
}