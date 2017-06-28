using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using LanguageExt.TypeClasses;

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
    internal class MapInternal<OrdK, K, V> :
        IEnumerable<(K Key, V Value)>,
        IReadOnlyDictionary<K, V>
        where OrdK : struct, Ord<K>
    {
        public static readonly MapInternal<OrdK, K, V> Empty = new MapInternal<OrdK, K, V>(MapItem<K, V>.Empty, false);

        internal readonly MapItem<K, V> Root;
        internal readonly bool Rev;
        int hashCode;

        /// <summary>
        /// Ctor
        /// </summary>
        internal MapInternal()
        {
            Root = MapItem<K, V>.Empty;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal MapInternal(MapItem<K, V> root, bool rev)
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
        /// Get the hash code of all items in the map
        /// </summary>
        public override int GetHashCode()
        {
            if (hashCode != 0) return hashCode;
            return hashCode = hash(AsEnumerable());
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
        [Pure]
        public MapInternal<OrdK, K, V> Add(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            return SetRoot(MapModule.Add<OrdK, K, V>(Root, key, value));
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
        public MapInternal<OrdK, K, V> TryAdd(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            return SetRoot(MapModule.TryAdd<OrdK, K, V>(Root, key, value));
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
        public MapInternal<OrdK, K, V> TryAdd(K key, V value, Func<MapInternal<OrdK, K, V>, V, MapInternal<OrdK, K, V>> Fail)
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
        public MapInternal<OrdK, K, V> AddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.Add<OrdK, K, V>(self, item.Item1, item.Item2);
            }
            return SetRoot(self);
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
        public MapInternal<OrdK, K, V> AddRange(IEnumerable<(K, V)> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.Add<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> TryAddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.TryAdd<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> TryAddRange(IEnumerable<(K, V)> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.TryAdd<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Key)) throw new ArgumentNullException(nameof(item.Key));
                self = MapModule.TryAdd<OrdK, K, V>(self, item.Key, item.Value);
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
        public MapInternal<OrdK, K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.AddOrUpdate<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> AddOrUpdateRange(IEnumerable<(K, V)> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Item1)) throw new ArgumentNullException(nameof(item.Item1));
                self = MapModule.AddOrUpdate<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }

            var self = Root;
            foreach (var item in range)
            {
                if (isnull(item.Key)) throw new ArgumentNullException(nameof(item.Key));
                self = MapModule.AddOrUpdate<OrdK, K, V>(self, item.Key, item.Value);
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
        public MapInternal<OrdK, K, V> Remove(K key) =>
            isnull(key)
                ? this
                : SetRoot(MapModule.Remove<OrdK, K, V>(Root, key));

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<V> Find(K key) =>
            isnull(key)
                ? None
                : MapModule.TryFind<OrdK, K, V>(Root, key);

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
                : match(MapModule.TryFind<OrdK, K, V>(Root, key), Some, None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public MapInternal<OrdK, K, V> SetItem(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            return SetRoot(MapModule.SetItem<OrdK, K, V>(Root, key, value));
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
        public MapInternal<OrdK, K, V> SetItem(K key, Func<V, V> Some) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind<OrdK, K, V>(Root, key),
                        Some: x  => SetItem(key, Some(x)),
                        None: () => throw new ArgumentException("Key not found in Map"));

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
        public MapInternal<OrdK, K, V> TrySetItem(K key, V value)
        {
            if (isnull(key)) return this;
            return SetRoot(MapModule.TrySetItem<OrdK, K, V>(Root, key, value));
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
        public MapInternal<OrdK, K, V> TrySetItem(K key, Func<V, V> Some) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind<OrdK, K, V>(Root, key),
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
        public MapInternal<OrdK, K, V> TrySetItem(K key, Func<V, V> Some, Func<Map<K, V>, Map<K, V>> None) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind<OrdK, K, V>(Root, key),
                        Some: x => SetItem(key, Some(x)),
                        None: () => this);

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
        public MapInternal<OrdK, K, V> AddOrUpdate(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            return SetRoot(MapModule.AddOrUpdate<OrdK, K, V>(Root, key, value));
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
        public MapInternal<OrdK, K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) =>
            isnull(key)
                ? this
                : match(MapModule.TryFind<OrdK, K, V>(Root, key),
                        Some: x => SetItem(key, Some(x)),
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
        public MapInternal<OrdK, K, V> AddOrUpdate(K key, Func<V, V> Some, V None)
        {
            if (isnull(None)) throw new ArgumentNullException(nameof(None));

            return isnull(key)
                ? this
                : match(MapModule.TryFind<OrdK, K, V>(Root, key),
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
            return default(OrdK).Compare(keyFrom, keyTo) > 0
                ? MapModule.FindRange<OrdK, K, V>(Root, keyTo, keyFrom)
                : MapModule.FindRange<OrdK, K, V>(Root, keyFrom, keyTo);
        }

        /// <summary>
        /// Skips 'amount' values and returns a new tree without the 
        /// skipped values.
        /// </summary>
        /// <param name="amount">Amount to skip</param>
        /// <returns>New tree</returns>
        [Pure]
        public IEnumerable<(K Key, V Value)> Skip(int amount)
        {
            var enumer = new MapEnumerator<K, V>(Root, Rev, amount);
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
        public MapInternal<OrdK, K, V> Clear() =>
            Empty;

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public MapInternal<OrdK, K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            AddRange(from kv in pairs
                     select Tuple(kv.Key, kv.Value));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public MapInternal<OrdK, K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            if (items == null) return this;
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Key)) continue;
                self = MapModule.SetItem<OrdK, K, V>(self, item.Key, item.Value);
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
        public MapInternal<OrdK, K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        {
            if (items == null) return this;
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = MapModule.SetItem<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> SetItems(IEnumerable<(K, V)> items)
        {
            if (items == null) return this;
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = MapModule.SetItem<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Key)) continue;
                self = MapModule.TrySetItem<OrdK, K, V>(self, item.Key, item.Value);
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
        public MapInternal<OrdK, K, V> TrySetItems(IEnumerable<Tuple<K, V>> items)
        {
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = MapModule.TrySetItem<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> TrySetItems(IEnumerable<(K, V)> items)
        {
            var self = Root;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = MapModule.TrySetItem<OrdK, K, V>(self, item.Item1, item.Item2);
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
        public MapInternal<OrdK, K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some)
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
        public MapInternal<OrdK, K, V> RemoveRange(IEnumerable<K> keys)
        {
            var self = Root;
            foreach (var key in keys)
            {
                self = MapModule.Remove<OrdK, K, V>(self, key);
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
            match(MapModule.TryFind<OrdK, K, V>(Root, pair.Key),
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
                var iter = new MapKeyEnumerator<K, V>(Root, Rev, 0);
                while (iter.MoveNext())
                {
                    yield return iter.Current;
                }
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
                var iter = new MapValueEnumerator<K, V>(Root, Rev, 0);
                while(iter.MoveNext())
                {
                    yield return iter.Current;
                }
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
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<(K Key, V Value), KR> keySelector, Func<(K Key, V Value), VR> valueSelector) =>
            AsEnumerable().ToDictionary(keySelector, valueSelector);

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<Tuple<K, V>> Tuples =>
            AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value));

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<(K Key, V Value)> ValueTuples =>
            AsEnumerable().Map(kv => (kv.Key, kv.Value));

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [Pure]
        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            new MapEnumerator<K, V>(Root, Rev, 0);

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new MapEnumerator<K, V>(Root, Rev, 0);

        [Pure]
        public Seq<(K Key, V Value)> ToSeq() =>
            Seq(AsEnumerable());

        [Pure]
        public IEnumerable<(K Key, V Value)> AsEnumerable()
        {
            var iter = new MapEnumerator<K, V>(Root, Rev, 0);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() =>
            AsEnumerable().Map(ToKeyValuePair).GetEnumerator();

        static KeyValuePair<K, V> ToKeyValuePair((K Key, V Value) kv) => new KeyValuePair<K, V>(kv.Key, kv.Value);

        internal MapInternal<OrdK, K, V> SetRoot(MapItem<K, V> root) =>
            new MapInternal<OrdK, K, V>(root, Rev);

        [Pure]
        public static MapInternal<OrdK, K, V> operator +(MapInternal<OrdK, K, V> lhs, MapInternal<OrdK, K, V> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public MapInternal<OrdK, K, V> Append(MapInternal<OrdK, K, V> rhs)
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
        public static MapInternal<OrdK, K, V> operator -(MapInternal<OrdK, K, V> lhs, MapInternal<OrdK, K, V> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public MapInternal<OrdK, K, V> Subtract(MapInternal<OrdK, K, V> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item.Key);
            }
            return self;
        }

        [Pure]
        public override bool Equals(object obj) =>
            Equals(obj as MapInternal<OrdK, K, V>);

        [Pure]
        public static bool operator ==(MapInternal<OrdK, K, V> lhs, MapInternal<OrdK, K, V> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(MapInternal<OrdK, K, V> lhs, MapInternal<OrdK, K, V> rhs) =>
            !(lhs == rhs);

        [Pure]
        public bool Equals(MapInternal<OrdK, K, V> rhs)
        {
            if (ReferenceEquals(this, rhs)) return true;
            if (Count != rhs.Count) return false;
            if (hashCode != 0 && rhs.hashCode != 0 && hashCode != rhs.hashCode) return false;

            var iterA = GetEnumerator();
            var iterB = rhs.GetEnumerator();
            var count = Count;

            for (int i = 0; i < count; i++)
            {
                iterA.MoveNext();
                iterB.MoveNext();
                if (!default(OrdK).Equals(iterA.Current.Key, iterB.Current.Key)) return false;
                if (!EqualityComparer<V>.Default.Equals(iterA.Current.Value, iterB.Current.Value)) return false;
            }
            return true;
        }
    }

    internal interface IMapItem<K, V>
    {
        (K Key, V Value) KeyValue
        {
            get;
        }
    }

    [Serializable]
    class MapItem<K, V> :
        ISerializable,
        IMapItem<K, V>
    {
        internal static readonly MapItem<K, V> Empty = new MapItem<K, V>(0, 0, (default(K), default(V)), null, null);

        internal bool IsEmpty => Count == 0;
        internal readonly int Count;
        internal readonly byte Height;
        internal readonly MapItem<K, V> Left;
        internal readonly MapItem<K, V> Right;

        /// <summary>
        /// Ctor
        /// </summary>
        internal MapItem(byte height, int count, (K Key, V Value) keyValue, MapItem<K, V> left, MapItem<K, V> right)
        {
            Count = count;
            Height = height;
            KeyValue = keyValue;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Deserialisation constructor
        /// </summary>
        internal MapItem(SerializationInfo info, StreamingContext context)
        {
            var key = (K)info.GetValue("Key", typeof(K));
            var value = (V)info.GetValue("Value", typeof(V));
            KeyValue = (key, value);
            Count = 1;
            Height = 1;
            Left = Empty;
            Right = Empty;
        }

        /// <summary>
        /// Serialisation support
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", KeyValue.Key, typeof(K));
            info.AddValue("Value", KeyValue.Value, typeof(V));
        }

        internal int BalanceFactor =>
            Count == 0
                ? 0
                : ((int)Left.Height) - ((int)Right.Height);

        public (K Key, V Value) KeyValue
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
            state = folder(state, node.KeyValue.Key, node.KeyValue.Value);
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
            state = folder(state, node.KeyValue.Value);
            state = Fold(node.Right, state, folder);
            return state;
        }

        public static MapItem<K, U> Choose<K, V, U>(MapItem<K, V> node, Func<K, V, Option<U>> selector) =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static MapItem<K, U> Choose<K, V, U>(MapItem<K, V> node, Func<V, Option<U>> selector) =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static bool ForAll<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
            node.IsEmpty
                ? true
                : pred(node.KeyValue.Key, node.KeyValue.Value)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        public static bool Exists<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
            node.IsEmpty
                ? false
                : pred(node.KeyValue.Key, node.KeyValue.Value)
                    ? true
                    : Exists(node.Left, pred) || Exists(node.Right, pred);

        public static MapItem<K, V> Filter<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
            node.IsEmpty
                ? node
                : pred(node.KeyValue.Key, node.KeyValue.Value)
                    ? Balance(Make(node.KeyValue, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        public static MapItem<K, V> Filter<K, V>(MapItem<K, V> node, Func<V, bool> pred) =>
            node.IsEmpty
                ? node
                : pred(node.KeyValue.Value)
                    ? Balance(Make(node.KeyValue, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        public static MapItem<K, U> Map<K, V, U>(MapItem<K, V> node, Func<V, U> mapper) =>
            node.IsEmpty
                ? MapItem<K, U>.Empty
                : new MapItem<K, U>(node.Height, node.Count, (node.KeyValue.Key, mapper(node.KeyValue.Value)), Map(node.Left, mapper), Map(node.Right, mapper));

        public static MapItem<K, U> Map<K, V, U>(MapItem<K, V> node, Func<K, V, U> mapper) =>
            node.IsEmpty
                ? MapItem<K, U>.Empty
                : new MapItem<K, U>(node.Height, node.Count, (node.KeyValue.Key, mapper(node.KeyValue.Key, node.KeyValue.Value)), Map(node.Left, mapper), Map(node.Right, mapper));

        public static MapItem<K, V> Add<OrdK, K, V>(MapItem<K, V> node, K key, V value)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return new MapItem<K, V>(1, 1, (key, value), MapItem<K, V>.Empty, MapItem<K, V>.Empty);
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.KeyValue, Add<OrdK, K, V>(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.KeyValue, node.Left, Add<OrdK, K, V>(node.Right, key, value)));
            }
            else
            {
                throw new ArgumentException("An element with the same key already exists in the Map");
            }
        }

        public static MapItem<K, V> SetItem<OrdK, K, V>(MapItem<K, V> node, K key, V value)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Key not found in Map");
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.KeyValue, SetItem<OrdK, K, V>(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.KeyValue, node.Left, SetItem<OrdK, K, V>(node.Right, key, value)));
            }
            else
            {
                return new MapItem<K, V>(node.Height, node.Count, (node.KeyValue.Key, value), node.Left, node.Right);
            }
        }

        public static MapItem<K, V> TrySetItem<OrdK, K, V>(MapItem<K, V> node, K key, V value)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return node;
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.KeyValue, TrySetItem<OrdK, K, V>(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.KeyValue, node.Left, TrySetItem<OrdK, K, V>(node.Right, key, value)));
            }
            else
            {
                return new MapItem<K, V>(node.Height, node.Count, (node.KeyValue.Key, value), node.Left, node.Right);
            }
        }

        public static MapItem<K, V> TryAdd<OrdK, K, V>(MapItem<K, V> node, K key, V value)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return new MapItem<K, V>(1, 1, (key, value), MapItem<K, V>.Empty, MapItem<K, V>.Empty);
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.KeyValue, TryAdd<OrdK, K, V>(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.KeyValue, node.Left, TryAdd<OrdK, K, V>(node.Right, key, value)));
            }
            else
            {
                return node;
            }
        }

        public static MapItem<K, V> AddOrUpdate<OrdK, K, V>(MapItem<K, V> node, K key, V value)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return new MapItem<K, V>(1, 1, (key, value), MapItem<K, V>.Empty, MapItem<K, V>.Empty);
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.KeyValue, AddOrUpdate<OrdK, K, V>(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.KeyValue, node.Left, AddOrUpdate<OrdK, K, V>(node.Right, key, value)));
            }
            else
            {
                return new MapItem<K, V>(node.Height, node.Count, (node.KeyValue.Key, value), node.Left, node.Right);
            }
        }

        public static MapItem<K, V> AddTreeToRight<K, V>(MapItem<K, V> node, MapItem<K, V> toAdd) =>
            node.IsEmpty
                ? toAdd
                : Balance(Make(node.KeyValue, node.Left, AddTreeToRight(node.Right, toAdd)));

        public static MapItem<K, V> Remove<OrdK, K, V>(MapItem<K, V> node, K key)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return node;
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.KeyValue, Remove<OrdK, K, V>(node.Left, key), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.KeyValue, node.Left, Remove<OrdK, K, V>(node.Right, key)));
            }
            else
            {
                return Balance(AddTreeToRight(node.Left, node.Right));
            }
        }

        public static V Find<OrdK, K, V>(MapItem<K, V> node, K key)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Key not found in Map");
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return Find<OrdK, K, V>(node.Left, key);
            }
            else if (cmp > 0)
            {
                return Find<OrdK, K, V>(node.Right, key);
            }
            else
            {
                return node.KeyValue.Value;
            }
        }

        /// <summary>
        /// TODO: I suspect this is suboptimal, it would be better with a custom Enumerator 
        /// that maintains a stack of nodes to retrace.
        /// </summary>
        public static IEnumerable<V> FindRange<OrdK, K, V>(MapItem<K, V> node, K a, K b)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                yield break;
            }
            if (default(OrdK).Compare(node.KeyValue.Key, a) < 0)
            {
                foreach (var item in FindRange<OrdK, K, V>(node.Right, a, b))
                {
                    yield return item;
                }
            }
            else if (default(OrdK).Compare(node.KeyValue.Key, b) > 0)
            {
                foreach (var item in FindRange<OrdK, K, V>(node.Left, a, b))
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in FindRange<OrdK, K, V>(node.Left, a, b))
                {
                    yield return item;
                }
                yield return node.KeyValue.Value;
                foreach (var item in FindRange<OrdK, K, V>(node.Right, a, b))
                {
                    yield return item;
                }
            }
        }

        public static Option<V> TryFind<OrdK, K, V>(MapItem<K, V> node, K key)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return None;
            }
            var cmp = default(OrdK).Compare(key, node.KeyValue.Key);
            if (cmp < 0)
            {
                return TryFind<OrdK, K, V>(node.Left, key);
            }
            else if (cmp > 0)
            {
                return TryFind<OrdK, K, V>(node.Right, key);
            }
            else
            {
                return Some(node.KeyValue.Value);
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
                return Balance(Make(node.KeyValue, MapItem<K, V>.Empty, node.Right));
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
                return Skip(Balance(Make(node.KeyValue, newleft, node.Right)), remaining);
            }
            else
            {
                return Balance(Make(node.KeyValue, newleft, node.Right));
            }
        }

        public static MapItem<K, V> Make<K, V>((K,V) kv, MapItem<K, V> l, MapItem<K, V> r) =>
            new MapItem<K, V>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, kv, l, r);

        public static MapItem<K, V> Make<K, V>(K k, V v, MapItem<K, V> l, MapItem<K, V> r) =>
            new MapItem<K, V>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, (k, v), l, r);

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
                : Make(node.Left.KeyValue, node.Left.Left, Make(node.KeyValue, node.Left.Right, node.Right));

        public static MapItem<K, V> RotLeft<K, V>(MapItem<K, V> node) =>
            node.IsEmpty || node.Right.IsEmpty
                ? node
                : Make(node.Right.KeyValue, Make(node.KeyValue, node.Left, node.Right.Left), node.Right.Right);

        public static MapItem<K, V> DblRotRight<K, V>(MapItem<K, V> node) =>
            node.IsEmpty
                ? node
                : RotRight(Make(node.KeyValue, RotLeft(node.Left), node.Right));

        public static MapItem<K, V> DblRotLeft<K, V>(MapItem<K, V> node) =>
            node.IsEmpty
                ? node
                : RotLeft(Make(node.KeyValue, node.Left, RotRight(node.Right)));
    }
}
