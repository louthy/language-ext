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
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Unsorted immutable hash-map
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public struct HMap<K, V> :
        IEnumerable<IMapItem<K, V>>,
        IReadOnlyDictionary<K, V>,
        Functor<V>,
        Foldable<V>,
        Monoid<HMap<K, V>>,
        Difference<HMap<K, V>>,
        Eq<HMap<K, V>>
    {
        public static readonly HMap<K, V> Empty = new HMap<K,V>(HMapInternal<K, V>.Empty);

        readonly HMapInternal<K, V> value;

        internal HMapInternal<K, V> Value => 
            value ?? HMapInternal<K, V>.Empty;

        internal HMap(HMapInternal<K, V> value)
        {
            this.value = value;
        }

        HMap<K, V> Wrap(HMapInternal<K, V> value) =>
            new HMap<K, V>(value);

        HMap<K, U> Wrap<U>(HMapInternal<K, U> value) =>
            new HMap<K, U>(value);

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public V this[K key] =>
            Value[key];

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty =>
            Value.IsEmpty;

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count =>
            Value.Count;

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length =>
            Value.Count;


        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public HMap<K, V> Filter(Func<V, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public HMap<K, V> Filter(Func<K, V, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HMap<K, U> Map<U>(Func<V, U> mapper) =>
            Wrap(Value.Map(mapper));

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HMap<K, U> Map<U>(Func<K, V, U> mapper) =>
            Wrap(Value.Map(mapper));

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
        public HMap<K, V> Add(K key, V value) =>
            Wrap(Value.Add(key, value));

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
        public HMap<K, V> TryAdd(K key, V value) =>
            Wrap(Value.TryAdd(key, value));

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
        public HMap<K, V> AddOrUpdate(K key, V value) =>
            Wrap(Value.AddOrUpdate(key, value));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public HMap<K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) =>
            Wrap(Value.AddOrUpdate(key, Some, None));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public HMap<K, V> AddOrUpdate(K key, Func<V, V> Some, V None) =>
            Wrap(Value.AddOrUpdate(key, Some, None));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HMap<K, V> AddRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.AddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HMap<K, V> TryAddRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HMap<K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range) =>
            Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HMap<K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HMap<K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public HMap<K, V> Remove(K key) =>
            Wrap(Value.Remove(key));

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<V> Find(K key) =>
            Value.Find(key);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public IEnumerable<V> FindSeq(K key) =>
            Value.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(K key, Func<V, R> Some, Func<R> None) =>
            Value.Find(key,Some,None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public HMap<K, V> SetItem(K key, V value) =>
            Wrap(Value.SetItem(key, value));

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the item isn't found</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public HMap<K, V> SetItem(K key, Func<V, V> Some) =>
            Wrap(Value.SetItem(key, Some));

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
        public HMap<K, V> TrySetItem(K key, V value) =>
            Wrap(Value.TrySetItem(key, value));

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
        public HMap<K, V> TrySetItem(K key, Func<V, V> Some) =>
            Wrap(Value.SetItem(key, Some));

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool ContainsKey(K key) =>
            Value.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains(K key, V value) =>
            Value.Contains(key, value);

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public HMap<K, V> Clear() =>
            Wrap(Value.Clear());

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HMap<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            Wrap(Value.AddRange(pairs));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HMap<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HMap<K, V> SetItems(IEnumerable<Tuple<K, V>> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HMap<K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items) =>
            Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HMap<K, V> TrySetItems(IEnumerable<Tuple<K, V>> items) =>
            Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HMap<K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some) =>
            Wrap(Value.TrySetItems(keys, Some));

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New map with the items removed</returns>
        [Pure]
        public HMap<K, V> RemoveRange(IEnumerable<K> keys) =>
            Wrap(Value.RemoveRange(keys));

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) =>
            Value.Contains(pair);

        /// <summary>
        /// TryGetValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("TryGetValue is obsolete, use TryFind instead")]
        public bool TryGetValue(K key, out V value) =>
            Value.TryGetValue(key, out value);

        /// <summary>
        /// Enumerable of map keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys =>
            Value.Keys;

        /// <summary>
        /// Enumerable of map values
        /// </summary>
        [Pure]
        public IEnumerable<V> Values =>
            Value.Values;

        /// <summary>
        /// Convert the map to an IDictionary
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IDictionary<K, V> ToDictionary() =>
            Value.ToDictionary();

        /// <summary>
        /// Map the map the a dictionary
        /// </summary>
        [Pure]
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<IMapItem<K, V>, KR> keySelector, Func<IMapItem<K, V>, VR> valueSelector) =>
            Value.ToDictionary(keySelector, valueSelector);

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<Tuple<K, V>> Tuples =>
            Value.Tuples;

        #region IEnumerable interface
        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<IMapItem<K, V>> GetEnumerator() =>
            Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        public IEnumerable<IMapItem<K, V>> AsEnumerable() =>
            Value.AsEnumerable();

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() =>
            ((IEnumerable<KeyValuePair<K, V>>)Value).GetEnumerator();

        #endregion

        public bool TryGetKey(K equalKey, out K actualKey)
        {
            // TODO: Not sure of the behaviour here
            throw new NotImplementedException();
        }

        [Pure]
        public static bool operator ==(HMap<K, V> lhs, HMap<K, V> rhs) =>
            lhs.Value == rhs.Value;

        [Pure]
        public static bool operator !=(HMap<K, V> lhs, HMap<K, V> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static HMap<K, V> operator +(HMap<K, V> lhs, HMap<K, V> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public HMap<K, V> Append(HMap<K, V> rhs) =>
            Wrap(Value.Append(rhs.Value));

        [Pure]
        public static HMap<K, V> operator -(HMap<K, V> lhs, HMap<K, V> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public HMap<K, V> Subtract(HMap<K, V> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        HMap<K, V> As(Functor<V> m) => (HMap<K, V>)m;
        HMap<K, V> As(Foldable<V> m) => (HMap<K, V>)m;

        [Pure]
        public Functor<B> Map<B>(Functor<V> fa, Func<V, B> f) =>
            As(fa).Map(f);

        [Pure]
        public S Fold<S>(Foldable<V> fa, S state, Func<S, V, S> f) =>
            As(fa).Fold(state, f);

        [Pure]
        public S FoldBack<S>(Foldable<V> fa, S state, Func<S, V, S> f) =>
            As(fa).FoldBack(state, f);

        [Pure]
        HMap<K, V> Monoid<HMap<K, V>>.Empty() => 
            HMap<K, V>.Empty;

        [Pure]
        public HMap<K, V> Append(HMap<K, V> x, HMap<K, V> y) =>
            x + y;

        [Pure]
        public HMap<K, V> Difference(HMap<K, V> x, HMap<K, V> y) =>
            x - y;

        [Pure]
        public bool Equals(HMap<K, V> x, HMap<K, V> y) =>
            x == y;

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) && obj is HMap<K, V> && Equals(this, (HMap<K, V>)obj);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

    }
}
