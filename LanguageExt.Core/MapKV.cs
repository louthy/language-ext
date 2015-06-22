using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal enum MapTag
    {
        Node,
        Empty
    };

    /// <summary>
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// http://en.wikipedia.org/wiki/AVL_tree
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public abstract class Map<K, V> : IEnumerable<Tuple<K, V>>, IImmutableDictionary<K,V>
    {
        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        public V this[K key]
        {
            get
            {
                return Find(key).IfNone(() => failwith<V>("Key doesn't exist in map"));
            }
        }

        /// <summary>
        /// Number of items in the map
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// Alias of Count
        /// </summary>
        public int Length
        {
            get
            {
                return Count;
            } 
        }

        /// <summary>
        /// Atomically adds a new item to the map
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the key already exists</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        public Map<K, V> Add(K key, V value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return MapModule.Add(this, key, value, Comparer<K>.Default);
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
        public Map<K, V> TryAdd(K key, V value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return MapModule.TryAdd(this, key, value, Comparer<K>.Default);
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
        public Map<K, V> TryAdd(K key, V value, Func<Map<K,V>, V, Map<K, V>> Fail)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
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
        public Map<K, V> AddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                if (item.Item1 == null) throw new ArgumentNullException(nameof(item.Item1));
                if (item.Item2 == null) throw new ArgumentNullException(nameof(item.Item2));
                self = MapModule.Add(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public Map<K, V> TryAddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = this;
            foreach (var item in range)
            {
                if (item.Item1 == null) throw new ArgumentNullException(nameof(item.Item1));
                if (item.Item2 == null) throw new ArgumentNullException(nameof(item.Item2));
                self = MapModule.TryAdd(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public Map<K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = this;
            foreach (var item in range)
            {
                if (item.Key == null) throw new ArgumentNullException(nameof(item.Key));
                if (item.Value == null) throw new ArgumentNullException(nameof(item.Value));
                self = MapModule.TryAdd(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public Map<K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = this;
            foreach (var item in range)
            {
                if (item.Item1 == null) throw new ArgumentNullException(nameof(item.Item1));
                if (item.Item2 == null) throw new ArgumentNullException(nameof(item.Item2));
                self = MapModule.AddOrUpdate(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public Map<K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = this;
            foreach (var item in range)
            {
                if (item.Key == null) throw new ArgumentNullException(nameof(item.Key));
                if (item.Value == null) throw new ArgumentNullException(nameof(item.Value));
                self = MapModule.AddOrUpdate(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        public Map<K, V> Remove(K key) =>
            key == null
                ? this
                : MapModule.Remove(this, key, Comparer<K>.Default);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        public Option<V> Find(K key) =>
            key == null
                ? None
                : MapModule.TryFind(this, key, Comparer<K>.Default);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        public R Find<R>(K key, Func<V, R> Some, Func<R> None) =>
            key == null
                ? None()
                : match(MapModule.TryFind(this, key, Comparer<K>.Default), Some, None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        public Map<K, V> SetItem(K key, V value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return MapModule.SetItem(this, key, value, Comparer<K>.Default);
        }

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the item isn't found</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        public Map<K,V> SetItem(K key, Func<V, V> Some) =>
            key == null
                ? this
                : match(MapModule.TryFind(this, key, Comparer<K>.Default), 
                        Some: x  => SetItem(key, MapModule.CheckNull(Some(x),"Some delegate")), 
                        None: () => raise<Map<K,V>>(new ArgumentException("Key not found in Map")));

        /// <summary>
        /// Atomically updates an existing item, unless it doesn't exist, in which case 
        /// it is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the value is null</exception>
        /// <returns>New Map with the item added</returns>
        public Map<K, V> TrySetItem(K key, V value)
        {
            if (key == null) return this;
            if (value == null) throw new ArgumentNullException(nameof(value));
            return MapModule.TrySetItem(this, key, value, Comparer<K>.Default);
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
        public Map<K, V> TrySetItem(K key, Func<V, V> Some) =>
            key == null
                ? this
                : match(MapModule.TryFind(this, key, Comparer<K>.Default),
                        Some: x  => SetItem(key, MapModule.CheckNull(Some(x),"Some delegate")),
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
        public Map<K, V> TrySetItem(K key, Func<V, V> Some, Func<Map<K,V>, Map<K, V>> None) =>
            key == null
                ? this
                : match(MapModule.TryFind(this, key, Comparer<K>.Default),
                        Some: x  => SetItem(key, MapModule.CheckNull(Some(x),"Some delegate")),
                        None: () => MapModule.CheckNull(None(this),"None delegate"));

        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists, the new item replaces it.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        public Map<K, V> AddOrUpdate(K key, V value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return MapModule.AddOrUpdate(this, key, value, Comparer<K>.Default);
        }

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        public Map<K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) =>
            key == null
                ? this
                : match(MapModule.TryFind(this, key, Comparer<K>.Default),
                        Some: x  => SetItem(key, MapModule.CheckNull(Some(x),"Some delegate")),
                        None: () => Add(key, MapModule.CheckNull(None(),"None delegate")));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        public Map<K, V> AddOrUpdate(K key, Func<V, V> Some, V None)
        {
            if (None == null) throw new ArgumentNullException("None");

            return key == null
                ? this
                : match(MapModule.TryFind(this, key, Comparer<K>.Default),
                        Some: x => SetItem(key, MapModule.CheckNull(Some(x), "Some delegate")),
                        None: () => Add(key, None));
        }

        /// <summary>
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        public IEnumerable<V> FindRange(K keyFrom, K keyTo)
        {
            if (keyFrom == null) throw new ArgumentNullException(nameof(keyFrom));
            if (keyTo == null) throw new ArgumentNullException(nameof(keyTo));
            return Comparer<K>.Default.Compare(keyFrom,keyTo) > 0
                ? MapModule.FindRange(this, keyTo, keyFrom, Comparer<K>.Default)
                : MapModule.FindRange(this, keyFrom, keyTo, Comparer<K>.Default);
        }

        /// <summary>
        /// Skips 'amount' values and returns a new tree without the 
        /// skipped values.
        /// </summary>
        /// <param name="amount">Amount to skip</param>
        /// <returns>New tree</returns>
        public Map<K, V> Skip(int amount) =>
            MapModule.Skip(this, amount);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        public bool ContainsKey(K key) =>
            key == null
                ? false
                : Find(key)
                    ? true
                    : false;

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        public bool Contains(K key, V value) =>
            match(Find(key),
                Some: v  => ReferenceEquals(v,value),
                None: () => false
                );

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        public Map<K, V> Clear() =>
            Empty<K, V>.Default;

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        public Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            AddRange(from kv in pairs
                     select Tuple(kv.Key, kv.Value));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        public Map<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (item.Key == null) continue;
                if (item.Value == null) throw new ArgumentNullException(nameof(item.Value));
                self = MapModule.SetItem(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        public Map<K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (item.Item1 == null) continue;
                if (item.Item2 == null) throw new ArgumentNullException(nameof(item.Item2));
                self = MapModule.SetItem(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        public Map<K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                if (item.Key == null) continue;
                if (item.Value == null) throw new ArgumentNullException(nameof(item.Value));
                self = MapModule.TrySetItem(self, item.Key, item.Value, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        public Map<K, V> TrySetItems(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                if (item.Item1 == null) continue;
                if (item.Item2 == null) throw new ArgumentNullException(nameof(item.Item2));
                self = MapModule.TrySetItem(self, item.Item1, item.Item2, Comparer<K>.Default);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <returns>New map with the items set</returns>
        public Map<K, V> TrySetItems(IEnumerable<K> keys, Func<V,V> Some)
        {
            var self = this;
            foreach (var key in keys)
            {
                if (key == null) continue;
                self = TrySetItem(key, Some);
            }
            return self;
        }

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New map with the items removed</returns>
        public Map<K, V> RemoveRange(IEnumerable<K> keys)
        {
            var self = this;
            foreach (var key in keys)
            {
                self = MapModule.Remove(self, key, Comparer<K>.Default);
            }
            return self;
        }
        
        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        public bool Contains(KeyValuePair<K, V> pair) =>
            match(MapModule.TryFind(this, pair.Key, Comparer<K>.Default),
                  Some: v => ReferenceEquals(v, pair.Value),
                  None: () => false);

        /// <summary>
        /// TryGetValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
        public IEnumerable<K> Keys
        {
            get
            {
                return from x in MapModule.ToEnumerable(this)
                       select x.Item1;
            }
        }

        /// <summary>
        /// Enumerable of map values
        /// </summary>
        public IEnumerable<V> Values
        {
            get
            {
                return from x in MapModule.ToEnumerable(this)
                       select x.Item2;
            }
        }

        public IDictionary<K, V> ToDictionary() =>
            new Dictionary<K, V>((IDictionary<K,V>)this);

        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<Tuple<K,V>, KR> keySelector, Func<Tuple<K, V>, VR> valueSelector) =>
            AsEnumerable().ToDictionary(x => keySelector(x), x => valueSelector(x));

        public IImmutableDictionary<K, V> ToImmutableDictionary() =>
            ImmutableDictionary.CreateRange<K, V>(from x in AsEnumerable()
                                                  select new KeyValuePair<K, V>(x.Item1,x.Item2));
        #region IEnumerable interface
        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<Tuple<K, V>> GetEnumerator() =>
            MapModule.ToEnumerable(this).GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            MapModule.ToEnumerable(this).GetEnumerator();

        public IEnumerable<Tuple<K, V>> AsEnumerable() =>
            MapModule.ToEnumerable(this);

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() =>
            (from x in MapModule.ToEnumerable(this)
             select new KeyValuePair<K, V>(x.Item1, x.Item2)).GetEnumerator();
        #endregion

        #region IImmutableDictionary interface
        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Add(K key, V value) =>
            MapModule.Add(this, key, value, Comparer<K>.Default);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItem(K key, V value) =>
            MapModule.SetItem(this, key, value, Comparer<K>.Default);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Remove(K key) =>
            MapModule.Remove(this, key, Comparer<K>.Default);

        public bool TryGetKey(K equalKey, out K actualKey)
        {
            // TODO: Not sure of the behaviour here
            throw new NotImplementedException();
        }

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Clear() =>
            Clear();

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            AddRange(pairs);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItems(IEnumerable<KeyValuePair<K, V>> items) =>
            SetItems(items);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.RemoveRange(IEnumerable<K> keys) =>
            RemoveRange(keys);
        #endregion

        #region Internal
        internal abstract byte Height
        {
            get;
        }

        internal abstract int BalanceFactor
        {
            get;
        }

        internal abstract K Key
        {
            get;
        }

        internal abstract V Value
        {
            get;
        }

        internal abstract MapTag Tag
        {
            get;
        }

        internal abstract Map<K, V> Left
        {
            get;
        }

        internal abstract Map<K, V> Right
        {
            get;
        }

        #endregion
    }

    internal static class MapModule
    {
        public static S Fold<S, K, V>(Map<K, V> node, S state, Func<S, K, V, S> folder)
        {
            if (node.Tag == MapTag.Empty)
            {
                return state;
            }

            state = Fold(node.Left, state, folder);
            state = folder(state, node.Key, node.Value);
            state = Fold(node.Right, state, folder);
            return state;
        }

        public static S Fold<S, K, V>(Map<K, V> node, S state, Func<S, V, S> folder)
        {
            if (node.Tag == MapTag.Empty)
            {
                return state;
            }

            state = Fold(node.Left, state, folder);
            state = folder(state, node.Value);
            state = Fold(node.Right, state, folder);
            return state;
        }

        public static S Fold<S, K, V>(Map<K, V> node, S state, Func<S, K, S> folder)
        {
            if (node.Tag == MapTag.Empty)
            {
                return state;
            }

            state = Fold(node.Left, state, folder);
            state = folder(state, node.Key);
            state = Fold(node.Right, state, folder);
            return state;
        }

        public static Map<K, V> Choose<K, V>(Map<K, V> node, Func<K, V, Option<V>> selector) =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static Map<K, V> Choose<K, V>(Map<K, V> node, Func<V, Option<V>> selector) =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static Unit Iter<K, V>(Map<K, V> node, Action<K, V> action) 
        {
            if (node.Tag == MapTag.Empty)
            {
                return unit;
            }
            Iter(node.Left, action);
            action(node.Key, node.Value);
            Iter(node.Right, action);
            return unit;
        }

        public static Unit Iter<K, V>(Map<K, V> node, Action<V> action) 
        {
            if (node.Tag == MapTag.Empty)
            {
                return unit;
            }
            Iter(node.Left, action);
            action(node.Value);
            Iter(node.Right, action);
            return unit;
        }

        public static bool ForAll<K, V>(Map<K, V> node, Func<K, V, bool> pred) =>
            node.Tag == MapTag.Empty
                ? true
                : pred(node.Key, node.Value)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        public static bool Exists<K, V>(Map<K, V> node, Func<K, V, bool> pred) =>
            node.Tag == MapTag.Empty
                ? false
                : pred(node.Key, node.Value)
                    ? true
                    : ForAll(node.Left, pred) || ForAll(node.Right, pred);

        public static Map<K, V> Filter<K, V>(Map<K, V> node, Func<K, V, bool> pred) =>
            node.Tag == MapTag.Empty
                ? node
                : pred(node.Key, node.Value)
                    ? Balance(Make(node.Key, node.Value, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        public static Map<K, V> Filter<K, V>(Map<K, V> node, Func<K, bool> pred) =>
            node.Tag == MapTag.Empty
                ? node
                : pred(node.Key)
                    ? Balance(Make(node.Key, node.Value, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        public static Map<K, V> Filter<K, V>(Map<K, V> node, Func<V, bool> pred) =>
            node.Tag == MapTag.Empty
                ? node
                : pred(node.Value)
                    ? Balance(Make(node.Key, node.Value, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        internal static T CheckNull<T>(T value, string context) =>
            value == null
                ? failwith<T>("Null result not allowed in " + context)
                : value;

        public static Map<K, U> Map<K, V, U>(Map<K, V> node, Func<V, U> mapper) =>
            node.Tag == MapTag.Empty
                ? Empty<K, U>.Default
                : new Node<K, U>(node.Height, node.Count, node.Key, CheckNull(mapper(node.Value),"map delegate"), Map(node.Left, mapper), Map(node.Right, mapper));

        public static Map<K, U> Map<K, V, U>(Map<K, V> node, Func<K, V, U> mapper) =>
            node.Tag == MapTag.Empty
                ? Empty<K, U>.Default
                : new Node<K, U>(node.Height, node.Count, node.Key, CheckNull(mapper(node.Key, node.Value), "map delegate"), Map(node.Left, mapper), Map(node.Right, mapper));

        public static Map<K, V> Add<K, V>(Map<K, V> node, K key, V value, Comparer<K> comparer)
        {
            if (node.Tag == MapTag.Empty)
            {
                return new Node<K, V>(1, 1, key, value, Empty<K, V>.Default, Empty<K, V>.Default);
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

        public static Map<K, V> SetItem<K, V>(Map<K, V> node, K key, V value, Comparer<K> comparer)
        {
            if (node.Tag == MapTag.Empty)
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
                return new Node<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static Map<K, V> TrySetItem<K, V>(Map<K, V> node, K key, V value, Comparer<K> comparer) 
        {
            if (node.Tag == MapTag.Empty)
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
                return new Node<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static Map<K, V> TryAdd<K, V>(Map<K, V> node, K key, V value, Comparer<K> comparer)
        {
            if (node.Tag == MapTag.Empty)
            {
                return new Node<K, V>(1, 1, key, value, Empty<K, V>.Default, Empty<K, V>.Default);
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

        public static Map<K, V> AddOrUpdate<K, V>(Map<K, V> node, K key, V value, Comparer<K> comparer) 
        {
            if (node.Tag == MapTag.Empty)
            {
                return new Node<K, V>(1, 1, key, value, Empty<K, V>.Default, Empty<K, V>.Default);
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
                return new Node<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static Map<K, V> AddTreeToRight<K, V>(Map<K, V> node, Map<K, V> toAdd) =>
            node.Tag == MapTag.Empty
                ? toAdd
                : Balance(Make(node.Key, node.Value, node.Left, AddTreeToRight(node.Right, toAdd)));

        public static Map<K, V> Remove<K, V>(Map<K, V> node, K key, Comparer<K> comparer)
        {
            if (node.Tag == MapTag.Empty)
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

        public static V Find<K, V>(Map<K, V> node, K key, Comparer<K> comparer)
        {
            if (node.Tag == MapTag.Empty)
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

        public static IEnumerable<Tuple<K, V>> ToEnumerable<K, V>(Map<K, V> node)
        {
            if (node.Tag == MapTag.Empty)
            {
                yield break;
            }
            foreach (var item in ToEnumerable(node.Left))
            {
                yield return item;
            }
            yield return Tuple(node.Key, node.Value);
            foreach (var item in ToEnumerable(node.Right))
            {
                yield return item;
            }
        }

        /// <summary>
        /// TODO: I suspect this is suboptimal, it would be better with a customer Enumerator 
        /// that maintains a stack of nodes to retrace.
        /// </summary>
        public static IEnumerable<V> FindRange<K, V>(Map<K, V> node, K a, K b, Comparer<K> comparer)
        {
            if (node.Tag == MapTag.Empty)
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

        public static Option<V> TryFind<K, V>(Map<K, V> node, K key, Comparer<K> comparer)
        {
            if (node.Tag == MapTag.Empty)
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

        public static Map<K, V> Skip<K, V>(Map<K, V> node, int amount)
        {
            if (amount == 0 || node.Tag == MapTag.Empty)
            {
                return node;
            }
            if (amount > node.Count)
            {
                return Empty<K, V>.Default;
            }
            if (node.Left.Tag != MapTag.Empty && node.Left.Count == amount)
            {
                return Balance(Make(node.Key, node.Value, Empty<K, V>.Default, node.Right));
            }
            if (node.Left.Tag != MapTag.Empty && node.Left.Count == amount - 1)
            {
                return node.Right;
            }
            if (node.Left.Tag == MapTag.Empty)
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

        public static Map<K, V> Make<K, V>(K k, V v, Map<K, V> l, Map<K, V> r) =>
            new Node<K, V>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, k, v, l, r);

        public static Map<K, V> Balance<K, V>(Map<K, V> node) =>
            node.BalanceFactor >= 2
                ? node.Left.BalanceFactor >= 1
                    ? RotRight(node)
                    : DblRotRight(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor <= -1
                        ? RotLeft(node)
                        : DblRotLeft(node)
                    : node;

        public static Map<K, V> RotRight<K, V>(Map<K, V> node) =>
            node.Tag == MapTag.Empty || node.Left.Tag == MapTag.Empty
                ? node
                : Make(node.Left.Key, node.Left.Value, node.Left.Left, Make(node.Key, node.Value, node.Left.Right, node.Right));

        public static Map<K, V> RotLeft<K, V>(Map<K, V> node) =>
            node.Tag == MapTag.Empty || node.Right.Tag == MapTag.Empty
                ? node
                : Make(node.Right.Key, node.Right.Value, Make(node.Key, node.Value, node.Left, node.Right.Left), node.Right.Right);

        public static Map<K, V> DblRotRight<K, V>(Map<K, V> node) =>
            node.Tag == MapTag.Empty
                ? node
                : RotRight(Make(node.Key, node.Value, RotLeft(node.Left), node.Right));

        public static Map<K, V> DblRotLeft<K, V>(Map<K, V> node) =>
            node.Tag == MapTag.Empty
                ? node
                : RotLeft(Make(node.Key, node.Value, node.Left, RotRight(node.Right)));
    }

    public class Empty<K, V> : Map<K, V>
    {
        public static readonly Map<K, V> Default = new Empty<K, V>();

        public override string ToString() =>
            "()";

        public override int Count
        {
            get
            {
                return 0;
            }
        }

        internal override byte Height
        {
            get
            {
                return 0;
            }
        }

        internal override int BalanceFactor
        {
            get
            {
                return 0;
            }
        }

        internal override MapTag Tag
        {
            get
            {
                return MapTag.Empty;
            }
        }

        internal override K Key
        {
            get
            {
                throw new NotSupportedException("Accessed the Key property of an Empty Map node");
            }
        }

        internal override V Value
        {
            get
            {
                throw new NotSupportedException("Accessed the Value property of an Empty Map node");
            }
        }

        internal override Map<K, V> Left
        {
            get
            {
                return Default;
            }
        }

        internal override Map<K, V> Right
        {
            get
            {
                return Default;
            }
        }
    }

    public class Node<K, V> : Map<K, V>
    {
        readonly int count;
        readonly byte height;

        public readonly K key;
        public readonly V value;
        public readonly Map<K, V> left;
        public readonly Map<K, V> right;

        internal Node(byte h, int c, K key, V value, Map<K, V> left, Map<K, V> right)
        {
            this.height = h;
            this.count = c;
            this.key = key;
            this.value = value;
            this.left = left;
            this.right = right;
        }

        public override string ToString() =>
            String.Format("({0},{1})", Key, Value);

        public override int Count
        {
            get
            {
                return count;
            }
        }

        internal override byte Height
        {
            get
            {
                return height;
            }
        }

        internal override int BalanceFactor
        {
            get
            {
                return ((int)Left.Height) - ((int)Right.Height);
            }
        }

        internal override MapTag Tag
        {
            get
            {
                return MapTag.Node;
            }
        }

        internal override Map<K, V> Left
        {
            get
            {
                return left;
            }
        }

        internal override Map<K, V> Right
        {
            get
            {
                return right;
            }
        }

        internal override K Key
        {
            get
            {
                return key;
            }
        }

        internal override V Value
        {
            get
            {
                return value;
            }
        }
    }
}