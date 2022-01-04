#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Atoms provide a way to manage shared, synchronous, independent state without 
    /// locks.  `AtomHashMap` wraps the language-ext `HashMap`, and makes sure all operations are atomic and thread-safe
    /// without resorting to locking
    /// </summary>
    /// <remarks>
    /// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
    /// </remarks>
    public class AtomHashMap<K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<HashMap<K, V>>,
        IEquatable<AtomHashMap<K, V>>
    {
        internal volatile TrieMap<EqDefault<K>, K, V> Items;

        public event AtomChangedEvent<HashMap<K, V>>? Change;

        /// <summary>
        /// Creates a new atom-hashmap
        /// </summary>
        public static AtomHashMap<K, V> Empty => new AtomHashMap<K, V>(TrieMap<EqDefault<K>, K, V>.Empty);
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">Trie map</param>
        AtomHashMap(TrieMap<EqDefault<K>, K, V> items) =>
            this.Items = items;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">Hash map</param>
        internal AtomHashMap(HashMap<K, V> items) =>
            this.Items = items.Value;
        
        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public V this[K key] =>
            Items[key];

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.IsEmpty;
        }

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.Count;
        }

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items.Count;
        }

        /// <summary>
        /// Get a IReadOnlyDictionary for this map.  No mapping is required, so this is very fast.
        /// </summary>
        [Pure]
        public IReadOnlyDictionary<K, V> ToReadOnlyDictionary() =>
            Items;

        /// <summary>
        /// Atomically swap the underlying hash-map.  Allows for multiple operations on the hash-map in an entirely
        /// transactional and atomic way.
        /// </summary>
        /// <param name="swap">Swap function, maps the current state of the AtomHashMap to a new state</param>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit Swap(Func<HashMap<K, V>, HashMap<K, V>> swap)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = swap(new HashMap<K, V>(oitems)).Value;
                if(ReferenceEquals(oitems, nitems))
                {
                    // no change
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically swap a key in the hash-map, if it exists.  If it doesn't exist, nothing changes.
        /// Allows for multiple operations on the hash-map value in an entirely transactional and atomic way.
        /// </summary>
        /// <param name="swap">Swap function, maps the current state of a value in the AtomHashMap to a new value</param>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit SwapKey(K key, Func<V, V> swap)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var ovalue = oitems.Find(key);
                if (ovalue.IsNone) return unit;
                var nitems = oitems.SetItem(key, swap((V)ovalue));
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically swap a key in the hash-map:
        /// 
        /// * If the key doesn't exist, then `None` is passed to `swap`
        /// * If the key does exist, then `Some(x)` is passed to `swap`
        /// 
        /// The operation performed on the hash-map depends on the value going in and out of `swap`:
        /// 
        ///   In        | Out       | Operation       
        ///   --------- | --------- | --------------- 
        ///   `Some(x)` | `Some(y)` | `SetItem(key, y)` 
        ///   `Some(x)` | `None`    | `Remove(key)`     
        ///   `None`    | `Some(y)` | `Add(key, y)`     
        ///   `None`    | `None`    | `no-op`           
        ///  
        /// Allows for multiple operations on the hash-map value in an entirely transactional and atomic way.
        /// </summary>
        /// <param name="swap">Swap function, maps the current state of a value in the AtomHashMap to a new value</param>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit SwapKey(K key, Func<Option<V>, Option<V>> swap)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var ovalue = oitems.Find(key);
                var nvalue = swap(ovalue);

                var nitems = (ovalue.IsSome, nvalue.IsSome) switch
                             {
                                 (true, true)   => oitems.SetItem(key, (V)nvalue),
                                 (true, false)  => oitems.Remove(key),
                                 (false, true)  => oitems.Add(key, (V)nvalue),
                                 (false, false) => oitems
                             };
                
                if(ReferenceEquals(oitems, nitems))
                {
                    // no change
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }
        
        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public AtomHashMap<K, V> Filter(Func<V, bool> pred) =>
            new(Items.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit FilterInPlace(Func<V, bool> pred)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.Filter(pred);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        [Pure]
        public AtomHashMap<K, V> Filter(Func<K, V, bool> pred) =>
            new(Items.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit FilterInPlace(Func<K, V, bool> pred)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.Filter(pred);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit MapInPlace(Func<V, V> f) 
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.Map(f);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        public AtomHashMap<K, U> Map<U>(Func<K, V, U> f) =>
            new AtomHashMap<K, U>(Items.Map(f));
        
        /// <summary>
        /// Atomically adds a new item to the map
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the key already exists</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        public Unit Add(K key, V value)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.Add(key, value);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists, then the new item is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        public Unit TryAdd(K key, V value)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.TryAdd(key, value);
                if(ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }
        
        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists, the new item replaces it.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        public Unit AddOrUpdate(K key, V value)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.AddOrUpdate(key, value);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit AddOrUpdate(K key, Func<V, V> Some, Func<V> None)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.AddOrUpdate(key, Some, None);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit AddOrUpdate(K key, Func<V, V> Some, V None)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.AddOrUpdate(key, Some, None);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        public Unit AddRange(IEnumerable<(K Key, V Value)> range)
        {
            SpinWait sw = default;
            var srange = toSeq(range);
            if (srange.IsEmpty) return default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.AddRange(srange);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        public Unit TryAddRange(IEnumerable<(K Key, V Value)> range)
        {
            SpinWait sw = default;
            var srange = toSeq(range);
            if (srange.IsEmpty) return default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.TryAddRange(srange);
                if(ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        public Unit TryAddRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            SpinWait sw = default;
            var srange = toSeq(range);
            if (srange.IsEmpty) return default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.TryAddRange(srange);
                if(ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        public Unit AddOrUpdateRange(IEnumerable<(K Key, V Value)> range)
        {
            SpinWait sw = default;
            var srange = toSeq(range);
            if (srange.IsEmpty) return default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.AddOrUpdateRange(srange);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        public Unit AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            SpinWait sw = default;
            var srange = toSeq(range);
            if (srange.IsEmpty) return default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.AddOrUpdateRange(srange);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        public Unit Remove(K key)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.Remove(key);
                if(ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<V> Find(K value) =>
            Items.Find(value);

        /// <summary>
        /// Retrieve a value from the map by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public IEnumerable<V> FindSeq(K key) =>
            Items.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the map by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(K key, Func<V, R> Some, Func<R> None) =>
            Items.Find(key, Some, None);

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Added value</returns>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public V FindOrAdd(K key, Func<V> None)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var (nitems, value) = Items.FindOrAdd(key, None);
                if(ReferenceEquals(oitems, nitems))
                {
                    return value;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return value;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }        

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="value">Delegate to get the value</param>
        /// <returns>Added value</returns>
        public V FindOrAdd(K key, V value)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var (nitems, nvalue) = Items.FindOrAdd(key, value);
                if(ReferenceEquals(oitems, nitems))
                {
                    return nvalue;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return nvalue;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }           

        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Added value</returns>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Option<V> FindOrMaybeAdd(K key, Func<Option<V>> None)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var (nitems, nvalue) = Items.FindOrMaybeAdd(key, None);
                if(ReferenceEquals(oitems, nitems))
                {
                    return nvalue;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return nvalue;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }  
        
        /// <summary>
        /// Try to find the key in the map, if it doesn't exist, add a new 
        /// item by invoking the delegate provided.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <param name="None">Delegate to get the value</param>
        /// <returns>Added value</returns>
        public Option<V> FindOrMaybeAdd(K key, Option<V> None)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var (nitems, nvalue) = Items.FindOrMaybeAdd(key, None);
                if(ReferenceEquals(oitems, nitems))
                {
                    return nvalue;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return nvalue;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }  

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        public Unit SetItem(K key, V value)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.SetItem(key, value);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }  
        
        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the item isn't found</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit SetItem(K key, Func<V, V> Some)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.SetItem(key, Some);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Atomically updates an existing item, unless it doesn't exist, in which case 
        /// it is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the value is null</exception>
        public Unit TrySetItem(K key, V value)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.TrySetItem(key, value);
                if(ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Atomically sets an item by first retrieving it, applying a map, and then putting it back.
        /// Silently fails if the value doesn't exist
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <param name="Some">delegate to map the existing value to a new one before setting</param>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit TrySetItem(K key, Func<V, V> Some)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.TrySetItem(key, Some);
                if(ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool ContainsKey(K key) =>
            Items.ContainsKey(key);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains(K key, V value) =>
            Items.Contains(key, value);

        /// <summary>
        /// Checks for existence of a value in the map
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if an item with the value supplied is in the map</returns>
        [Pure]
        public bool Contains(V value) =>
            Items.Contains(value);

        /// <summary>
        /// Checks for existence of a value in the map
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if an item with the value supplied is in the map</returns>
        [Pure]
        public bool Contains<EqV>(V value) where EqV : struct, Eq<V> =>
            Items.Contains<EqV>(value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains<EqV>(K key, V value) where EqV : struct, Eq<V> =>
            Items.Contains<EqV>(key, value);

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        public Unit Clear()
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.Clear();
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        public Unit AddRange(IEnumerable<KeyValuePair<K, V>> pairs)
        {
            SpinWait sw = default;
            var spairs = toSeq(pairs);
            if (spairs.IsEmpty) return default;
            while (true)
            {
                var oitems = Items;
                var nitems = Items.AddRange(spairs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        public Unit SetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            SpinWait sw = default;
            var sitems = toSeq(items);
            if (sitems.IsEmpty) return default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.SetItems(sitems);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        public Unit SetItems(IEnumerable<(K Key, V Value)> items)
        {
            SpinWait sw = default;
            var sitems = toSeq(items);
            if (sitems.IsEmpty) return default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.SetItems(sitems);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        public Unit TrySetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            SpinWait sw = default;
            var sitems = toSeq(items);
            if (sitems.IsEmpty) return default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.TrySetItems(sitems);
                if (ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        public Unit TrySetItems(IEnumerable<(K Key, V Value)> items)
        {
            SpinWait sw = default;
            var sitems = toSeq(items);
            if (sitems.IsEmpty) return default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.TrySetItems(sitems);
                if (ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Atomically sets a series of items using the keys provided to find the items
        /// and the Some delegate maps to a new value.  If the items don't exist then
        /// they're silently ignored.
        /// </summary>
        /// <param name="keys">Keys of items to set</param>
        /// <param name="Some">Function map the existing item to a new one</param>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit TrySetItems(IEnumerable<K> keys, Func<V, V> Some)
        {
            SpinWait sw = default;
            var skeys = toSeq(keys);
            if (skeys.IsEmpty) return default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.TrySetItems(skeys, Some);
                if (ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Atomically removes a set of keys from the map
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        public Unit RemoveRange(IEnumerable<K> keys)
        {
            SpinWait sw = default;
            var skeys = toSeq(keys);
            if (skeys.IsEmpty) return default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.RemoveRange(skeys);
                if (ReferenceEquals(oitems, nitems))
                {
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) =>
            Items.Contains(pair.Key, pair.Value);

        /// <summary>
        /// Enumerable of map keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys =>
            Items.Keys;

        /// <summary>
        /// Enumerable of map values
        /// </summary>
        [Pure]
        public IEnumerable<V> Values =>
            Items.Values;

        /// <summary>
        /// Convert the map to an IDictionary
        /// </summary>
        /// <remarks>This is effectively a zero cost operation, not even a single allocation</remarks>
        [Pure]
        public IReadOnlyDictionary<K, V> ToDictionary() =>
            Items;

        /// <summary>
        /// Convert to a HashMap
        /// </summary>
        /// <remarks>This is effectively a zero cost operation, not even a single allocation</remarks>
        [Pure]
        public HashMap<K, V> ToHashMap() =>
            new HashMap<K, V>(Items);

        /// <summary>
        /// Map the map the a dictionary
        /// </summary>
        [Pure]
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<(K Key, V Value), KR> keySelector, Func<(K Key, V Value), VR> valueSelector) =>
            AsEnumerable().ToDictionary(keySelector, valueSelector);

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            Items.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            Items.GetEnumerator();

        [Pure]
        public Seq<(K Key, V Value)> ToSeq() =>
            toSeq(AsEnumerable());

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// The ellipsis is used for collections over 50 items
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
            Items;

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AtomHashMap<K, V>(SeqEmpty _) =>
            Empty;

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator ==(AtomHashMap<K, V> lhs, AtomHashMap<K, V> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator ==(AtomHashMap<K, V> lhs, HashMap<K, V> rhs) =>
            lhs?.Items.Equals(rhs.Value) ?? false;

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator ==(HashMap<K, V> lhs, AtomHashMap<K, V> rhs) =>
            lhs.Value.Equals(rhs.Items);

        /// <summary>
        /// In-equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator !=(AtomHashMap<K, V> lhs, AtomHashMap<K, V> rhs) =>
            !(lhs == rhs);

        /// <summary>
        /// In-equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator !=(AtomHashMap<K, V> lhs, HashMap<K, V> rhs) =>
            !(lhs == rhs);

        /// <summary>
        /// In-equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public static bool operator !=(HashMap<K, V> lhs, AtomHashMap<K, V> rhs) =>
            !(lhs == rhs);

        public Unit Append(AtomHashMap<K, V> rhs)
        {
            if (rhs.IsEmpty) return default;
            SpinWait sw = default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Append(rhs.Items);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        public Unit Append(HashMap<K, V> rhs)
        {
            if (rhs.IsEmpty) return default;
            SpinWait sw = default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Append(rhs.Value);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        public Unit Subtract(AtomHashMap<K, V> rhs) 
        {
            if (rhs.IsEmpty) return default;
            SpinWait sw = default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Subtract(rhs.Items);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }         

        public Unit Subtract(HashMap<K, V> rhs) 
        {
            if (rhs.IsEmpty) return default;
            SpinWait sw = default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Subtract(rhs.Value);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }         

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<K> other) =>
            Items.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<K> other) =>
            Items.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<K> other) =>
            Items.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(HashMap<K, V> other) =>
            Items.IsSubsetOf(other.Value);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            Items.IsSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<K> rhs) =>
            Items.IsSupersetOf(rhs);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public Unit Intersect(IEnumerable<K> rhs)
        {
            SpinWait sw = default;
            var srhs = toSeq(rhs);            
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Intersect(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public Unit Intersect(IEnumerable<(K Key, V Value)> rhs)
        {
            SpinWait sw = default;
            var srhs = toSeq(rhs);            
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Intersect(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        public bool Overlaps(IEnumerable<(K Key, V Value)> other) =>
            Items.Overlaps(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        public bool Overlaps(IEnumerable<K> other) =>
            Items.Overlaps(other);

        /// <summary>
        /// Returns this - rhs.  Only the items in this that are not in 
        /// rhs will be returned.
        /// </summary>
        public Unit Except(IEnumerable<K> rhs)
        {
            SpinWait sw = default;
            var srhs = toSeq(rhs);            
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Except(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        public Unit Except(IEnumerable<(K Key, V Value)> rhs)
        {
            SpinWait sw = default;
            var srhs = toSeq(rhs);            
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Except(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        public Unit SymmetricExcept(HashMap<K, V> rhs)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Except(rhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        public Unit SymmetricExcept(IEnumerable<(K Key, V Value)> rhs)
        {
            SpinWait sw = default;
            var srhs = toSeq(rhs);
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Except(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        public Unit Union(IEnumerable<(K, V)> rhs)
        {
            SpinWait sw = default;
            var srhs = toSeq(rhs);
            while (true)
            {
                var oitems = this.Items;
                var nitems = oitems.Union(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
                    this.Change?.Invoke(new HashMap<K, V>(nitems));
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        } 
        
        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is AtomHashMap<K, V> hm && Equals(hm);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public bool Equals(AtomHashMap<K, V> other) =>
            Items.Equals(other.Items);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        public bool Equals(HashMap<K, V> other) =>
            Items.Equals(other.Value);

        /// <summary>
        /// Equality of keys only
        /// </summary>
        [Pure]
        public bool EqualsKeys(AtomHashMap<K, V> other) =>
            Items.Equals<EqTrue<V>>(other.Items);

        /// <summary>
        /// Equality of keys only
        /// </summary>
        [Pure]
        public bool EqualsKeys(HashMap<K, V> other) =>
            Items.Equals<EqTrue<V>>(other.Value);

        [Pure]
        public override int GetHashCode() =>
            Items.GetHashCode();

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public AtomHashMap<K, U> Select<U>(Func<V, U> f) =>
            new AtomHashMap<K, U>(Items.Map(f));

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public AtomHashMap<K, U> Select<U>(Func<K, V, U> f) =>
            new AtomHashMap<K, U>(Items.Map(f));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AtomHashMap<K, V> Where(Func<V, bool> pred) =>
            new AtomHashMap<K, V>(Items.Filter(pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AtomHashMap<K, V> Where(Func<K, V, bool> pred) =>
            new AtomHashMap<K, V>(Items.Filter(pred));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public bool ForAll(Func<K, V, bool> pred)
        {
            foreach (var (key, value) in AsEnumerable())
            {
                if (!pred(key, value)) return false;
            }
            return true;
        }

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
            foreach (var (key, value) in AsEnumerable())
            {
                if (pred(key, value)) return true;
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
            foreach (var (key, value) in this)
            {
                action(key, value);
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
    }
}
#nullable disable
