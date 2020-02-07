using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Linq;

namespace LanguageExt
{
    /// <summary>
    /// Unsorted immutable hash-map
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public struct HashMap<K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<HashMap<K, V>>
    {
        public static readonly HashMap<K, V> Empty = new HashMap<K,V>(TrieMap<EqDefault<K>, K, V>.Empty);

        readonly TrieMap<EqDefault<K>, K, V> value;

        internal TrieMap<EqDefault<K>, K, V> Value => 
            value ?? TrieMap<EqDefault<K>, K, V>.Empty;

        internal HashMap(TrieMap<EqDefault<K>, K, V> value)
        {
            this.value = value;
        }


        public HashMap(IEnumerable<(K Key, V Value)> items) 
            : this(items, true)
        { }

        public HashMap(IEnumerable<(K Key, V Value)> items, bool tryAdd) =>
            this.value = new TrieMap<EqDefault<K>, K, V>(items, tryAdd);

        /// <summary>
        /// Item at index lens
        /// </summary>
        [Pure]
        public static Lens<HashMap<K, V>, V> item(K key) => Lens<HashMap<K, V>, V>.New(
            Get: la => la[key],
            Set: a => la => la.AddOrUpdate(key, a)
            );

        /// <summary>
        /// Item or none at index lens
        /// </summary>
        [Pure]
        public static Lens<HashMap<K, V>, Option<V>> itemOrNone(K key) => Lens<HashMap<K, V>, Option<V>>.New(
            Get: la => la[key],
            Set: a => la => a.Match(Some: x => la.AddOrUpdate(key, x), None: () => la.Remove(key))
            );

        /// <summary>
        /// Lens map
        /// </summary>
        [Pure]
        public static Lens<HashMap<K, V>, HashMap<K, B>> map<B>(Lens<V, B> lens) => Lens<HashMap<K, V>, HashMap<K, B>>.New(
            Get: la => la.Map(lens.Get),
            Set: lb => la =>
            {
                foreach (var item in lb)
                {
                    la = la.AddOrUpdate(item.Key, lens.Set(item.Value, la[item.Key]));
                }
                return la;
            });

        static HashMap<K, V> Wrap(TrieMap<EqDefault<K>, K, V> value) =>
            new HashMap<K, V>(value);

        static HashMap<K, U> Wrap<U>(TrieMap<EqDefault<K>, K, U> value) =>
            new HashMap<K, U>(value);

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public SeqCase<(K Key, V Value)> Case =>
            Seq<(K Key, V Value)>(Value).Case;

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
        /// Get a IReadOnlyDictionary for this map.  No mapping is required, so this is very fast.
        /// </summary>
        [Pure]
        public IReadOnlyDictionary<K, V> ToReadOnlyDictionary() =>
            Value;

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public HashMap<K, V> Filter(Func<V, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public HashMap<K, V> Filter(Func<K, V, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HashMap<K, U> Map<U>(Func<V, U> mapper) =>
            Wrap(Value.Map(mapper));

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HashMap<K, U> Map<U>(Func<K, V, U> mapper) =>
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
        public HashMap<K, V> Add(K key, V value) =>
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
        public HashMap<K, V> TryAdd(K key, V value) =>
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
        public HashMap<K, V> AddOrUpdate(K key, V value) =>
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
        public HashMap<K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None) =>
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
        public HashMap<K, V> AddOrUpdate(K key, Func<V, V> Some, V None) =>
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
        public HashMap<K, V> AddRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.AddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HashMap<K, V> AddRange(IEnumerable<(K Key, V Value)> range) =>
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
        public HashMap<K, V> TryAddRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HashMap<K, V> TryAddRange(IEnumerable<(K Key, V Value)> range) =>
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
        public HashMap<K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range) =>
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
        public HashMap<K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HashMap<K, V> AddOrUpdateRange(IEnumerable<(K Key, V Value)> range) =>
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
        public HashMap<K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public HashMap<K, V> Remove(K key) =>
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
            Value.Find(key, Some, None);

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (HashMap<K, V> Map, V Value) FindOrAdd(K key, Func<V> None) =>
            Value.FindOrAdd(key, None).Map((x, y) => (Wrap(x), y));

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="value">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (HashMap<K, V>, V Value) FindOrAdd(K key, V value) =>
            Value.FindOrAdd(key, value).Map((x, y) => (Wrap(x), y));

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (HashMap<K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Func<Option<V>> None) =>
            Value.FindOrMaybeAdd(key, None).Map((x, y) => (Wrap(x), y));

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (HashMap<K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Option<V> None) =>
            Value.FindOrMaybeAdd(key, None).Map((x, y) => (Wrap(x), y));

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public HashMap<K, V> SetItem(K key, V value) =>
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
        public HashMap<K, V> SetItem(K key, Func<V, V> Some) =>
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
        public HashMap<K, V> TrySetItem(K key, V value) =>
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
        public HashMap<K, V> TrySetItem(K key, Func<V, V> Some) =>
            Wrap(Value.TrySetItem(key, Some));

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
        /// Checks for existence of a value in the map
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if an item with the value supplied is in the map</returns>
        [Pure]
        public bool Contains(V value) =>
            Value.Contains(value);

        /// <summary>
        /// Checks for existence of a value in the map
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if an item with the value supplied is in the map</returns>
        [Pure]
        public bool Contains<EqV>(V value) where EqV : struct, Eq<V> =>
            Value.Contains(value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains<EqV>(K key, V value) where EqV : struct, Eq<V> =>
            Value.Contains<EqV>(key, value);

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public HashMap<K, V> Clear() =>
            Wrap(Value.Clear());

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HashMap<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            Wrap(Value.AddRange(pairs));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMap<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMap<K, V> SetItems(IEnumerable<Tuple<K, V>> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMap<K, V> SetItems(IEnumerable<(K Key, V Value)> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMap<K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items) =>
            Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMap<K, V> TrySetItems(IEnumerable<Tuple<K, V>> items) =>
            Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMap<K, V> TrySetItems(IEnumerable<(K Key, V Value)> items) =>
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
        public HashMap<K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some) =>
            Wrap(Value.TrySetItems(keys, Some));

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New map with the items removed</returns>
        [Pure]
        public HashMap<K, V> RemoveRange(IEnumerable<K> keys) =>
            Wrap(Value.RemoveRange(keys));

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) =>
            Value.Contains(pair.Key, pair.Value);

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
        public IReadOnlyDictionary<K, V> ToDictionary() =>
            Value;

        /// <summary>
        /// Map the map the a dictionary
        /// </summary>
        [Pure]
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<(K Key, V Value), KR> keySelector, Func<(K Key, V Value), VR> valueSelector) =>
            AsEnumerable().ToDictionary(x => keySelector(x), x => valueSelector(x));

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        [Pure]
        public Seq<(K Key, V Value)> ToSeq() =>
            Seq(AsEnumerable());

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// The elipsis is used for collections over 50 items
        /// To get a formatted string with all the items, use `ToFullString`
        /// or `ToFullArrayString`.
        /// </summary>
        [Pure]
        public override string ToString() =>
            CollectionFormat.ToShortArrayString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), Count);

        /// <summary>
        /// Format the collection as `(key: value), (key: value), (key: value), ...`
        /// </summary>
        [Pure]
        public string ToFullString(string separator = ", ") =>
            CollectionFormat.ToFullString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), separator);

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// </summary>
        [Pure]
        public string ToFullArrayString(string separator = ", ") =>
            CollectionFormat.ToFullArrayString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), separator);

        [Pure]
        public IEnumerable<(K Key, V Value)> AsEnumerable() =>
            Value;

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator ==(HashMap<K, V> lhs, HashMap<K, V> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// In-equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator !=(HashMap<K, V> lhs, HashMap<K, V> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static HashMap<K, V> operator +(HashMap<K, V> lhs, HashMap<K, V> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public HashMap<K, V> Append(HashMap<K, V> rhs) =>
            Wrap(Value.Append(rhs.Value));

        [Pure]
        public static HashMap<K, V> operator -(HashMap<K, V> lhs, HashMap<K, V> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public HashMap<K, V> Subtract(HashMap<K, V> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<K> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<K> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<K> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(HashMap<K, V> other) =>
            Value.IsSubsetOf(other.Value);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Value.IsSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<K> rhs) =>
            Value.IsSupersetOf(rhs);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public HashMap<K, V> Intersect(IEnumerable<K> rhs) =>
            Wrap(Value.Intersect(rhs));

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public HashMap<K, V> Intersect(IEnumerable<(K Key, V Value)> rhs) =>
            Wrap(Value.Intersect(rhs));

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        public bool Overlaps(IEnumerable<(K Key, V Value)> other) =>
            Overlaps(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        public bool Overlaps(IEnumerable<K> other) =>
            Value.Overlaps(other);

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public HashMap<K, V> Except(IEnumerable<K> rhs) =>
            Wrap(Value.Except(rhs));

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public HashMap<K, V> Except(IEnumerable<(K Key, V Value)> rhs) =>
            Wrap(Value.Except(rhs));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public HashMap<K, V> SymmetricExcept(HashMap<K, V> rhs) =>
            Wrap(Value.SymmetricExcept(rhs.Value));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public HashMap<K, V> SymmetricExcept(IEnumerable<(K Key, V Value)> rhs) =>
            Wrap(Value.SymmetricExcept(rhs));

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public HashMap<K, V> Union(IEnumerable<(K, V)> rhs) =>
            this.TryAddRange(rhs);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is HashMap<K, V> hm && Equals(hm);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public bool Equals(HashMap<K, V> other) =>
            Value.Equals<EqDefault<V>>(other.Value);

        /// <summary>
        /// Equality of keys and values with `EqV` used for values
        /// </summary>
        [Pure]
        public bool Equals<EqV>(HashMap<K, V> other) where EqV : struct, Eq<V> =>
            Value.Equals<EqV>(other.Value);

        /// <summary>
        /// Equality of keys only
        /// </summary>
        [Pure]
        public bool EqualsKeys(HashMap<K, V> other) =>
            Value.Equals<EqTrue<V>>(other.Value);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public HashMap<K, V> Do(Action<V> f)
        {
            this.Iter(f);
            return this;
        }

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HashMap<K, U> Select<U>(Func<V, U> mapper) =>
            Map(mapper);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HashMap<K, U> Select<U>(Func<K, V, U> mapper) =>
            Map(mapper);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HashMap<K, V> Where(Func<V, bool> pred) =>
            Filter(pred);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HashMap<K, V> Where(Func<K, V, bool> pred) =>
            Filter(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<K, V, bool> pred)
        {
            foreach (var item in AsEnumerable())
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
        public bool ForAll(Func<Tuple<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<(K Key, V Value), bool> pred) =>
            AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<KeyValuePair<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<V, bool> pred) =>
            Values.ForAll(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        public bool Exists(Func<K, V, bool> pred)
        {
            foreach (var item in AsEnumerable())
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
        public bool Exists(Func<Tuple<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<(K Key, V Value), bool> pred) =>
            AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<KeyValuePair<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool Exists(Func<V, bool> pred) =>
            Values.Exists(pred);

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
        public Unit Iter(Action<(K Key, V Value)> action)
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
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, K, V, S> folder) =>
            AsEnumerable().Fold(state, (s, x) => folder(s, x.Key, x.Value));

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, V, S> folder) =>
            Values.Fold(state, folder);

        [Pure]
        public static implicit operator HashMap<K, V>(ValueTuple<(K, V)> items) =>
            new HashMap<K, V>(new[] { items.Item1 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14, items.Item15 });

        [Pure]
        public static implicit operator HashMap<K, V>(((K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V), (K, V)) items) =>
            new HashMap<K, V>(new[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6, items.Item7, items.Item8, items.Item9, items.Item10, items.Item11, items.Item12, items.Item13, items.Item14, items.Item15, items.Item16 });
    }
}
