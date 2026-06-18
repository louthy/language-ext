using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

/// <summary>
/// Immutable map
/// AVL tree implementation
/// AVL tree is a self-balancing binary search tree. 
/// [wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
/// </summary>
/// <typeparam name="K">Key type</typeparam>
/// <typeparam name="V">Value type</typeparam>
[Serializable]
class MapInternal<OrdK, K, V> :
    IEnumerable<(K Key, V Value)>
    where OrdK : Ord<K>
{
    public static readonly MapInternal<OrdK, K, V> Empty = new (MapItem<K, V>.Empty, false);

    internal readonly MapItem<K, V> Root;
    internal readonly bool Rev;
    int hashCode;

    /// <summary>
    /// Ctor
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MapInternal() =>
        Root = MapItem<K, V>.Empty;

    /// <summary>
    /// Ctor
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MapInternal(MapItem<K, V> root, bool rev)
    {
        Root = root;
        Rev = rev;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MapInternal(ReadOnlySpan<(K Key, V Value)> items, MapModuleM.AddOpt option)
    {
        var root = MapItem<K, V>.Empty;
        foreach (var item in items)
        {
            root = MapModuleM.Add<OrdK, K, V>(root, item.Key, item.Value, option);
        }
        Root = root;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MapInternal(IEnumerable<(K Key, V Value)> items, MapModuleM.AddOpt option)
    {
        var root = MapItem<K, V>.Empty;
        foreach (var item in items)
        {
            root = MapModuleM.Add<OrdK, K, V>(root, item.Key, item.Value, option);
        }
        Root = root;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MapInternal(IEnumerable<KeyValuePair<K, V>> items, MapModuleM.AddOpt option)
    {
        var root = MapItem<K, V>.Empty;
        foreach (var item in items)
        {
            root = MapModuleM.Add<OrdK, K, V>(root, item.Key, item.Value, option);
        }
        Root = root;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MapInternal(IEnumerable<Tuple<K, V>> items, MapModuleM.AddOpt option)
    {
        var root = MapItem<K, V>.Empty;
        foreach (var item in items)
        {
            root = MapModuleM.Add<OrdK, K, V>(root, item.Item1, item.Item2, option);
        }
        Root = root;
    }

    /// <summary>
    /// 'this' accessor
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Optional value</returns>
    [Pure]
    public V this[K key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Find(key).IfNone(() => failwith<V>("Key doesn't exist in map"));
    }

    /// <summary>
    /// Is the map empty
    /// </summary>
    [Pure]
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Count == 0;
    }

    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    public long Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Root.Count;
    }

    [Pure]
    public Option<(K, V)> Min
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Root.IsEmpty
                   ? None
                   : MapModule.Min(Root);
    }

    [Pure]
    public Option<(K, V)> Max
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Root.IsEmpty
                   ? None
                   : MapModule.Max(Root);
    }

    /// <summary>
    /// Get the hash code of all items in the map
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() =>
        hashCode == 0
            ? (hashCode = FNV32.Hash<HashablePair<OrdK, HashableDefault<V>, K, V>, (K, V)>(AsIterable()))
            : hashCode;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.ThrowOnDuplicate);
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.ThrowOnDuplicate);
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.TryAdd);
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.TryAdd);
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.TryAdd);
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.TryUpdate);
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.TryUpdate);
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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(range, MapModuleM.AddOpt.TryUpdate);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<V> FindSeq(K key) =>
        Find(key).ToSeq();

    /// <summary>
    /// Retrieve a value from the map by key and pattern match the
    /// result.
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Find<R>(K key, Func<V, R> Some, Func<R> None) =>
        isnull(key)
            ? None()
            : match(MapModule.TryFind<OrdK, K, V>(Root, key), Some, None);

    /// <summary>
    /// Retrieve the value from predecessor item to specified key
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<(K, V)> FindPredecessor(K key) => MapModule.TryFindPredecessor<OrdK, K, V>(Root, key);

    /// <summary>
    /// Retrieve the value from exact key, or if not found, the predecessor item 
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<(K, V)> FindOrPredecessor(K key) => MapModule.TryFindOrPredecessor<OrdK, K, V>(Root, key);

    /// <summary>
    /// Retrieve the value from next item to specified key
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<(K, V)> FindSuccessor(K key) => MapModule.TryFindSuccessor<OrdK, K, V>(Root, key);

    /// <summary>
    /// Retrieve the value from exact key, or if not found, the next item 
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<(K, V)> FindOrSuccessor(K key) => MapModule.TryFindOrSuccessor<OrdK, K, V>(Root, key);


    /// <summary>
    /// Try to find the key in the map, if it doesn't exist, add a new 
    /// item by invoking the delegate provided.
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <param name="None">Delegate to get the value</param>
    /// <returns>Updated map and added value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (MapInternal<OrdK, K, V> Map, V Value) FindOrAdd(K key, Func<V> None) =>
        Find(key).Match(
            Some: x => (this, x),
            None: () =>
                  {
                      var v = None();
                      return (Add(key, v), v);
                  });

    /// <summary>
    /// Try to find the key in the map, if it doesn't exist, add a new 
    /// item provided.
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <param name="value">Delegate to get the value</param>
    /// <returns>Updated map and added value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (MapInternal<OrdK, K, V>, V Value) FindOrAdd(K key, V value) =>
        Find(key).Match(
            Some: x => (this, x),
            None: () => (Add(key, value), value));

    /// <summary>
    /// Try to find the key in the map, if it doesn't exist, add a new 
    /// item provided.
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <param name="value">Delegate to get the value</param>
    /// <returns>Updated map and added value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (MapInternal<OrdK, K, V>, Option<V> Value) FindOrMaybeAdd(K key, Func<Option<V>> value) =>
        Find(key).Match(
            Some: x => (this, Some(x)),
            None: () => value().Map(v => (Add(key, v), Some(v)))
                               .IfNone((this, None)));

    /// <summary>
    /// Try to find the key in the map, if it doesn't exist, add a new 
    /// item provided.
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <param name="value">Delegate to get the value</param>
    /// <returns>Updated map and added value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (MapInternal<OrdK, K, V>, Option<V> Value) FindOrMaybeAdd(K key, Option<V> value) =>
        Find(key).Match(
            Some: x => (this, Some(x)),
            None: () => value.Map(v => (Add(key, v), Some(v)))
                             .IfNone((this, None)));

    /// <summary>
    /// Atomically updates an existing item
    /// </summary>
    /// <remarks>Null is not allowed for a Key or a Value</remarks>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
    /// <returns>New Map with the item added</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MapInternal<OrdK, K, V> SetItem(K key, Func<V, V> Some) =>
        isnull(key)
            ? this
            : match(MapModule.TryFind<OrdK, K, V>(Root, key),
                    Some: x => SetItem(key, Some(x)),
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Iterable<V> FindRange(K keyFrom, K keyTo)
    {
        if (isnull(keyFrom)) throw new ArgumentNullException(nameof(keyFrom));
        if (isnull(keyTo)) throw new ArgumentNullException(nameof(keyTo));
        return OrdK.Compare(keyFrom, keyTo) > 0
                   ? MapModule.FindRange<OrdK, K, V>(Root, keyTo, keyFrom)
                   : MapModule.FindRange<OrdK, K, V>(Root, keyFrom, keyTo);
    }

    /// <summary>
    /// Retrieve a range of values 
    /// </summary>
    /// <param name="keyFrom">Range start (inclusive)</param>
    /// <param name="keyTo">Range to (inclusive)</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
    /// <returns>Range of values</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<V> FindRangeStrict(K keyFrom, K keyTo)
    {
        if (isnull(keyFrom)) throw new ArgumentNullException(nameof(keyFrom));
        if (isnull(keyTo)) throw new ArgumentNullException(nameof(keyTo));
        return OrdK.Compare(keyFrom, keyTo) > 0
                   ? MapModule.FindRangeStrict<OrdK, K, V>(Root, keyTo, keyFrom)
                   : MapModule.FindRangeStrict<OrdK, K, V>(Root, keyFrom, keyTo);
    }

    /// <summary>
    /// Retrieve a range of values 
    /// </summary>
    /// <param name="keyFrom">Range start (inclusive)</param>
    /// <param name="keyTo">Range to (inclusive)</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
    /// <returns>Range of values</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Iterable<(K, V)> FindRangePairs(K keyFrom, K keyTo)
    {
        if (isnull(keyFrom)) throw new ArgumentNullException(nameof(keyFrom));
        if (isnull(keyTo)) throw new ArgumentNullException(nameof(keyTo));
        return OrdK.Compare(keyFrom, keyTo) > 0
                   ? MapModule.FindRangePairs<OrdK, K, V>(Root, keyTo, keyFrom)
                   : MapModule.FindRangePairs<OrdK, K, V>(Root, keyFrom, keyTo);
    }

    /// <summary>
    /// Retrieve a range of values 
    /// </summary>
    /// <param name="keyFrom">Range start (inclusive)</param>
    /// <param name="keyTo">Range to (inclusive)</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
    /// <returns>Range of values</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<(K, V)> FindRangePairsStrict(K keyFrom, K keyTo)
    {
        if (isnull(keyFrom)) throw new ArgumentNullException(nameof(keyFrom));
        if (isnull(keyTo)) throw new ArgumentNullException(nameof(keyTo));
        return OrdK.Compare(keyFrom, keyTo) > 0
                   ? MapModule.FindRangePairsStrict<OrdK, K, V>(Root, keyTo, keyFrom)
                   : MapModule.FindRangePairsStrict<OrdK, K, V>(Root, keyFrom, keyTo);
    }

    /// <summary>
    /// Skips 'amount' values and returns a new tree without the 
    /// skipped values.
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>New tree</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MapInternal<OrdK, K, V> Skip(long amount)
    {
        var skip = amount;
        var node = Root;

        if (Rev)
        {
            while (!node.IsEmpty && skip != node.Right.Count)
            {
                if (skip < node.Right.Count)
                {
                    node = node.Right;
                }
                else
                {
                    skip -= node.Right.Count + 1;
                    node = node.Left;
                }
            }

            return new MapInternal<OrdK, K, V>(
                node.IsEmpty
                    ? MapItem<K, V>.Empty
                    : MapModule.Add<OrdK, K, V>(node.Left, node.KeyValue.Key, node.KeyValue.Value),
                Rev);
        }
        else
        {
            while (!node.IsEmpty && skip != node.Left.Count)
            {
                if (skip < node.Left.Count)
                {
                    node = node.Left;
                }
                else
                {
                    skip -= node.Left.Count + 1;
                    node = node.Right;
                }
            }

            return new MapInternal<OrdK, K, V>(
                node.IsEmpty
                    ? MapItem<K, V>.Empty
                    : MapModule.Add<OrdK, K, V>(node.Right, node.KeyValue.Key, node.KeyValue.Value),
                Rev);
        }
    }

    /// <summary>
    /// Checks for existence of a key in the map
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True if an item with the key supplied is in the map</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(K key) =>
        !isnull(key) && (Find(key) ? true : false);

    /// <summary>
    /// Checks for existence of a key in the map
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True if an item with the key supplied is in the map</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MapInternal<OrdK, K, V> Clear() =>
        Empty;

    /// <summary>
    /// Atomically adds a range of items to the map
    /// </summary>
    /// <param name="pairs">Range of KeyValuePairs to add</param>
    /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
    /// <returns>New Map with the items added</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MapInternal<OrdK, K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
        AddRange(pairs.Select(kv => (kv.Key, kv.Value)));

    /// <summary>
    /// Atomically sets a series of items using the KeyValuePairs provided
    /// </summary>
    /// <param name="items">Items to set</param>
    /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
    /// <returns>New map with the items set</returns>
    [Pure]
    public MapInternal<OrdK, K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
    {
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(items, MapModuleM.AddOpt.ThrowOnDuplicate);
        }

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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(items, MapModuleM.AddOpt.ThrowOnDuplicate);
        }

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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(items, MapModuleM.AddOpt.ThrowOnDuplicate);
        }

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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(items, MapModuleM.AddOpt.TryAdd);
        }

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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(items, MapModuleM.AddOpt.TryAdd);
        }

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
        if (Count == 0)
        {
            return new MapInternal<OrdK, K, V>(items, MapModuleM.AddOpt.TryAdd);
        }

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(K key, out V value)
    {
        var res = match(Find(key), Some: x => (x, true), None: () => (default(V)!, false));
        value = res.Item1;
        return res.Item2;
    }

    /// <summary>
    /// Equivalent to map and filter but the filtering is done based on whether the returned
    /// Option is Some or None.  If the item is None then it's filtered out, if not the the
    /// mapped Some value is used.
    /// </summary>
    /// <param name="selector">Predicate</param>
    /// <returns>Filtered map</returns>
    [Pure]
    public MapInternal<OrdK, K, U> Choose<U>(Func<K, V, Option<U>> selector)
    {
        IEnumerable<(K, U)> Yield()
        {
            foreach (var item in this)
            {
                var opt = selector(item.Key, item.Value);
                if (opt.IsNone) continue;
                yield return (item.Key, (U)opt);
            }
        }
        return new MapInternal<OrdK, K, U>(Yield(), MapModuleM.AddOpt.TryAdd);
    }

    /// <summary>
    /// Equivalent to map and filter but the filtering is done based on whether the returned
    /// Option is Some or None.  If the item is None then it's filtered out, if not the the
    /// mapped Some value is used.
    /// </summary>
    /// <param name="selector">Predicate</param>
    /// <returns>Filtered map</returns>
    [Pure]
    public MapInternal<OrdK, K, U> Choose<U>(Func<V, Option<U>> selector)
    {
        IEnumerable<(K, U)> Yield()
        {
            foreach (var item in this)
            {
                var opt = selector(item.Value);
                if (opt.IsNone) continue;
                yield return (item.Key, (U)opt);
            }
        }
        return new MapInternal<OrdK, K, U>(Yield(), MapModuleM.AddOpt.TryAdd);
    }

    /// <summary>
    /// Enumerable of map keys
    /// </summary>
    [Pure]
    public Iterable<K> Keys =>
        AsIterable().Map(x => x.Key);

    /// <summary>
    /// Enumerable of map values
    /// </summary>
    [Pure]
    public Iterable<V> Values =>
        AsIterable().Map(x => x.Value);

    /// <summary>
    /// Map to a dictionary
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<(K Key, V Value), KR> keySelector, Func<(K Key, V Value), VR> valueSelector) where KR : notnull =>
        AsIterable().ToDictionary(keySelector, valueSelector);

    /// <summary>
    /// Enumerable of in-order tuples that make up the map
    /// </summary>
    /// <returns>Tuples</returns>
    [Pure]
    public Iterable<(K Key, V Value)> Pairs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AsIterable().Map(kv => (kv.Key, kv.Value));
    }

    /// <summary>
    /// GetEnumerator - IEnumerable interface
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MapEnumerator<K, V> GetEnumerator() =>
        new(Root, Rev, 0);

    /// <summary>
    /// GetEnumerator - IEnumerable interface
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<(K Key, V Value)> IEnumerable<(K Key, V Value)>.GetEnumerator() =>
        new MapEnumerator<K, V>(Root, Rev, 0);

    /// <summary>
    /// GetEnumerator - IEnumerable interface
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() =>
        new MapEnumerator<K, V>(Root, Rev, 0);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<(K Key, V Value)> ToSeq() =>
        toSeq(AsIterable());

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Iterable<(K Key, V Value)> AsIterable() =>
        ForwardIterator().AsIterable();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Iterator<(K Key, V Value)> ForwardIterator() =>
        new Iterator.IterMapFwd<K, V>(Map.IteratorState<K, V>.Setup(Root));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Iterator<(K Key, V Value)> BackwardIterator() =>
        new Iterator.IterMapBkwd<K, V>(Map.IteratorState<K, V>.Setup(Root));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<(K Key, V Value)> AsEnumerable()
    {
        using var iter = new MapEnumerator<K, V>(Root, Rev, 0);
        while (iter.MoveNext())
        {
            yield return iter.Current;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static KeyValuePair<K, V> ToKeyValuePair((K Key, V Value) kv) => 
        new(kv.Key, kv.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MapInternal<OrdK, K, V> SetRoot(MapItem<K, V> root) =>
        new(root, Rev);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MapInternal<OrdK, K, V> operator +(MapInternal<OrdK, K, V> lhs, MapInternal<OrdK, K, V> rhs) =>
        lhs.Append(rhs);

    /// <summary>
    /// Union two maps.  The merge function is called keys are
    /// present in both map.
    /// </summary>
    [Pure]
    public MapInternal<OrdK, K, R> Union<V2, R>(MapInternal<OrdK, K, V2> other, WhenMissing<K, V, R> MapLeft, WhenMissing<K, V2, R> MapRight, WhenMatched<K, V, V2, R> Merge)
    {
        if (MapLeft  == null) throw new ArgumentNullException(nameof(MapLeft));
        if (MapRight == null) throw new ArgumentNullException(nameof(MapRight));
        if (Merge    == null) throw new ArgumentNullException(nameof(Merge));

        var root = MapItem<K, R>.Empty;

        foreach (var right in other)
        {
            var key  = right.Key;
            var left = Find(key);
            if (left.IsSome)
            {
                root = MapModuleM.Add<OrdK, K, R>(
                    root,
                    key,
                    Merge(key, left.Value!, right.Value),
                    MapModuleM.AddOpt.TryAdd);
            }
            else
            {
                root = MapModuleM.Add<OrdK, K, R>(
                    root,
                    key,
                    MapRight(key, right.Value),
                    MapModuleM.AddOpt.TryAdd);
            }
        }
        foreach (var left in this)
        {
            var key   = left.Key;
            var right = other.Find(key);
            if (right.IsNone)
            {
                root = MapModuleM.Add<OrdK, K, R>(
                    root,
                    key,
                    MapLeft(key, left.Value),
                    MapModuleM.AddOpt.TryAdd);
            }
        }
        return new MapInternal<OrdK, K, R>(root, Rev);
    }

    /// <summary>
    /// Intersect two maps.  Only keys that are in both maps are
    /// returned.  The merge function is called for every resulting
    /// key.
    /// </summary>
    [Pure]
    public MapInternal<OrdK, K, R> Intersect<V2, R>(MapInternal<OrdK, K, V2> other, WhenMatched<K, V, V2, R> Merge)
    {
        if (Merge == null) throw new ArgumentNullException(nameof(Merge));

        var root = MapItem<K, R>.Empty;

        foreach (var right in other)
        {
            var left = Find(right.Key);
            if (left.IsSome)
            {
                root = MapModuleM.Add<OrdK, K, R>(
                    root,
                    right.Key,
                    Merge(right.Key, left.Value!, right.Value),
                    MapModuleM.AddOpt.TryAdd);
            }
        }
        return new MapInternal<OrdK, K, R>(root, Rev);
    }

    /// <summary>
    /// Map differencing based on key.  this - other.
    /// </summary>
    [Pure]
    public MapInternal<OrdK, K, V> Except(MapInternal<OrdK, K, V> other)
    {
        var root = MapItem<K, V>.Empty;
        foreach(var item in this)
        {
            if (!other.ContainsKey(item.Key))
            {
                root = MapModuleM.Add<OrdK, K, V>(
                    root,
                    item.Key,
                    item.Value,
                    MapModuleM.AddOpt.ThrowOnDuplicate);
            }
        }
        return new MapInternal<OrdK, K, V>(root, Rev);
    }

    /// <summary>
    /// Keys that are in both maps are dropped and the remaining
    /// items are merged and returned.
    /// </summary>
    [Pure]
    public MapInternal<OrdK, K, V> SymmetricExcept(MapInternal<OrdK, K, V> other)
    {
        var root = MapItem<K, V>.Empty;

        foreach (var left in this)
        {
            if (!other.ContainsKey(left.Key))
            {
                root = MapModuleM.Add<OrdK, K, V>(
                    root,
                    left.Key,
                    left.Value,
                    MapModuleM.AddOpt.ThrowOnDuplicate);
            }
        }
        foreach (var right in other)
        {
            if (!ContainsKey(right.Key))
            {
                //map = map.Add(right.Key, right.Value);
                root = MapModuleM.Add<OrdK, K, V>(
                    root,
                    right.Key,
                    right.Value,
                    MapModuleM.AddOpt.ThrowOnDuplicate);
            }
        }
        return new MapInternal<OrdK, K, V>(root, Rev);
    }

    [Pure]
    public MapInternal<OrdK, K, V> Append(MapInternal<OrdK, K, V> rhs)
    {
        if (Count == 0)
        {
            return rhs;
        }
        if (rhs.Count == 0)
        {
            return this;
        }

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MapInternal<OrdK, K, V> operator -(MapInternal<OrdK, K, V> lhs, MapInternal<OrdK, K, V> rhs) =>
        lhs.Subtract(rhs);

    [Pure]
    public MapInternal<OrdK, K, V> Subtract(MapInternal<OrdK, K, V> rhs)
    {
        if(Count == 0)
        {
            return Empty;
        }

        if (rhs.Count == 0)
        {
            return this;
        }

        if (rhs.Count < Count)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item.Key);
            }
            return self;
        }
        else
        {
            var root = MapItem<K, V>.Empty;
            foreach (var item in this)
            {
                if (!rhs.Contains(item))
                {
                    root = MapModuleM.Add<OrdK, K, V>(root, item.Key, item.Value, MapModuleM.AddOpt.TryAdd);
                }
            }
            return new MapInternal<OrdK, K, V>(root, Rev);
        }
    }

    [Pure]
    public bool Equals<EqV>(MapInternal<OrdK, K, V> rhs) where EqV : Eq<V>
    {
        if (ReferenceEquals(this, rhs)) return true;
        if (Count != rhs.Count) return false;
        if (hashCode != 0 && rhs.hashCode != 0 && hashCode != rhs.hashCode) return false;

        using var iterA = GetEnumerator();
        using var iterB = rhs.GetEnumerator();
        var       count = Count;

        for (int i = 0; i < count; i++)
        {
            iterA.MoveNext();
            iterB.MoveNext();
            if (!OrdK.Equals(iterA.Current.Key, iterB.Current.Key)) return false;
            if (!EqV.Equals(iterA.Current.Value, iterB.Current.Value)) return false;
        }
        return true;
    }

    [Pure]
    public int CompareTo<OrdV>(MapInternal<OrdK, K, V> other) where OrdV : Ord<V>
    {
        var cmp = Count.CompareTo(other.Count);
        if (cmp != 0) return cmp;
        using var iterA = GetEnumerator();
        using var iterB = other.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdK.Compare(iterA.Current.Key, iterB.Current.Key);
            if (cmp != 0) return cmp;
            cmp = OrdV.Compare(iterA.Current.Value, iterB.Current.Value);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MapInternal<OrdK, K, V> Filter(Func<K, V, bool> f) =>
        new(AsIterable().Filter(mi => f(mi.Key, mi.Value)), 
            MapModuleM.AddOpt.ThrowOnDuplicate);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MapInternal<OrdK, K, V> Filter(Func<V, bool> f) =>
        new(AsIterable().Filter(mi => f(mi.Value)),
            MapModuleM.AddOpt.ThrowOnDuplicate);
    
    public IReadOnlyDictionary<K, V> ToReadOnlyDictionary() =>
        new ReadOnlyDict(this);

    /// <summary>
    /// Proxy object that maps the underlying `TrieMap` to an `IReadOnlyDictionary`
    /// </summary>
    /// <param name="Data"></param>
    class ReadOnlyDict(MapInternal<OrdK, K, V> Data) : IReadOnlyDictionary<K, V>
    {
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => 
            Data.AsEnumerable().Select(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public int Count { get; } =
            Data.Count > int.MaxValue
                ? throw new ValueOutOfRangeException(nameof(Data.Count), Data.Count, int.MaxValue)
                : (int)Data.Count;
        
        public bool ContainsKey(K key) => 
            Data.ContainsKey(key);

        public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value) =>
            Data.TryGetValue(key, out value);

        public V this[K key] => 
            Data[key];

        public IEnumerable<K> Keys => Data.Keys;
        public IEnumerable<V> Values => Data.Values;
    }
    
    class ValueOutOfRangeException(string valueName, long value, int maxValue)
        : Exception($"{valueName} ({value}) exceeds maximum value ({maxValue}), consider moving away from using "+
                    "ToReadOnlyDictionary, its Count property is limited to an `int`");          
}

