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
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// Internal representation of a hash-map.  This allows for the HMap type to be
    /// a non-nullable struct.
    /// </summary>
    internal class HashMapInternal<EqK, K, V> :
        IEnumerable<(K Key, V Value)>,
        IReadOnlyDictionary<K, V>
        where EqK : struct, Eq<K>
    {
        public static readonly HashMapInternal<EqK, K, V> Empty = new HashMapInternal<EqK, K, V>();

        readonly Map<int, Lst<(K Key, V Value)>> hashTable;
        readonly int count;
        int hashCode;

        internal HashMapInternal()
        {
            hashTable = Map<int, Lst<(K, V)>>.Empty;
        }

        internal HashMapInternal(Map<int, Lst<(K, V)>> hashTable, int count)
        {
            this.hashTable = hashTable;
            this.count = count;
        }

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public V this[K key] =>
            Find(key).IfNone(() => failwith<V>("Key doesn't exist in map"));

        /// <summary>
        /// Get the hash code of all items in the map
        /// </summary>
        public override int GetHashCode()
        {
            if (hashCode != 0) return hashCode;
            return hashCode = hash(AsEnumerable());
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
            count;

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length =>
            count;

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> Filter(Func<V, bool> pred) =>
            Filter(kv => pred(kv.Value));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> Filter(Func<K, V, bool> pred) =>
            Filter(kv => pred(kv.Key, kv.Value));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> Filter(Func<(K Key, V Value), bool> pred)
        {
            var ht = Map<int, Lst<(K, V)>>.Empty;
            var count = 0;

            foreach (var bucket in hashTable)
            {
                var b = bucket.Value.Filter(pred);
                count += b.Count;
                if (b.Count > 0)
                {
                    ht = ht.Add(bucket.Key, b);
                }
            }
            return new HashMapInternal<EqK, K, V>(ht, count);
        }

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HashMapInternal<EqK, K, U> Map<U>(Func<V, U> mapper) =>
            new HashMapInternal<EqK, K, U>(hashTable.Map(bucket => bucket.Map(kv => (kv.Key, mapper(kv.Value)))), Count);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public HashMapInternal<EqK, K, U> Map<U>(Func<K, V, U> mapper) =>
            new HashMapInternal<EqK, K, U>(hashTable.Map(bucket => bucket.Map(kv => (kv.Key, mapper(kv.Key, kv.Value)))), Count);

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
        public HashMapInternal<EqK, K, V> Add(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));
            if (isnull(value)) throw new ArgumentNullException(nameof(value));

            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);

            if (bucket.IsSome)
            {
                foreach(var item in bucket.Value)
                {
                    if(default(EqK).Equals(item.Key, key))
                    {
                        throw new ArgumentException("Key already exists in HMap");
                    }
                }
                ht = ht.SetItem(hash, bucket.Value.Add((key, value)));
            }
            else
            {
                ht = ht.Add(hash, List((key, value)));
            }
            return new HashMapInternal<EqK, K, V>(ht, Count + 1);
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
        public HashMapInternal<EqK, K, V> TryAdd(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));

            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);

            if (bucket.IsSome)
            {
                foreach (var item in bucket.Value)
                {
                    if (default(EqK).Equals(item.Key, key))
                    {
                        return this;
                    }
                }
                ht = ht.SetItem(hash, bucket.Value.Add((key, value)));
            }
            else
            {
                ht = ht.Add(hash, List((key, value)));
            }
            return new HashMapInternal<EqK, K, V>(ht, Count + 1);
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
        [Pure]
        public HashMapInternal<EqK, K, V> AddOrUpdate(K key, V value) =>
            AddOrUpdate(key, _ => value, () => value);

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="Exception">Throws Exception if None returns null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));

            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);

            if (bucket.IsSome)
            {
                var bucketValue = bucket.Value;
                var contains = false;
                var index = 0;
                V value = default(V);

                foreach (var item in bucketValue)
                {
                    if (default(EqK).Equals(item.Key, key))
                    {
                        value = item.Value;
                        contains = true;
                        break;
                    }
                    index++;
                }
                if (contains)
                {
                    return new HashMapInternal<EqK, K, V>(ht.SetItem(hash, bucketValue.SetItem(index, (key, Some(value)))), Count);
                }
                else
                {
                    return new HashMapInternal<EqK, K, V>(ht.SetItem(hash, bucketValue.Add((key, None()))), Count + 1);
                }
            }
            else
            {
                return new HashMapInternal<EqK, K, V>(ht.Add(hash, List((key, None()))), Count + 1);
            }
        }

        /// <summary>
        /// Retrieve a value from the map by key, map it to a new value,
        /// put it back.  If it doesn't exist, add a new one based on None result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if None is null</exception>
        /// <exception cref="Exception">Throws Exception if Some returns null</exception>
        /// <returns>New map with the mapped value</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, V None) =>
            AddOrUpdate(key, Some, () => None);

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> AddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.Add(item.Item1, item.Item2);
            }
            return self;
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
        public HashMapInternal<EqK, K, V> AddRange(IEnumerable<(K,V)> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.Add(item.Item1, item.Item2);
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
        [Pure]
        public HashMapInternal<EqK, K, V> TryAddRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.TryAdd(item.Item1, item.Item2);
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
        [Pure]
        public HashMapInternal<EqK, K, V> TryAddRange(IEnumerable<(K, V)> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.TryAdd(item.Item1, item.Item2);
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
        [Pure]
        public HashMapInternal<EqK, K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.TryAdd(item.Key, item.Value);
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
        [Pure]
        public HashMapInternal<EqK, K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.AddOrUpdate(item.Item1, item.Item2);
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
        [Pure]
        public HashMapInternal<EqK, K, V> AddOrUpdateRange(IEnumerable<(K, V)> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.AddOrUpdate(item.Item1, item.Item2);
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
        [Pure]
        public HashMapInternal<EqK, K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> range)
        {
            if (range == null)
            {
                return this;
            }
            var self = this;
            foreach (var item in range)
            {
                self = self.AddOrUpdate(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> Remove(K key)
        {
            if (isnull(key)) return this;
            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);

            if (bucket.IsSome)
            {
                var bucketValue = bucket.Value;
                bucketValue = bucketValue.Filter(x => !default(EqK).Equals(x.Key, key));
                return bucketValue.Count == 0
                    ? new HashMapInternal<EqK, K, V>(ht.Remove(hash), Count - 1)
                    : new HashMapInternal<EqK, K, V>(ht.SetItem(hash, bucketValue), Count - 1);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<V> Find(K key)
        {
            if (isnull(key)) return None;
            return hashTable.Find(default(EqK).GetHashCode(key))
                            .Bind(bucket => bucket.Find(x => default(EqK).Equals(x.Key, key))
                                                  .Map(x => x.Value));
        }

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
            Find(key).Match(Some, None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> SetItem(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));

            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);
            if (bucket.IsSome)
            {
                return new HashMapInternal<EqK, K, V>(ht.SetItem(hash, bucket.Value.Map(x => default(EqK).Equals(x.Key, key) ? (x.Key, value) : x)), Count);
            }
            else
            {
                return this;
            }
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
        public HashMapInternal<EqK, K, V> SetItem(K key, Func<V, V> Some)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));

            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);
            if (bucket.IsSome)
            {
                return new HashMapInternal<EqK, K, V>(ht.SetItem(hash, bucket.Value.Map(x => default(EqK).Equals(x.Key, key) ? (x.Key, Some(x.Value)) : x)), Count);
            }
            else
            {
                return this;
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
        /// <returns>New Map with the item added</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> TrySetItem(K key, V value)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));

            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);
            if (bucket.IsSome)
            {
                return new HashMapInternal<EqK, K, V>(ht.TrySetItem(hash, bucket.Value.Map(x => default(EqK).Equals(x.Key, key) ? (x.Key, value) : x)), Count);
            }
            else
            {
                return this;
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
        /// <returns>New map with the item set</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> TrySetItem(K key, Func<V, V> Some)
        {
            if (isnull(key)) throw new ArgumentNullException(nameof(key));

            var ht = hashTable;
            var hash = default(EqK).GetHashCode(key);
            var bucket = ht.Find(hash);
            if (bucket.IsSome)
            {
                return new HashMapInternal<EqK, K, V>(ht.TrySetItem(hash, bucket.Value.Map(x => default(EqK).Equals(x.Key, key) ? (x.Key, Some(x.Value)) : x)), Count);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool ContainsKey(K key) =>
            !isnull(key) && Find(key).IsSome;

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains(K key, V value) =>
            match(Find(key),
                Some: v => default(EqDefault<V>).Equals(v, value),
                None: () => false
                );

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool Contains<EqV>(K key, V value) where EqV : struct, Eq<V> =>
            match(Find(key),
                Some: v => default(EqV).Equals(v, value),
                None: () => false
                );

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> Clear() =>
            Empty;

        /// <summary>
        /// Atomically adds a range of items to the map
        /// </summary>
        /// <param name="pairs">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> AddRange(IEnumerable<KeyValuePair<K, V>> pairs) =>
            AddRange(from kv in pairs
                     select Tuple(kv.Key, kv.Value));

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (isnull(item.Key)) continue;
                self = SetItem(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = SetItem(item.Item1, item.Item2);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> SetItems(IEnumerable<(K, V)> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = SetItem(item.Item1, item.Item2);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the KeyValuePairs provided.  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (isnull(item.Key)) continue;
                self = TrySetItem(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> TrySetItems(IEnumerable<Tuple<K, V>> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = TrySetItem(item.Item1, item.Item2);
            }
            return self;
        }

        /// <summary>
        /// Atomically sets a series of items using the Tuples provided  If any of the 
        /// items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New map with the items set</returns>
        [Pure]
        public HashMapInternal<EqK, K, V> TrySetItems(IEnumerable<(K, V)> items)
        {
            if (items == null) return this;
            var self = this;
            foreach (var item in items)
            {
                if (isnull(item.Item1)) continue;
                self = TrySetItem(item.Item1, item.Item2);
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
        [Pure]
        public HashMapInternal<EqK, K, V> TrySetItems(IEnumerable<K> keys, Func<V, V> Some)
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
        public HashMapInternal<EqK, K, V> RemoveRange(IEnumerable<K> keys)
        {
            var self = this;
            foreach (var key in keys)
            {
                self = self.Remove(key);
            }
            return self;
        }

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains(KeyValuePair<K, V> pair) =>
            hashTable.Find(default(EqK).GetHashCode(pair.Key),
                Some: bucket => bucket.Exists(kv => default(EqK).Equals(kv.Key, pair.Key) && default(EqDefault<V>).Equals(kv.Value, pair.Value)),
                None: () => false);

        /// <summary>
        /// Returns true if a Key/Value pair exists in the map
        /// </summary>
        /// <param name="pair">Pair to find</param>
        /// <returns>True if exists, false otherwise</returns>
        [Pure]
        public bool Contains<EqV>(KeyValuePair<K, V> pair) where EqV : struct, Eq<V> =>
            hashTable.Find(default(EqK).GetHashCode(pair.Key),
                Some: bucket => bucket.Exists(kv => default(EqK).Equals(kv.Key, pair.Key) && default(EqV).Equals(kv.Value, pair.Value)),
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
            var res = Find(key);
            value = res.IfNoneUnsafe(default(V));
            return res.IsSome;
        }

        /// <summary>
        /// Enumerable of map keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys =>
            hashTable.Values.Bind(x => x.Map(kv => kv.Key));

        /// <summary>
        /// Enumerable of map values
        /// </summary>
        [Pure]
        public IEnumerable<V> Values =>
            hashTable.Values.Bind(x => x.Map(kv => kv.Value));

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
        public IDictionary<KR, VR> ToDictionary<KR, VR>(Func<(K, V), KR> keySelector, Func<(K, V), VR> valueSelector) =>
            AsEnumerable().ToDictionary(x => keySelector(x), x => valueSelector(x));

        #region IEnumerable interface
        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        public Seq<(K Key, V Value)> ToSeq() =>
            Seq(hashTable.Values.Bind(x => x));

        public IEnumerable<(K Key, V Value)> AsEnumerable() =>
            hashTable.Values.Bind(x => x);

        #endregion

        [Pure]
        public static HashMapInternal<EqK, K, V> operator +(HashMapInternal<EqK, K, V> lhs, HashMapInternal<EqK, K, V> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public HashMapInternal<EqK, K, V> Append(HashMapInternal<EqK, K, V> rhs)
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
        public static HashMapInternal<EqK, K, V> operator -(HashMapInternal<EqK, K, V> lhs, HashMapInternal<EqK, K, V> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public HashMapInternal<EqK, K, V> Subtract(HashMapInternal<EqK, K, V> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item.Key);
            }
            return self;
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() =>
            AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Item1, kv.Item2)).GetEnumerator();

        [Pure]
        public override bool Equals(object obj) =>
            Equals(obj as HashMapInternal<EqK, K, V>);

        [Pure]
        public static bool operator ==(HashMapInternal<EqK, K, V> lhs, HashMapInternal<EqK, K, V> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(HashMapInternal<EqK, K, V> lhs, HashMapInternal<EqK, K, V> rhs) =>
            !(lhs == rhs);

        [Pure]
        public bool Equals(HashMapInternal<EqK, K, V> rhs)
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
                if (!default(EqK).Equals(iterA.Current.Key, iterB.Current.Key)) return false;
                if (!EqualityComparer<V>.Default.Equals(iterA.Current.Value, iterB.Current.Value)) return false;
            }
            return true;
        }

    }

    class HMapKeyValue<K, V> 
    {
        public K Key { get; }
        public V Value { get; }

        public HMapKeyValue(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}
