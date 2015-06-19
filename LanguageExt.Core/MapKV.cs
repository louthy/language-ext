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
    public abstract class Map<K, V> : IEnumerable<Tuple<K, V>>, IImmutableDictionary<K,V> where K : IComparable<K>
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
        /// Length (alias for Count)
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
            return MapModule.Add(this, key, value);
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
            return MapModule.TryAdd(this, key, value);
        }

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
            return MapModule.AddOrUpdate(this, key, value);
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
                self = MapModule.Add(self, item.Item1, item.Item2);
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
                self = MapModule.TryAdd(self, item.Item1, item.Item2);
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
                self = MapModule.TryAdd(self, item.Key, item.Value);
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
                self = MapModule.AddOrUpdate(self, item.Item1, item.Item2);
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
                self = MapModule.AddOrUpdate(self, item.Key, item.Value);
            }
            return self;
        }

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
            return MapModule.SetItem(this, key, value);
        }

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        public Try<Map<K, V>> TrySetItem(K key, V value) => () =>
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return MapModule.SetItem(this, key, value);
        };

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        public Map<K, V> Remove(K key) =>
            key == null
                ? this
                : MapModule.Remove(this, key);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Found value</returns>
        public Option<V> Find(K key) =>
            key == null
                ? None
                : MapModule.TryFind(this, key);

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
            return  keyFrom.CompareTo(keyTo) > 0
                ? MapModule.FindRange(this, keyTo, keyFrom)
                : MapModule.FindRange(this, keyFrom, keyTo);
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
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        public Map<K, U> Select<U>(Func<V, U> mapper) =>
            MapModule.Map(this, mapper);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        public Map<K, U> Select<U>(Func<K, V, U> mapper) =>
            MapModule.Map(this, mapper);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        public Map<K, V> Filter(Func<V, bool> pred) =>
            MapModule.Filter(this, pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool ForAll(Func<K, V, bool> pred) =>
            MapModule.ForAll(this, pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool ForAll(Func<Tuple<K, V>, bool> pred) =>
            MapModule.ForAll(this, (k,v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool ForAll(Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.ForAll(this, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool Exists(Func<K, V, bool> pred) =>
            MapModule.Exists(this, pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool Exists(Func<Tuple<K, V>, bool> pred) =>
            MapModule.Exists(this, (k, v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool Exists(Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.Exists(this, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<K, V> action) =>
            MapModule.Iter(this, action);

        /// <summary>
        /// Atomically iterate through all values in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<V> action) =>
            MapModule.Iter(this, action);

        /// <summary>
        /// Atomically iterate through all key/value pairs (as tuples) in the map (in order) 
        /// and execute an action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<Tuple<K, V>> action) =>
            MapModule.Iter(this, (k, v) => action(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public Unit Iter(Action<KeyValuePair<K,V>> action) =>
            MapModule.Iter(this, (k, v) => action(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        public Map<K, V> Choose(Func<K, V, Option<V>> selector) =>
            MapModule.Choose(this, selector);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        public Map<K, V> Choose(Func<V, Option<V>> selector) =>
            MapModule.Choose(this,selector);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public S Fold<S>(S state, Func<S, K, V, S> folder) =>
            MapModule.Fold(this, state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public S Fold<S>(S state, Func<S, V, S> folder) =>
            MapModule.Fold(this, state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public S Fold<S>(S state, Func<S, K, S> folder) =>
            MapModule.Fold(this, state, folder);

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
                     select tuple(kv.Key, kv.Value));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        public Map<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = MapModule.SetItem(self, item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        public Map<K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = MapModule.SetItem(self, item.Item1, item.Item2);
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
                self = MapModule.Remove(self, key);
            }
            return self;
        }
        
        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        public bool Contains(KeyValuePair<K, V> pair) =>
            match(MapModule.TryFind(this, pair.Key),
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
                            Some: x => tuple(x, true),
                            None: () => tuple(default(V), false));
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
            MapModule.Add(this, key, value);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItem(K key, V value) =>
            MapModule.SetItem(this, key, value);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Remove(K key) =>
            MapModule.Remove(this, key);

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
        public static S Fold<S, K, V>(Map<K, V> node, S state, Func<S, K, V, S> folder) where K : IComparable<K>
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

        public static S Fold<S, K, V>(Map<K, V> node, S state, Func<S, V, S> folder) where K : IComparable<K>
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

        public static S Fold<S, K, V>(Map<K, V> node, S state, Func<S, K, S> folder) where K : IComparable<K>
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

        public static Map<K, V> Choose<K, V>(Map<K, V> node, Func<K, V, Option<V>> selector) where K : IComparable<K> =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static Map<K, V> Choose<K, V>(Map<K, V> node, Func<V, Option<V>> selector) where K : IComparable<K> =>
            Map(Filter(Map(node, selector), n => n.IsSome), n => n.Value);

        public static Unit Iter<K, V>(Map<K, V> node, Action<K, V> action) where K : IComparable<K>
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

        public static Unit Iter<K, V>(Map<K, V> node, Action<V> action) where K : IComparable<K>
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

        public static bool ForAll<K, V>(Map<K, V> node, Func<K, V, bool> pred) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? true
                : pred(node.Key, node.Value)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        public static bool Exists<K, V>(Map<K, V> node, Func<K, V, bool> pred) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? false
                : pred(node.Key, node.Value)
                    ? true
                    : ForAll(node.Left, pred) || ForAll(node.Right, pred);

        public static Map<K, V> Filter<K, V>(Map<K, V> node, Func<K, V, bool> pred) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? node
                : pred(node.Key, node.Value)
                    ? Balance(Make(node.Key, node.Value, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        public static Map<K, V> Filter<K, V>(Map<K, V> node, Func<V, bool> pred) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? node
                : pred(node.Value)
                    ? Balance(Make(node.Key, node.Value, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        private static T CheckNull<T>(T value, string context) =>
            value == null
                ? failwith<T>("Null result not allowed in " + context)
                : value;

        public static Map<K, U> Map<K, V, U>(Map<K, V> node, Func<V, U> mapper) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? Empty<K, U>.Default
                : new Node<K, U>(node.Height, node.Count, node.Key, CheckNull(mapper(node.Value),"map delegate"), Map(node.Left, mapper), Map(node.Right, mapper));

        public static Map<K, U> Map<K, V, U>(Map<K, V> node, Func<K, V, U> mapper) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? Empty<K, U>.Default
                : new Node<K, U>(node.Height, node.Count, node.Key, CheckNull(mapper(node.Key, node.Value), "map delegate"), Map(node.Left, mapper), Map(node.Right, mapper));

        public static Map<K, V> Add<K, V>(Map<K, V> node, K key, V value) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                return new Node<K, V>(1, 1, key, value, Empty<K, V>.Default, Empty<K, V>.Default);
            }
            var cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, Add(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, Add(node.Right, key, value)));
            }
            else
            {
                throw new ArgumentException("An element with the same key already exists in the Map");
            }
        }

        public static Map<K, V> SetItem<K, V>(Map<K, V> node, K key, V value) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                throw new ArgumentException("Key not found in Map");
            }
            var cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, SetItem(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, SetItem(node.Right, key, value)));
            }
            else
            {
                return new Node<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static Map<K, V> TryAdd<K, V>(Map<K, V> node, K key, V value) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                return new Node<K, V>(1, 1, key, value, Empty<K, V>.Default, Empty<K, V>.Default);
            }
            var cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, TryAdd(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, TryAdd(node.Right, key, value)));
            }
            else
            {
                return node;
            }
        }

        public static Map<K, V> AddOrUpdate<K, V>(Map<K, V> node, K key, V value) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                return new Node<K, V>(1, 1, key, value, Empty<K, V>.Default, Empty<K, V>.Default);
            }
            var cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, AddOrUpdate(node.Left, key, value), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, AddOrUpdate(node.Right, key, value)));
            }
            else
            {
                return new Node<K, V>(node.Height, node.Count, node.Key, value, node.Left, node.Right);
            }
        }

        public static Map<K, V> AddTreeToRight<K, V>(Map<K, V> node, Map<K, V> toAdd) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? toAdd
                : Balance(Make(node.Key, node.Value, node.Left, AddTreeToRight(node.Right, toAdd)));

        public static Map<K, V> Remove<K, V>(Map<K, V> node, K key) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                return node;
            }
            var cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, node.Value, Remove(node.Left, key), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Value, node.Left, Remove(node.Right, key)));
            }
            else
            {
                return Balance(AddTreeToRight(node.Left, node.Right));
            }
        }

        public static V Find<K, V>(Map<K, V> node, K key) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                throw new ArgumentException("Key not found in Map");
            }
            var cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                return Find(node.Left, key);
            }
            else if (cmp > 0)
            {
                return Find(node.Right, key);
            }
            else
            {
                return node.Value;
            }
        }

        public static IEnumerable<Tuple<K, V>> ToEnumerable<K, V>(Map<K, V> node) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                yield break;
            }
            foreach (var item in ToEnumerable(node.Left))
            {
                yield return item;
            }
            yield return tuple(node.Key, node.Value);
            foreach (var item in ToEnumerable(node.Right))
            {
                yield return item;
            }
        }

        public static IEnumerable<V> FindRange<K, V>(Map<K, V> node, K a, K b) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                yield break;
            }
            if (node.Key.CompareTo(a) < 0)
            {
                foreach (var item in FindRange(node.Right, a, b))
                {
                    yield return item;
                }
            }
            else if (node.Key.CompareTo(b) > 0)
            {
                foreach (var item in FindRange(node.Left, a, b))
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in FindRange(node.Left, a, b))
                {
                    yield return item;
                }
                yield return node.Value;
                foreach (var item in FindRange(node.Right, a, b))
                {
                    yield return item;
                }
            }
        }

        public static Option<V> TryFind<K, V>(Map<K, V> node, K key) where K : IComparable<K>
        {
            if (node.Tag == MapTag.Empty)
            {
                return None;
            }
            var cmp = key.CompareTo(node.Key);
            if (cmp < 0)
            {
                return TryFind(node.Left, key);
            }
            else if (cmp > 0)
            {
                return TryFind(node.Right, key);
            }
            else
            {
                return Some(node.Value);
            }
        }

        public static Map<K, V> Skip<K, V>(Map<K, V> node, int amount) where K : IComparable<K>
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

        public static Map<K, V> Make<K, V>(K k, V v, Map<K, V> l, Map<K, V> r) where K : IComparable<K> =>
            new Node<K, V>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, k, v, l, r);

        public static Map<K, V> Balance<K, V>(Map<K, V> node) where K : IComparable<K> =>
            node.BalanceFactor >= 2
                ? node.Left.BalanceFactor >= 1
                    ? RotRight(node)
                    : DblRotRight(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor <= -1
                        ? RotLeft(node)
                        : DblRotLeft(node)
                    : node;

        public static Map<K, V> RotRight<K, V>(Map<K, V> node) where K : IComparable<K> =>
            node.Tag == MapTag.Empty || node.Left.Tag == MapTag.Empty
                ? node
                : Make(node.Left.Key, node.Left.Value, node.Left.Left, Make(node.Key, node.Value, node.Left.Right, node.Right));

        public static Map<K, V> RotLeft<K, V>(Map<K, V> node) where K : IComparable<K> =>
            node.Tag == MapTag.Empty || node.Right.Tag == MapTag.Empty
                ? node
                : Make(node.Right.Key, node.Right.Value, Make(node.Key, node.Value, node.Left, node.Right.Left), node.Right.Right);

        public static Map<K, V> DblRotRight<K, V>(Map<K, V> node) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? node
                : RotRight(Make(node.Key, node.Value, RotLeft(node.Left), node.Right));

        public static Map<K, V> DblRotLeft<K, V>(Map<K, V> node) where K : IComparable<K> =>
            node.Tag == MapTag.Empty
                ? node
                : RotLeft(Make(node.Key, node.Value, node.Left, RotRight(node.Right)));
    }

    public class Empty<K, V> : Map<K, V> where K : IComparable<K>
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

    public class Node<K, V> : Map<K, V> where K : IComparable<K>
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
