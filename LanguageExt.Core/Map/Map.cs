using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System;
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
#if !COREFX
    [Serializable]
#endif
    public struct Map<K, V> :
        IEnumerable<IMapItem<K, V>>,
        IReadOnlyDictionary<K, V>,
        Functor<V>,
        Foldable<V>,
        Monoid<Map<K, V>>,
        Difference<Map<K, V>>,
        Eq<Map<K,V>>
    {
        readonly MapInternal<K, V> value;

        internal Map(MapInternal<K, V> value)
        {
            this.value = value;
        }

        internal Map(MapItem<K, V> root, bool rev)
        {
            this.value = new MapInternal<K, V>(root, rev);
        }

        internal MapInternal<K, V> Value =>
            value ?? MapInternal<K, V>.Empty.Value;

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
        public Map<K, V> Add(K key, V value) => Value.Add(key,value);

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
        public Map<K, V> TryAdd(K key, V value) => Value.TryAdd(key, value);

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
        public Map<K, V> TryAdd(K key, V value, Func<Map<K, V>, V, Map<K, V>> Fail) => Value.TryAdd(key, value, Fail);

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddRange(IEnumerable<Tuple<K, V>> range) => Value.AddRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> TryAddRange(IEnumerable<Tuple<K, V>> range) => Value.TryAddRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range) => Value.TryAddRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range) => Value.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range) => Value.AddOrUpdateRange(range);

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public Map<K, V> Remove(K key) => Value.Remove(key);

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
        public Map<K, V> SetItem(K key, V value) => Value.SetItem(key, value);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the item isn't found</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<K, V> SetItem(K key, Func<V, V> Some) => Value.SetItem(key, Some);

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
        public Map<K, V> TrySetItem(K key, V value) => Value.TrySetItem(key, value);

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
        public Map<K, V> TrySetItem(K key, Func<V, V> Some) => Value.TrySetItem(key, Some);

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
        public Map<K, V> TrySetItem(K key, Func<V, V> Some, Func<Map<K, V>, Map<K, V>> None) => Value.TrySetItem(key, Some, None);

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
        public Map<K, V> AddOrUpdate(K key, V value) => Value.TrySetItem(key,value);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) => Value.AddOrUpdate(key, Some, None);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public Map<K, V> AddOrUpdate(K key, Func<V, V> Some, V None) => Value.AddOrUpdate(key, Some, None);

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
        public IEnumerable<IMapItem<K, V>> Skip(int amount) => Value.Skip(amount);

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
        public Map<K, V> Clear() => Value.Clear();

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) => Value.AddRange(pairs);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items) => Value.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> SetItems(IEnumerable<Tuple<K, V>> items) => Value.SetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items) => Value.TrySetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> TrySetItems(IEnumerable<Tuple<K, V>> items) => Value.TrySetItems(items);

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public Map<K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some) => Value.TrySetItems(keys, Some);

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New map with the items removed</returns>
        [Pure]
        public Map<K, V> RemoveRange(IEnumerable<K> keys) => Value.RemoveRange(keys);

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) => Value.Contains(pair);

        /// <summary>
        /// TryGetValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("TryGetValue is obsolete, use TryFind instead")]
        public bool TryGetValue(K key, out V value) => Value.TryGetValue(key, out value);

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
        /// Convert the map to an IDictionary
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IDictionary<K, V> ToDictionary() => Value.ToDictionary();

        /// <summary>
        /// Map the map the a dictionary
        /// </summary>
        [Pure]
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<IMapItem<K, V>, KR> keySelector, Func<IMapItem<K, V>, VR> valueSelector)
            => Value.ToDictionary(keySelector, valueSelector);

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<Tuple<K, V>> Tuples => Value.Tuples;

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<IMapItem<K, V>> GetEnumerator() => Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();

        public IEnumerable<IMapItem<K, V>> AsEnumerable() => Value.AsEnumerable();

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            foreach(var item in this)
            {
                yield return new KeyValuePair<K, V>(item.Key, item.Value);
            }
        }

        public bool TryGetKey(K equalKey, out K actualKey) =>  Value.TryGetKey(equalKey, out actualKey);

        internal Map<K, V> SetRoot(MapItem<K, V> root) =>
            new Map<K, V>(new MapInternal<K, V>(root, Value.Rev));

        [Pure]
        static Map<K, V> AsMap(Foldable<V> f) =>
            (Map<K, V>)f;


        [Pure]
        static Map<K, V> AsMap(Functor<V> f) =>
            (Map<K, V>)f;

        [Pure]
        IEnumerable<V> FoldableValues(Foldable<V> fa) =>
            AsMap(fa).Values;

        [Pure]
        IEnumerable<V> MappableValues(Functor<V> fa) =>
            AsMap(fa).Values;

        [Pure]
        public S Fold<S>(Foldable<V> fa, S state, Func<S, V, S> f) =>
            FoldableValues(fa).Fold(state, f);

        [Pure]
        public S FoldBack<S>(Foldable<V> fa, S state, Func<S, V, S> f) =>
            FoldableValues(fa).FoldBack(state, f);

        Functor<B> Functor<V>.Map<B>(Functor<V> fa, Func<V, B> f) =>
            Map.map(AsMap(fa), f);

        public Map<K, V> Append(Map<K, V> x, Map<K, V> y) =>
            x.Value.Append(y);

        public Map<K, V> Difference(Map<K, V> x, Map<K, V> y) =>
            x.Value.Subtract(y);

        public static Map<K, V> Empty =>
            MapInternal<K,V>.Empty;

        Map<K, V> Monoid<Map<K,V>>.Empty() =>
            MapInternal<K, V>.Empty;

        public bool Equals(Map<K, V> x, Map<K, V> y) =>
            x.Value == y.Value;

        [Pure]
        public static Map<K, V> operator +(Map<K, V> lhs, Map<K, V> rhs) =>
            new Map<K, V>(lhs.Value + rhs.Value);

        [Pure]
        public static Map<K, V> operator -(Map<K, V> lhs, Map<K, V> rhs) =>
            new Map<K, V>(lhs.Value - rhs.Value);
    }
}
