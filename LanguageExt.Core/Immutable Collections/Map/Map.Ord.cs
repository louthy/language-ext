using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// Immutable map
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// [wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    [Serializable]
    public readonly struct Map<OrdK, K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<Map<OrdK, K, V>>,
        IComparable<Map<OrdK, K, V>>,
        IComparable
        where OrdK : struct, Ord<K>
    {
        readonly MapInternal<OrdK, K, V> value;

        internal static Map<OrdK, K, V> Wrap(MapInternal<OrdK, K, V> map) =>
            new Map<OrdK, K, V>(map);

        public Map(IEnumerable<(K Key, V Value)> items) : this(items, true)
        { }

        public Map(IEnumerable<(K Key, V Value)> items, bool tryAdd) =>
            this.value = new MapInternal<OrdK, K, V>(items, tryAdd
                ? MapModuleM.AddOpt.TryAdd
                : MapModuleM.AddOpt.ThrowOnDuplicate);

        internal Map(MapInternal<OrdK, K, V> value) =>
            this.value = value;

        internal Map(MapItem<K, V> root, bool rev) =>
            this.value = new MapInternal<OrdK, K, V>(root, rev);

        internal MapInternal<OrdK, K, V> Value =>
            value ?? MapInternal<OrdK, K, V>.Empty;
        
        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        /// <remarks>
        ///
        ///     Empty collection     = null
        ///     Singleton collection = (K, V)
        ///     More                 = ((K, V), Seq<(K, V)>)   -- head and tail
        ///
        ///     var res = list.Case switch
        ///     {
        ///       
        ///        (var x, var xs) => ...,
        ///        A value         => ...,
        ///        _               => ...
        ///     }
        /// 
        /// </remarks>
        [Pure]
        public object Case =>
            IsEmpty 
                ? null
                : toSeq(this).Case;

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
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.IsEmpty ?? true;
        }

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.Count ?? 0;
        }

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.Count ?? 0;
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
        /// Retrieve the value from previous item to specified key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key/value</returns>
        [Pure]
        public Option<(K Key, V Value)> FindPredecessor(K key) => Value.FindPredecessor(key);

        /// <summary>
        /// Retrieve the value from exact key, or if not found, the previous item 
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key/value</returns>
        [Pure]
        public Option<(K Key, V Value)> FindExactOrPredecessor(K key) => Value.FindOrPredecessor(key);

        /// <summary>
        /// Retrieve the value from next item to specified key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key/value</returns>
        [Pure]
        public Option<(K Key, V Value)> FindSuccessor(K key) => Value.FindSuccessor(key);

        /// <summary>
        /// Retrieve the value from exact key, or if not found, the next item 
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key/value</returns>
        [Pure]
        public Option<(K Key, V Value)> FindExactOrSuccessor(K key) => Value.FindOrSuccessor(key);

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (Map<OrdK, K, V> Map, V Value) FindOrAdd(K key, Func<V> None) =>
            Value.FindOrAdd(key, None).Map((x, y) => (Wrap(x), y));

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="value">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (Map<OrdK, K, V>, V Value) FindOrAdd(K key, V value) =>
            Value.FindOrAdd(key, value).Map((x, y) => (Wrap(x), y));

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (Map<OrdK, K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Func<Option<V>> None) =>
            Value.FindOrMaybeAdd(key, None).Map((x, y) => (Wrap(x), y));

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Updated map and added value</returns>
        [Pure]
        public (Map<OrdK, K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Option<V> None) =>
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
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        [Pure]
        public IEnumerable<(K Key, V Value)> FindRangePairs(K keyFrom, K keyTo) => Value.FindRangePairs(keyFrom, keyTo);

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
            toSeq(this);

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// The elipsis is used for collections over 50 items
        /// To get a formatted string with all the items, use `ToFullString`
        /// or `ToFullArrayString`.
        /// </summary>
        [Pure]
        public override string ToString() =>
            CollectionFormat.ToShortArrayString(ValueTuples.Map(kv => $"({kv.Key}: {kv.Value})"), Count);

        /// <summary>
        /// Format the collection as `(key: value), (key: value), (key: value), ...`
        /// </summary>
        [Pure]
        public string ToFullString(string separator = ", ") =>
            CollectionFormat.ToFullString(ValueTuples.Map(kv => $"({kv.Key}: {kv.Value})"), separator);

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// </summary>
        [Pure]
        public string ToFullArrayString(string separator = ", ") =>
            CollectionFormat.ToFullArrayString(ValueTuples.Map(kv => $"({kv.Key}: {kv.Value})"), separator);


        [Pure]
        public IEnumerable<(K Key, V Value)> AsEnumerable() => 
            Value.AsEnumerable();

        internal Map<OrdK, K, V> SetRoot(MapItem<K, V> root) =>
            new Map<OrdK, K, V>(new MapInternal<OrdK, K, V>(root, Value.Rev));

        public static Map<OrdK, K, V> Empty = 
            new Map<OrdK, K, V>(MapInternal<OrdK, K, V>.Empty);

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

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is Map<OrdK, K, V> m && Equals(m);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public bool Equals(Map<OrdK, K, V> y) =>
            Value.Equals<EqDefault<V>>(y.Value);

        /// <summary>
        /// Equality of keys and values
        /// </summary>
        [Pure]
        public bool Equals<EqV>(Map<OrdK, K, V> y) where EqV : struct, Eq<V> =>
            Value.Equals<EqV>(y.Value);

        /// <summary>
        /// Equality of keys only
        /// </summary>
        [Pure]
        public bool EqualsKeys(Map<OrdK, K, V> y) =>
            Value.Equals<EqTrue<V>>(y.Value);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        public int CompareTo(object obj) =>
            obj is Map<OrdK, K, V> t ? CompareTo(t) : 1;

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public Map<OrdK, K, V> Do(Action<V> f)
        {
            this.Iter(f);
            return this;
        }

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
        /// <param name="valuePred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map<OrdK, K, V> Where(Func<V, bool> valuePred) =>
            new Map<OrdK, K, V>(Value.Filter(valuePred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="keyValuePred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map<OrdK, K, V> Where(Func<K, V, bool> keyValuePred) =>
            new Map<OrdK, K, V>(Value.Filter(keyValuePred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="valuePred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public Map<OrdK, K, V> Filter(Func<V, bool> valuePred) =>
            new Map<OrdK, K, V>(Value.Filter(valuePred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="keyValuePred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public Map<OrdK, K, V> Filter(Func<K, V, bool> keyValuePred) =>
            new Map<OrdK, K, V>(Value.Filter(keyValuePred));

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
            new Map<OrdK, K, U>(Value.Choose(selector));

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public Map<OrdK, K, U> Choose<U>(Func<V, Option<U>> selector) =>
            new Map<OrdK, K, U>(Value.Choose(selector));

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
        public Map<OrdK, K, R> Union<V2, R>(Map<OrdK, K, V2> other, WhenMissing<K, V, R> MapLeft, WhenMissing<K, V2, R> MapRight, WhenMatched<K, V, V2, R> Merge) =>
            new Map<OrdK, K, R>(Value.Union(other.Value, MapLeft, MapRight, Merge));

        /// <summary>
        /// Intersect two maps.  Only keys that are in both maps are
        /// returned.  The merge function is called for every resulting
        /// key.
        [Pure]
        public Map<OrdK, K, R> Intersect<V2, R>(Map<OrdK, K, V2> other, WhenMatched<K, V, V2, R> Merge) =>
            new Map<OrdK, K, R>(Value.Intersect(other.Value, Merge));

        /// <summary>
        /// Map differencing based on key.  this - other.
        /// </summary>
        [Pure]
        public Map<OrdK, K, V> Except(Map<OrdK, K, V> other) =>
            new Map<OrdK, K, V>(Value.Except(other.Value));

        /// <summary>
        /// Keys that are in both maps are dropped and the remaining
        /// items are merged and returned.
        /// </summary>
        [Pure]
        public Map<OrdK, K, V> SymmetricExcept(Map<OrdK, K, V> other) =>
            new Map<OrdK, K, V>(Value.SymmetricExcept(other.Value));

        /// <summary>
        /// Compare keys and values (values use `OrdDefault<V>` for ordering)
        /// </summary>
        [Pure]
        public int CompareTo(Map<OrdK, K, V> other) =>
            Value.CompareTo<OrdDefault<V>>(other.Value);

        /// <summary>
        /// Compare keys and values (values use `OrdV` for ordering)
        /// </summary>
        [Pure]
        public int CompareTo<OrdV>(Map<OrdK, K, V> other) where OrdV : struct, Ord<V> =>
            Value.CompareTo<OrdV>(other.Value);

        /// <summary>
        /// Compare keys only
        /// </summary>
        [Pure]
        public int CompareKeysTo(Map<OrdK, K, V> other) =>
            Value.CompareTo<OrdTrue<V>>(other.Value);

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        public static implicit operator Map<OrdK, K, V>(SeqEmpty _) =>
            Empty;

        /// <summary>
        /// Creates a new map from a range/slice of this map
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <returns></returns>
        [Pure]
        public Map<OrdK, K, V> Slice(K keyFrom, K keyTo) =>
            new Map<OrdK, K, V>(FindRangePairs(keyFrom, keyTo));

        /// <summary>
        /// Find the lowest ordered item in the map
        /// </summary>
        [Pure]
        public Option<(K Key, V Value)> Min => Value.Min;

        /// <summary>
        /// Find the highest ordered item in the map
        /// </summary>
        [Pure]
        public Option<(K Key, V Value)> Max => Value.Max;
    }
}
