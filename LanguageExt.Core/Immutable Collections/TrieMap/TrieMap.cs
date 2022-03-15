using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using LanguageExt.ClassInstances;
using Array = System.Array;

namespace LanguageExt
{
    /// <summary>
    /// Implementation of the CHAMP trie hash map data structure (Compressed Hash Array Map Trie)
    /// [efficient-immutable-collections.pdf](https://michael.steindorfer.name/publications/phd-thesis-efficient-immutable-collections.pdf)
    /// </summary>
    /// <remarks>
    /// Used by internally by `LanguageExt.HashMap`
    /// </remarks>
    internal class TrieMap<EqK, K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<TrieMap<EqK, K, V>>,
        IReadOnlyDictionary<K, V>
        where EqK : struct, Eq<K>
    {
        internal enum UpdateType
        {
            Add,
            TryAdd,
            AddOrUpdate,
            SetItem,
            TrySetItem
        }

        internal enum Tag
        {
            Entries,
            Collision,
            Empty
        }

        public static readonly TrieMap<EqK, K, V> Empty = new TrieMap<EqK, K, V>(EmptyNode.Default, 0);
        internal static TrieMap<EqK, K, V> EmptyForMutating => new TrieMap<EqK, K, V>(new EmptyNode(), 0);

        readonly Node Root;
        readonly int count;
        int hash;

        /// <summary>
        /// Ctor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TrieMap(Node root, int count)
        {
            Root = root;
            this.count = count;
        }

        public TrieMap(IEnumerable<(K Key, V Value)> items, bool tryAdd = true)
        {
            Root = EmptyNode.Default;
            var type = tryAdd ? UpdateType.TryAdd : UpdateType.AddOrUpdate;
            foreach (var item in items)
            {
                var hash = (uint)default(EqK).GetHashCode(item.Key);
                Sec section = default;
                var (countDelta, newRoot, _) = Root.Update((type, true), item, hash, section);
                count += countDelta;
                Root = newRoot;
            }
        }

        /// <summary>
        /// True if no items in the map
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count == 0;
        }

        /// <summary>
        /// Number of items in the map
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count;
        }

        /// <summary>
        /// Add an item to the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Add(K key, V value) =>
            Update(key, value, UpdateType.Add, false);

        /// <summary>
        /// Add an item to the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) AddWithLog(K key, V value) =>
            UpdateWithLog(key, value, UpdateType.Add, false);

        /// <summary>
        /// Try to add an item to the map.  If it already exists, do
        /// nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TryAdd(K key, V value) =>
            Update(key, value, UpdateType.TryAdd, false);

        /// <summary>
        /// Try to add an item to the map.  If it already exists, do
        /// nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) TryAddWithLog(K key, V value) =>
            UpdateWithLog(key, value, UpdateType.TryAdd, false);

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrUpdate(K key, V value) =>
            Update(key, value, UpdateType.AddOrUpdate, false);

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TrieMap<EqK, K, V> AddOrUpdateInPlace(K key, V value) =>
            Update(key, value, UpdateType.AddOrUpdate, true);

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) AddOrUpdateWithLog(K key, V value) =>
            UpdateWithLog(key, value, UpdateType.AddOrUpdate, false);

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None)
        {
            var (found, _, value) = FindInternal(key);
            return found
                ? AddOrUpdate(key, Some(value))
                : AddOrUpdate(key, None());
        }

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) AddOrUpdateWithLog(K key, Func<V, V> Some, Func<V> None)
        {
            var (found, _, value) = FindInternal(key);
            return found
                ? AddOrUpdateWithLog(key, Some(value))
                : AddOrUpdateWithLog(key, None());
        }

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrMaybeUpdate(K key, Func<V, V> Some, Func<Option<V>> None)
        {
            var (found, _, value) = FindInternal(key);
            return found
                       ? AddOrUpdate(key, Some(value))
                       : None().Map(x => AddOrUpdate(key, x)).IfNone(this);
        }

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) AddOrMaybeUpdateWithLog(K key, Func<V, V> Some, Func<Option<V>> None)
        {
            var (found, _, value) = FindInternal(key);
            return found
                ? AddOrUpdateWithLog(key, Some(value))
                : None().Map(x => AddOrUpdateWithLog(key, x)).IfNone((this, Change<V>.None));
        }

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, V None)
        {
            var (found, _, value) = FindInternal(key);
            return found
                ? AddOrUpdate(key, Some(value))
                : AddOrUpdate(key, None);
        }

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) AddOrUpdateWithLog(K key, Func<V, V> Some, V None)
        {
            var (found, _, value) = FindInternal(key);
            return found
                ? AddOrUpdateWithLog(key, Some(value))
                : AddOrUpdateWithLog(key, None);
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddRange(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.Add(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) AddRangeWithLog(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.AddWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddRange(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.Add(item.Item1, item.Item2);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) AddRangeWithLog(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.AddWithLog(item.Item1, item.Item2);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Item1, pair.Change);
                }
            }
            return (self, changes);
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddRange(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.Add(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) AddRangeWithLog(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.AddWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TryAddRange(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.TryAdd(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) TryAddRangeWithLog(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.TryAddWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TryAddRange(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.TryAdd(item.Item1, item.Item2);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) TryAddRangeWithLog(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.TryAddWithLog(item.Item1, item.Item2);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Item1, pair.Change);
                }
            }
            return (self, changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TryAddRange(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.TryAdd(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) TryAddRangeWithLog(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.TryAddWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrUpdateRange(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.AddOrUpdate(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) AddOrUpdateRangeWithLog(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.AddOrUpdateWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrUpdateRange(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.AddOrUpdate(item.Item1, item.Item2);
            }
            return self;
        }
       
        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) AddOrUpdateRangeWithLog(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.AddOrUpdateWithLog(item.Item1, item.Item2);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Item1, pair.Change);
                }
            }
            return (self, changes);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrUpdateRange(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.AddOrUpdate(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) AddOrUpdateRangeWithLog(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.AddOrUpdateWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> SetItems(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.SetItem(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) SetItemsWithLog(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.SetItemWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.SetItem(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) SetItemsWithLog(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.SetItemWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.SetItem(item.Item1, item.Item2);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) SetItemsWithLog(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.SetItemWithLog(item.Item1, item.Item2);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Item1, pair.Change);
                }
            }
            return (self, changes);
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TrySetItems(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.TrySetItem(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) TrySetItemsWithLog(IEnumerable<(K Key, V Value)> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.TrySetItemWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TrySetItems(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.TrySetItem(item.Key, item.Value);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) TrySetItemsWithLog(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.TrySetItemWithLog(item.Key, item.Value);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TrySetItems(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.TrySetItem(item.Item1, item.Item2);
            }
            return self;
        }

        /// <summary>
        /// Add a range of values to the map
        /// If any items already exist an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) TrySetItemsWithLog(IEnumerable<Tuple<K, V>> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.TrySetItemWithLog(item.Item1, item.Item2);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item.Item1, pair.Change);
                }
            }
            return (self, changes);
        }  
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TrySetItems(IEnumerable<K> items, Func<V, V> Some)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.TrySetItem(item, Some);
            }
            return self;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) TrySetItemsWithLog(IEnumerable<K> items, Func<V, V> Some)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.TrySetItemWithLog(item, Some);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item, pair.Change);
                }
            }
            return (self, changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> RemoveRange(IEnumerable<K> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.Remove(item);
            }
            return self;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V>, TrieMap<EqK, K, Change<V>> Changes) RemoveRangeWithLog(IEnumerable<K> items)
        {
            var self = this;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var item in items)
            {
                var pair = self.RemoveWithLog(item);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item, pair.Change);
                }
            }
            return (self, changes);
        }

        /// <summary>
        /// Set an item that already exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> SetItem(K key, V value) =>
            Update(key, value, UpdateType.SetItem, false);

        /// <summary>
        /// Set an item that already exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) SetItemWithLog(K key, V value) =>
            UpdateWithLog(key, value, UpdateType.SetItem, false);
        
        /// <summary>
        /// Set an item that already exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> SetItem(K key, Func<V, V> Some)
        {
            var value = Find(key).Map(Some).IfNone(() => throw new ArgumentException($"Key doesn't exist in map: {key}"));
            return SetItem(key, value);
        }
        
        /// <summary>
        /// Set an item that already exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) SetItemWithLog(K key, Func<V, V> Some)
        {
            var value = Find(key).Map(Some).IfNone(() => throw new ArgumentException($"Key doesn't exist in map: {key}"));
            return SetItemWithLog(key, value);
        }

        /// <summary>
        /// Try to set an item that already exists in the map.  If none
        /// exists, do nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TrySetItem(K key, V value) =>
            Update(key, value, UpdateType.TrySetItem, false);

        /// <summary>
        /// Try to set an item that already exists in the map.  If none
        /// exists, do nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) TrySetItemWithLog(K key, V value) =>
            UpdateWithLog(key, value, UpdateType.TrySetItem, false);

        /// <summary>
        /// Set an item that already exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TrySetItem(K key, Func<V, V> Some) =>
            Find(key)
                .Map(Some)
                .Match(Some: v => SetItem(key, v),
                       None: () => this);

        /// <summary>
        /// Set an item that already exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) TrySetItemWithLog(K key, Func<V, V> Some) =>
            Find(key)
                .Map(Some)
                .Match(Some: v => SetItemWithLog(key, v),
                       None: () => (this, Change<V>.None));

        /// <summary>
        /// Remove an item from the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Remove(K key)
        {
            var hash = (uint)default(EqK).GetHashCode(key);
            Sec section = default;
            var (countDelta, newRoot, _) = Root.Remove(key, hash, section);
            return ReferenceEquals(newRoot, Root)
                ? this
                : new TrieMap<EqK, K, V>(newRoot, count + countDelta);
        }

        /// <summary>
        /// Remove an item from the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) RemoveWithLog(K key)
        {
            var hash = (uint)default(EqK).GetHashCode(key);
            Sec section = default;
            var (countDelta, newRoot, old) = Root.Remove(key, hash, section);
            return ReferenceEquals(newRoot, Root)
                ? (this, Change<V>.None)
                : (new TrieMap<EqK, K, V>(newRoot, count + countDelta), 
                    countDelta == 0
                        ? Change<V>.None
                        : Change<V>.Removed(old));
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public V this[K key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var (found, _, value) = FindInternal(key);
                return found
                    ? value
                    : throw new ArgumentException($"Key doesn't exist in map: {key}");
            }
        }

        /// <summary>
        /// Get a key value pair from a key
        /// </summary>
        public (K Key, V Value) Get(K key)
        {
            var (found, nkey, value) = FindInternal(key);
            return found
                ? (nkey, value)
                : throw new ArgumentException($"Key doesn't exist in map: {key}");
        }

        /// <summary>
        /// Get a key value pair from a key
        /// </summary>
        public Option<(K Key, V Value)> GetOption(K key)
        {
            var (found, nkey, value) = FindInternal(key);
            return found
                ? Some((nkey, value))
                : default;
        }

        /// <summary>
        /// Get a key value pair from a key
        /// </summary>
        public Option<K> GetKeyOption(K key)
        {
            var (found, nkey, value) = FindInternal(key);
            return found
                ? Some(nkey)
                : default;
        }

        /// <summary>
        /// Create an empty map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Clear() =>
            Empty;

        /// <summary>
        /// Create an empty map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>>) ClearWithLog() =>
            (Empty, Map(Change<V>.Removed));

        /// <summary>
        /// Get the hash code of the items in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            hash == 0
                ? (hash = FNV32.Hash<HashablePair<EqK, HashableDefault<V>, K, V>, (K, V)>(AsEnumerable()))
                : hash;

        /// <summary>
        /// Returns the whether the `key` exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(K key) =>
            FindInternal(key).Found;

        /// <summary>
        /// Returns the whether the `value` exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(V value) =>
            Contains<EqDefault<V>>(value);

        /// <summary>
        /// Returns the whether the `value` exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains<EqV>(V value) where EqV: struct, Eq<V> =>
            Values.Exists(v => default(EqV).Equals(v, value));

        /// <summary>
        /// Returns the whether the `key` exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(K key, V value) =>
            Contains<EqDefault<V>>(key, value);

        /// <summary>
        /// Returns the whether the `key` exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains<EqV>(K key, V Value) where EqV : struct, Eq<V> =>
            Find(key).Map(v => default(EqV).Equals(v, Value)).IfNone(false);

        /// <summary>
        /// Returns the value associated with `key`.  Or None, if no key exists
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<V> Find(K key)
        {
            var (found, _, value) = FindInternal(key);
            return found
                ? Some(value)
                : default;
        }

        /// <summary>
        /// Returns the value associated with `key`.  Or None, if no key exists
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        (bool Found, K Key, V Value) FindInternal(K key)
        {
            var hash = (uint)default(EqK).GetHashCode(key);
            Sec section = default;
            return Root.Read(key, hash, section);
        }

        /// <summary>
        /// Returns the value associated with `key` then match the result
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Find<R>(K key, Func<V, R> Some, Func<R> None)
        {
            var (found, _, value) = FindInternal(key);
            return found
                ? Some(value)
                : None();
        }

        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, V Value) FindOrAdd(K key, Func<V> None) =>
            Find(key, Some: v => (this, v), None: () =>
            {
                var v = None();
                return (Add(key, v), v);
            });

        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, V Value, Change<V> Change) FindOrAddWithLog(K key, Func<V> None)
        {
            var item = Find(key);
            if (item.IsSome)
            {
                return (this, item.Value, Change<V>.None);
            }
            else
            {
                var v = None();
                var self = AddWithLog(key, v);
                return (self.Map, v, self.Change);
            }
        }

        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, V Value) FindOrAdd(K key, V value) =>
            Find(key, Some: v => (this, v), None: () => (Add(key, value), value));

        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, V Value, Change<V> Change) FindOrAddWithLog(K key, V value)
        {
            var item = Find(key);
            if (item.IsSome)
            {
                return (this, item.Value, Change<V>.None);
            }
            else
            {
                var self = AddWithLog(key, value);
                return (self.Map, value, self.Change);
            }
        }   
        
        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Func<Option<V>> None) =>
            Find(key, Some: v => (this, v), None: () =>
            {
                var v = None();
                return v.IsSome
                    ? (Add(key, (V)v), v)
                    : (this, v);
            });

        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, V Value, Change<V> Change) FindOrMaybeAddWithLog(K key, Func<Option<V>> None)
        {
            var item = Find(key);
            if (item.IsSome)
            {
                return (this, item.Value, Change<V>.None);
            }
            else
            {
                var v = None();
                if (v.IsSome)
                {
                    var self = AddWithLog(key, v.Value);
                    return (self.Map, v.Value, self.Change);
                }
                else
                {
                    return (this, item.Value, Change<V>.None);
                }
            }
        }        

        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Option<V> value) =>
            Find(key, Some: v => (this, v), None: () =>
                value.IsSome
                    ? (Add(key, (V)value), value)
                    : (this, value));

        /// <summary>
        /// Tries to find the value, if not adds it and returns the update map and/or value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Option<V> Value, Change<V> Change) FindOrMaybeAddWithLog(K key, Option<V> value)
        {
            var item = Find(key);
            if (item.IsSome)
            {
                return (this, item.Value, Change<V>.None);
            }
            else
            {
                if (value.IsSome)
                {
                    var self = AddWithLog(key, value.Value);
                    return (self.Map, value.Value, self.Change);
                }
                else
                {
                    return (this, item.Value, Change<V>.None);
                }
            }
        }  
        
        /// <summary>
        /// Returns the value associated with `key`.  Or None, if no key exists
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<V> FindSeq(K key) =>
            Find(key).ToSeq();

        internal static TrieMap<EqK, K, Change<V>> FindChanges(TrieMap<EqK, K, V> mx, TrieMap<EqK, K, V> my)
        {
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;

            foreach (var x in mx)
            {
                var y = my.Find(x.Key);
                if (y.IsSome)
                {
                    if (!ReferenceEquals(x.Value, y.Value) ||
                        !default(EqDefault<V>).Equals(x.Value, y.Value)
                        )
                    {
                        if (!default(EqDefault<V>).Equals(x.Value, y.Value))
                        {
                            changes = changes.AddOrUpdateInPlace(x.Key, Change<V>.Mapped(x.Value, y.Value));
                        }
                    }
                }
                else
                {
                    changes = changes.AddOrUpdateInPlace(x.Key, Change<V>.Removed(x.Value));
                }
            }

            foreach (var y in my)
            {
                var x = mx.Find(y.Key);
                if (x.IsNone)
                {
                    changes = changes.AddOrUpdateInPlace(y.Key, Change<V>.Added(y.Value));
                }
            }

            return changes;
        }

        /// <summary>
        /// Map from V to U
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, U> Map<U>(Func<V, U> f) =>
            new TrieMap<EqK, K, U>(AsEnumerable().Select(kv => (kv.Key, f(kv.Value))), false);

        /// <summary>
        /// Map from V to U
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, U> Map, TrieMap<EqK, K, Change<U>> Changes) MapWithLog<U>(Func<V, U> f)
        {
            var target = TrieMap<EqK, K, U>.EmptyForMutating;
            var changes = TrieMap<EqK, K, Change<U>>.EmptyForMutating;
            
            foreach (var pair in this)
            {
                var newv = f(pair.Value);
                target = target.AddOrUpdateInPlace(pair.Key, newv);
                changes = changes.AddOrUpdateInPlace(pair.Key, new EntryMapped<V, U>(pair.Value, newv));
            }
            return (target, changes);
        }

        /// <summary>
        /// Map from V to U
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, U> Map<U>(Func<K, V, U> f) =>
            new TrieMap<EqK, K, U>(AsEnumerable().Select(kv => (kv.Key, f(kv.Key, kv.Value))), false);

        /// <summary>
        /// Map from V to U
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, U> Map, TrieMap<EqK, K, Change<U>> Changes) MapWithLog<U>(Func<K, V, U> f)
        {
            var target = TrieMap<EqK, K, U>.EmptyForMutating;
            var changes = TrieMap<EqK, K, Change<U>>.EmptyForMutating;
            
            foreach (var pair in this)
            {
                var newv = f(pair.Key, pair.Value);
                target = target.AddOrUpdateInPlace(pair.Key, newv);
                changes = changes.AddOrUpdateInPlace(pair.Key, new EntryMapped<V, U>(pair.Value, newv));
            }
            return (target, changes);
        }

        /// <summary>
        /// Filter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Filter(Func<V, bool> f) =>
            new TrieMap<EqK, K, V>(AsEnumerable().Filter(kv => f(kv.Value)), false);

        /// <summary>
        /// Map from V to U
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) FilterWithLog(Func<V, bool> f)
        {
            var target = EmptyForMutating;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            
            foreach (var pair in this)
            {
                var pred = f(pair.Value);
                if (pred)
                {
                    target = target.AddOrUpdateInPlace(pair.Key, pair.Value);
                }
                else
                {
                    changes = changes.AddOrUpdateInPlace(pair.Key, Change<V>.Removed(pair.Value));
                }
            }
            return (target, changes);
        }
        
        /// <summary>
        /// Filter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Filter(Func<K, V, bool> f) =>
            new TrieMap<EqK, K, V>(AsEnumerable().Filter(kv => f(kv.Key, kv.Value)), false);

        /// <summary>
        /// Map from V to U
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) FilterWithLog(Func<K, V, bool> f)
        {
            var target = EmptyForMutating;
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            
            foreach (var pair in this)
            {
                var pred = f(pair.Key, pair.Value);
                if (pred)
                {
                    target = target.AddOrUpdateInPlace(pair.Key, pair.Value);
                }
                else
                {
                    changes = changes.AddOrUpdateInPlace(pair.Key, Change<V>.Removed(pair.Value));
                }
            }
            return (target, changes);
        }

        /// <summary>
        /// Associative union
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Append(TrieMap<EqK, K, V> rhs) =>
            TryAddRange(rhs.AsEnumerable());

        /// <summary>
        /// Associative union
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) AppendWithLog(TrieMap<EqK, K, V> rhs) =>
            TryAddRangeWithLog(rhs.AsEnumerable());

        /// <summary>
        /// Subtract
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Subtract(TrieMap<EqK, K, V> rhs)
        {
            var lhs = this;
            foreach (var item in rhs.Keys)
            {
                lhs = lhs.Remove(item);
            }
            return lhs;
        }

        /// <summary>
        /// Subtract
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) SubtractWithLog(TrieMap<EqK, K, V> rhs)
        {
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            var lhs = this;
            foreach (var item in rhs.Keys)
            {
                var pair = lhs.RemoveWithLog(item);
                lhs = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item, pair.Change);
                }
            }
            return (lhs, changes);
        }

        /// <summary>
        /// Union
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TrieMap<EqK, K, V> operator +(TrieMap<EqK, K, V> lhs, TrieMap<EqK, K, V> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Subtract
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TrieMap<EqK, K, V> operator -(TrieMap<EqK, K, V> lhs, TrieMap<EqK, K, V> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Equality
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(TrieMap<EqK, K, V> lhs, TrieMap<EqK, K, V> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non equality
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(TrieMap<EqK, K, V> lhs, TrieMap<EqK, K, V> rhs) =>
            (lhs != rhs);

        /// <summary>
        /// Equality
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object rhs) =>
            rhs is TrieMap<EqK, K, V> map && Equals<EqDefault<V>>(map);

        /// <summary>
        /// Equality
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TrieMap<EqK, K, V> rhs) =>
            Equals<EqDefault<V>>(rhs);

        /// <summary>
        /// Equality
        /// </summary>
        public bool Equals<EqV>(TrieMap<EqK, K, V> rhs)
            where EqV : struct, Eq<V>
        {
            if (ReferenceEquals(this, rhs)) return true;
            if (ReferenceEquals(rhs, null)) return false;
            if (Count != rhs.Count) return false;
            using var iterA = GetEnumerator();
            using var iterB = rhs.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                if (!default(EqK).Equals(iterA.Current.Key, iterB.Current.Key)) return false;
            }
            using var iterA1 = GetEnumerator();
            using var iterB1 = rhs.GetEnumerator();
            while (iterA1.MoveNext() && iterB1.MoveNext())
            {
                if (!default(EqV).Equals(iterA1.Current.Value, iterB1.Current.Value)) return false;
            }
            return true;
        }

        /// <summary>
        /// Update an item in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Update(K key, V value) =>
            Update(key, value, UpdateType.Add, false);

        /// <summary>
        /// Update an item in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, Change<V> Change) UpdateWithLog(K key, V value) =>
            UpdateWithLog(key, value, UpdateType.Add, false);
        
        /// <summary>
        /// Update an item in the map - can mutate if needed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TrieMap<EqK, K, V> Update(K key, V value, UpdateType type, bool mutate)
        {
            var hash = (uint)default(EqK).GetHashCode(key);
            Sec section = default;
            var (countDelta, newRoot, _) = Root.Update((type, mutate), (key, value), hash, section);
            return ReferenceEquals(newRoot, Root)
                ? this
                : new TrieMap<EqK, K, V>(newRoot, count + countDelta);
        }

        /// <summary>
        /// Update an item in the map - can mutate if needed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        (TrieMap<EqK, K, V> Map, Change<V> Change) UpdateWithLog(K key, V value, UpdateType type, bool mutate)
        {
            var hash = (uint)default(EqK).GetHashCode(key);
            Sec section = default;
            var (countDelta, newRoot, oldV) = Root.Update((type, mutate), (key, value), hash, section);
            return ReferenceEquals(newRoot, Root)
                ? (this, Change<V>.None)
                : (new TrieMap<EqK, K, V>(newRoot, count + countDelta), 
                    countDelta == 0 
                        ? default(EqDefault<V>).Equals(oldV, value)
                            ? Change<V>.None 
                            : Change<V>.Mapped(oldV, value)
                        : Change<V>.Added(value));
        }

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        public bool IsProperSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            IsProperSubsetOf(other.Map(x => x.Key));

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        public bool IsProperSubsetOf(IEnumerable<K> other)
        {
            if (IsEmpty)
            {
                return other.Any();
            }

            int matches = 0;
            bool extraFound = false;
            foreach (var item in other)
            {
                if (ContainsKey(item))
                {
                    matches++;
                }
                else
                {
                    extraFound = true;
                }

                if (matches == Count && extraFound)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        public bool IsProperSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            IsProperSupersetOf(other.Map(x => x.Key));

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        public bool IsProperSupersetOf(IEnumerable<K> other)
        {
            if (IsEmpty)
            {
                return false;
            }

            int matchCount = 0;
            foreach (var item in other)
            {
                matchCount++;
                if (!ContainsKey(item))
                {
                    return false;
                }
            }

            return Count > matchCount;
        }

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        public bool IsSubsetOf(IEnumerable<(K Key, V Value)> other)
        {
            if (IsEmpty)
            {
                return true;
            }

            int matches = 0;
            foreach (var item in other)
            {
                if (ContainsKey(item.Key))
                {
                    matches++;
                }
            }
            return matches == Count;
        }

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        public bool IsSubsetOf(IEnumerable<K> other)
        {
            if (IsEmpty)
            {
                return true;
            }

            int matches = 0;
            foreach (var item in other)
            {
                if (ContainsKey(item))
                {
                    matches++;
                }
            }
            return matches == Count;
        }

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        public bool IsSubsetOf(TrieMap<EqK, K, V> other)
        {
            if (IsEmpty)
            {
                // All empty sets are subsets
                return true;
            }
            if(Count > other.Count)
            {
                // A subset must be smaller or equal in size
                return false;
            }

            foreach(var item in this)
            {
                if(!other.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        public bool IsSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            IsSupersetOf(other.Map(x => x.Key));

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        public bool IsSupersetOf(IEnumerable<K> other)
        {
            foreach (var item in other)
            {
                if (!ContainsKey(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        public bool Overlaps(IEnumerable<(K Key, V Value)> other) =>
            Overlaps(other.Map(x => x.Key));

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        public bool Overlaps(IEnumerable<K> other)
        {
            if (IsEmpty)
            {
                return false;
            }

            foreach (var item in other)
            {
                if (ContainsKey(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public TrieMap<EqK, K, V> Intersect(IEnumerable<K> other)
        {
            var res = new List<(K, V)>();
            foreach (var item in other)
            {
                var litem = GetOption(item);
                if (litem.IsSome) res.Add(((K, V))litem);
            }
            return new TrieMap<EqK, K, V>(res);
        }

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) IntersectWithLog(IEnumerable<K> other)
        {
            var set = new TrieSet<EqK, K>(other);
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            var res = TrieMap<EqK, K, V>.EmptyForMutating;
            
            foreach (var item in this)
            {
                if (set.Contains(item.Key))
                {
                    res = res.AddOrUpdateInPlace(item.Key, item.Value);
                }
                else
                {
                    changes = changes.AddOrUpdateInPlace(item.Key, Change<V>.Removed(item.Value));
                }
            }
            return (res, changes);
        }

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public TrieMap<EqK, K, V> Intersect(IEnumerable<(K Key, V Value)> other)
        {
            var res = new List<(K, V)>();
            foreach (var item in other)
            {
                var litem = GetOption(item.Key);
                if (litem.IsSome) res.Add(((K, V))litem);
            }
            return new TrieMap<EqK, K, V>(res);
        }

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) IntersectWithLog(
            IEnumerable<(K Key, V Value)> other) =>
            IntersectWithLog(other.Map(pair => pair.Key));

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public TrieMap<EqK, K, V> Intersect(
            IEnumerable<(K Key, V Value)> other,
            WhenMatched<K, V, V, V> Merge)
        {
            var t = EmptyForMutating;
            foreach (var py in other)
            {
                var px = Find(py.Key);
                if (px.IsSome)
                {
                    var r = Merge(py.Key, px.Value, py.Value);
                    t = t.AddOrUpdateInPlace(py.Key, r);
                }
            }
            return t;
        }

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) IntersectWithLog(
            TrieMap<EqK, K, V> other,
            WhenMatched<K, V, V, V> Merge)
        {
            var t = EmptyForMutating;
            var c = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            foreach (var px in this)
            {
                var py = other.Find(px.Key);
                if (py.IsSome)
                {
                    var r = Merge(px.Key, px.Value, py.Value);
                    t = t.AddOrUpdateInPlace(px.Key, r);
                    if (!default(EqDefault<V>).Equals(px.Value, r))
                    {
                        c = c.AddOrUpdateInPlace(px.Key, Change<V>.Mapped(px.Value, r));
                    }
                }
                else
                {
                    c = c.AddOrUpdateInPlace(px.Key, Change<V>.Removed(px.Value));
                }
            }

            return (t, c);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        public TrieMap<EqK, K, V> Except(IEnumerable<K> other)
        {
            var self = this;
            foreach (var item in other)
            {
                self = self.Remove(item);
            }
            return self;
        }
        
        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) ExceptWithLog(IEnumerable<K> other)
        {
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            var self = this;
            
            foreach (var item in other)
            {
                var pair = self.RemoveWithLog(item);
                self = pair.Map;
                if (pair.Change.HasChanged)
                {
                    changes = changes.AddOrUpdateInPlace(item, pair.Change);
                }
            }
            return (self, changes);
        }        

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        public TrieMap<EqK, K, V> Except(IEnumerable<(K Key, V Value)> other)
        {
            var self = this;
            foreach (var item in other)
            {
                self = self.Remove(item.Key);
            }
            return self;
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) ExceptWithLog(
            IEnumerable<(K Key, V Value)> other) =>
            ExceptWithLog(other.Map(p => p.Key));
 
        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        public TrieMap<EqK, K, V> SymmetricExcept(TrieMap<EqK, K, V> rhs)
        {
            var self = this;
            
            foreach (var item in rhs)
            {
                var pair = self.RemoveWithLog(item.Key);
                if (pair.Change.HasNoChange)
                {
                    self = self.Add(item.Key, item.Value);
                }
            }
            return self;
        }        
        
        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) SymmetricExceptWithLog(TrieMap<EqK, K, V> rhs)
        {
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            var self = this;
            
            foreach (var item in rhs)
            {
                var pair = self.RemoveWithLog(item.Key);
                if (pair.Change.HasNoChange)
                {
                    self = self.Add(item.Key, item.Value);
                    changes = changes.AddOrUpdateInPlace(item.Key, Change<V>.Added(item.Value));
                }
            }
            return (self, changes);
        }        

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        public TrieMap<EqK, K, V> SymmetricExcept(IEnumerable<(K Key, V Value)> rhs)
        {
            var self = this;
            
            foreach (var item in rhs)
            {
                var pair = self.RemoveWithLog(item.Key);
                if (pair.Change.HasNoChange)
                {
                    self = self.Add(item.Key, item.Value);
                }
                else
                {
                    self = pair.Map;
                }
            }
            return self;
        }
        
        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) SymmetricExceptWithLog(IEnumerable<(K Key, V Value)> rhs)
        {
            var changes = TrieMap<EqK, K, Change<V>>.EmptyForMutating;
            var self = this;
            
            foreach (var item in rhs)
            {
                var pair = self.RemoveWithLog(item.Key);
                if (pair.Change.HasNoChange)
                {
                    self = self.Add(item.Key, item.Value);
                    changes = changes.AddOrUpdateInPlace(item.Key, Change<V>.Added(item.Value));
                }
                else
                {
                    self = pair.Map;
                    changes = changes.AddOrUpdateInPlace(item.Key, pair.Change);
                }
            }
            return (self, changes);
        }

        public TrieMap<EqK, K, V> Merge<SemigroupV>(TrieMap<EqK, K, V> rhs) where SemigroupV : struct, Semigroup<V>
        {
            var self = this;
            foreach (var iy in rhs)
            {
                var ix = self.Find(iy.Key);
                if (ix.IsSome)
                {
                    self = self.SetItem(iy.Key, default(SemigroupV).Append(ix.Value, iy.Value));
                }
                else
                {
                    self = self.Add(iy.Key, iy.Value);
                }
            }
            return self;
        }

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        public TrieMap<EqK, K, V> Union(IEnumerable<(K, V)> other) =>
            TryAddRange(other);

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) UnionWithLog(IEnumerable<(K, V)> other) =>
            TryAddRangeWithLog(other);
        
        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Union(
            IEnumerable<(K Key, V Value)> other,
            WhenMatched<K, V, V, V> Merge) =>
            Union(other, MapLeft: static (_, v) => v, MapRight: static (_, v) => v, Merge);

        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) UnionWithLog(
            IEnumerable<(K Key, V Value)> other,
            WhenMatched<K, V, V, V> Merge) =>
            UnionWithLog(other, MapLeft: static (_, v) => v, MapRight: static (_, v) => v, Merge);
        
        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing` function is called when there is a key in the right-hand side, but not the left-hand-side.
        /// This allows the `V2` value-type to be mapped to the target `V` value-type. 
        /// </remarks>
        public TrieMap<EqK, K, V> Union<W>(
            IEnumerable<(K Key, W Value)> other,
            WhenMissing<K, W, V> MapRight, 
            WhenMatched<K, V, W, V> Merge) =>
            Union(other, MapLeft: static (_, v) => v, MapRight, Merge);

        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing` function is called when there is a key in the right-hand side, but not the left-hand-side.
        /// This allows the `V2` value-type to be mapped to the target `V` value-type. 
        /// </remarks>
        public (TrieMap<EqK, K, V> Map, TrieMap<EqK, K, Change<V>> Changes) UnionWithLog<W>(
            IEnumerable<(K Key, W Value)> other, 
            WhenMissing<K, W, V> MapRight, 
            WhenMatched<K, V, W, V> Merge) =>
            UnionWithLog(other, MapLeft: static (_, v) => v, MapRight, Merge);

        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing` function is called when there is a key in the left-hand side, but not the right-hand-side.
        /// This allows the `V` value-type to be mapped to the target `V2` value-type. 
        /// </remarks>
        public TrieMap<EqK, K, W> Union<W>(
            IEnumerable<(K Key, W Value)> other,
            WhenMissing<K, V, W> MapLeft, 
            WhenMatched<K, V, W, W> Merge) =>
            Union(other, MapLeft, MapRight: static (_, v2) => v2, Merge);

        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing` function is called when there is a key in the left-hand side, but not the right-hand-side.
        /// This allows the `V` value-type to be mapped to the target `V2` value-type. 
        /// </remarks>
        public (TrieMap<EqK, K, W> Map, TrieMap<EqK, K, Change<W>> Changes) UnionWithLog<W>(
            IEnumerable<(K Key, W Value)> other,
            WhenMissing<K, V, W> MapLeft,
            WhenMatched<K, V, W, W> Merge) =>
            UnionWithLog(other, MapLeft, MapRight: static (_, v2) => v2, Merge);
        
        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing MapLeft` function is called when there is a key in the left-hand side, but not the
        /// right-hand-side.   This allows the `V` value-type to be mapped to the target `R` value-type. 
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing MapRight` function is called when there is a key in the right-hand side, but not the
        /// left-hand-side.   This allows the `V2` value-type to be mapped to the target `R` value-type. 
        /// </remarks>
        public TrieMap<EqK, K, R> Union<W, R>(
            IEnumerable<(K Key, W Value)> other, 
            WhenMissing<K, V, R> MapLeft, 
            WhenMissing<K, W, R> MapRight, 
            WhenMatched<K, V, W, R> Merge)
        {
            var t = TrieMap<EqK, K, R>.EmptyForMutating;
            foreach(var (key, value) in other)
            {
                var px = Find(key);
                t = t.AddOrUpdateInPlace(key, px.IsSome 
                    ? Merge(key, px.Value, value) 
                    : MapRight(key, value));
            }

            foreach (var (key, value) in this)
            {
                if (t.ContainsKey(key)) continue;
                t = t.AddOrUpdateInPlace(key, MapLeft(key, value));
            }

            return t;
        }
        
        /// <summary>
        /// Union two maps.  
        /// </summary>
        /// <remarks>
        /// The `WhenMatched` merge function is called when keys are present in both map to allow resolving to a
        /// sensible value.
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing MapLeft` function is called when there is a key in the left-hand side, but not the
        /// right-hand-side.   This allows the `V` value-type to be mapped to the target `R` value-type. 
        /// </remarks>
        /// <remarks>
        /// The `WhenMissing MapRight` function is called when there is a key in the right-hand side, but not the
        /// left-hand-side.   This allows the `V2` value-type to be mapped to the target `R` value-type. 
        /// </remarks>
        public (TrieMap<EqK, K, R> Map, TrieMap<EqK, K, Change<R>> Changes) UnionWithLog<W, R>(
            IEnumerable<(K Key, W Value)> other, 
            WhenMissing<K, V, R> MapLeft,
            WhenMissing<K, W, R> MapRight, 
            WhenMatched<K, V, W, R> Merge)
        {
            var t = TrieMap<EqK, K, R>.EmptyForMutating;
            var c = TrieMap<EqK, K, Change<R>>.EmptyForMutating;
            foreach(var (key, value) in other)
            {
                var px = Find(key);
                if (px.IsSome)
                {
                    var r = Merge(key, px.Value, value);
                    t = t.AddOrUpdateInPlace(key, r);
                    if (!EqDefault<V, W>.Equals(px.Value, r))
                    {
                        c = c.AddOrUpdateInPlace(key, Change<R>.Mapped(px.Value, r));
                    }
                }
                else
                {
                    var r = MapRight(key, value);
                    t = t.AddOrUpdateInPlace(key, r);
                    c = c.AddOrUpdateInPlace(key, Change<R>.Added(r));
                }
            }

            foreach (var (key, value) in this)
            {
                if (t.ContainsKey(key)) continue;
                
                var r = MapLeft(key, value);
                t = t.AddOrUpdateInPlace(key, r);
                if (!EqDefault<V, W>.Equals(value, r))
                {
                    c = c.AddOrUpdateInPlace(key, Change<R>.Mapped(value, r));
                }
            }

            return (t, c);
        }
        
        /// <summary>
        /// Nodes in the CHAMP hash trie map can be in one of three states:
        /// 
        ///     Empty - nothing in the map
        ///     Entries - contains items and sub-nodes
        ///     Collision - keeps track of items that have different keys but the same hash
        /// 
        /// </summary>
        internal interface Node : IEnumerable<(K, V)>
        {
            Tag Type { get; }
            (bool Found, K Key, V Value) Read(K key, uint hash, Sec section);
            (int CountDelta, Node Node, V Old) Update((UpdateType Type, bool Mutate) env, (K Key, V Value) change, uint hash, Sec section);
            (int CountDelta, Node Node, V Old) Remove(K key, uint hash, Sec section);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        //
        // NOTE: Here be dragons!  The code below is has been optimised for performace.  Yes, it's 
        //       ugly, yes there's repetition, but it's all to squeeze the last few nanoseconds of 
        //       performance out of the system.  Don't hate me ;)
        //

        /// <summary>
        /// Contains items and sub-nodes
        /// </summary>
        internal class Entries : Node
        {
            public readonly uint EntryMap;
            public readonly uint NodeMap;
            public readonly (K Key, V Value)[] Items;
            public readonly Node[] Nodes;

            public Tag Type => Tag.Entries;

            public Entries(uint entryMap, uint nodeMap, (K, V)[] items, Node[] nodes)
            {
                EntryMap = entryMap;
                NodeMap = nodeMap;
                Items = items;
                Nodes = nodes;
            }

            public void Deconstruct(out uint entryMap, out uint nodeMap, out (K, V)[] items, out Node[] nodes)
            {
                entryMap = EntryMap;
                nodeMap = NodeMap;
                items = Items;
                nodes = Nodes;
            }

            public (int CountDelta, Node Node, V Old) Remove(K key, uint hash, Sec section)
            {
                var hashIndex = Bit.Get(hash, section);
                var mask = Mask(hashIndex);

                if (Bit.Get(EntryMap, mask))
                {
                    // If key belongs to an entry
                    var ind = Index(EntryMap, mask);
                    if (default(EqK).Equals(Items[ind].Key, key))
                    {
                        var v = Items[ind].Value;
                        return (-1, 
                            new Entries(
                                Bit.Set(EntryMap, mask, false), 
                                NodeMap,
                                RemoveAt(Items, ind), 
                                Nodes),
                                v
                            );
                    }
                    else
                    {
                        return (0, this, default);
                    }
                }
                else if (Bit.Get(NodeMap, mask))
                {
                    //If key lies in a sub-node
                    var ind = Index(NodeMap, mask);
                    var (cd, subNode, v) = Nodes[ind].Remove(key, hash, section.Next());
                    if (cd == 0) return (0, this, default);

                    switch (subNode.Type)
                    {
                        case Tag.Entries:

                            var subEntries = (Entries)subNode;

                            if (subEntries.Items.Length == 1 && subEntries.Nodes.Length == 0)
                            {
                                // If the node only has one subnode, make that subnode the new node
                                if (Items.Length == 0 && Nodes.Length == 1)
                                {
                                    // Build a new Entries for this level with the sublevel mask fixed
                                    return (cd, new Entries(
                                            Mask(Bit.Get((uint)default(EqK).GetHashCode(subEntries.Items[0].Key),
                                                section)),
                                            0,
                                            Clone(subEntries.Items),
                                            Array.Empty<Node>()
                                        ),
                                        v);
                                }
                                else
                                {
                                    return (cd,
                                        new Entries(
                                            Bit.Set(EntryMap, mask, true),
                                            Bit.Set(NodeMap, mask, false),
                                            Insert(Items, Index(EntryMap, mask), subEntries.Items[0]),
                                            RemoveAt(Nodes, ind)),
                                        v);
                                }
                            }
                            else
                            {
                                var nodeCopy = Clone(Nodes);
                                nodeCopy[ind] = subNode;
                                return (cd, new Entries(EntryMap, NodeMap, Items, nodeCopy), v);
                            }

                        case Tag.Collision:
                            var nodeCopy2 = Clone(Nodes);
                            nodeCopy2[ind] = subNode;
                            return (cd, new Entries(EntryMap, NodeMap, Items, nodeCopy2), v);

                        default:
                            return (0, this, default);
                    }
                }
                else
                {
                    return (0, this, default);
                }
            }

            public (bool Found, K Key, V Value) Read(K key, uint hash, Sec section)
            {                                                                                         
                // var hashIndex = Bit.Get(hash, section);
                // Mask(hashIndex)
                var mask = (uint)(1 << (int)((hash & (uint)(Sec.Mask << section.Offset)) >> section.Offset));

                // if(Bit.Get(EntryMap, mask))
                if ((EntryMap & mask) == mask)                                                        
                {
                    // var entryIndex = Index(EntryMap, mask);
                    var entryIndex = BitCount((int)EntryMap & (((int)mask) - 1));                     
                    if (default(EqK).Equals(Items[entryIndex].Key, key))
                    {
                        var item = Items[entryIndex];
                        return (true, item.Key, item.Value);
                    }
                    else
                    {
                        return default;
                    }
                }
                // else if (Bit.Get(NodeMap, mask))
                else if ((NodeMap & mask) == mask)                                                   
                {
                    // var entryIndex = Index(NodeMap, mask);
                    var entryIndex = BitCount((int)NodeMap & (((int)mask) - 1));                     
                    return Nodes[entryIndex].Read(key, hash, section.Next());
                }
                else
                {
                    return default;
                }
            }

            public (int CountDelta, Node Node, V Old) Update((UpdateType Type, bool Mutate) env, (K Key, V Value) change, uint hash, Sec section)
            {
                // var hashIndex = Bit.Get(hash, section);
                // var mask = Mask(hashIndex);
                var mask = (uint)(1 << (int)((hash & (uint)(Sec.Mask << section.Offset)) >> section.Offset));

                //if (Bit.Get(EntryMap, mask))
                if((EntryMap & mask) == mask)
                {
                    //var entryIndex = Index(EntryMap, mask);
                    var entryIndex = BitCount((int)EntryMap & (((int)mask) - 1));
                    var currentEntry = Items[entryIndex];

                    if (default(EqK).Equals(currentEntry.Key, change.Key))
                    {
                        if (env.Type == UpdateType.Add)
                        {
                            // Key already exists - so it's an error to add again
                            throw new ArgumentException($"Key already exists in map: {change.Key}");
                        }
                        else if (env.Type == UpdateType.TryAdd)
                        {
                            // Already added, so we don't continue to try
                            return (0, this, default);
                        }

                        var (newItems, old) = SetItem(Items, entryIndex, change, env.Mutate);
                        return (0, new Entries(EntryMap, NodeMap, newItems, Nodes), old.Value);
                    }
                    else
                    {
                        if (env.Type == UpdateType.SetItem)
                        {
                            // Key must already exist to set it
                            throw new ArgumentException($"Key already exists in map: {change.Key}");
                        }
                        else if (env.Type == UpdateType.TrySetItem)
                        {
                            // Key doesn't exist, so there's nothing to set
                            return (0, this, default);
                        }

                        // Add
                        var node = Merge(change, currentEntry, hash, (uint)default(EqK).GetHashCode(currentEntry.Key), section);

                        //var newItems = Items.Filter(elem => !default(EqK).Equals(elem.Key, currentEntry.Key)).ToArray();
                        var newItems = new (K Key, V Value)[Items.Length - 1];
                        var i = 0;
                        foreach(var elem in Items)
                        {
                            if(!default(EqK).Equals(elem.Key, currentEntry.Key))
                            {
                                newItems[i] = elem;
                                i++;
                            }
                        }

                        //var newEntryMap = Bit.Set(EntryMap, mask, false);
                        var newEntryMap = EntryMap & (~mask);

                        // var newNodeMap = Bit.Set(NodeMap, mask, true);
                        var newNodeMap = NodeMap | mask;

                        // var nodeIndex = Index(NodeMap, mask);
                        var nodeIndex = BitCount((int)NodeMap & (((int)mask) - 1));

                        var newNodes = Insert(Nodes, nodeIndex, node);

                        return (1, new Entries(
                            newEntryMap, 
                            newNodeMap, 
                            newItems, 
                            newNodes), 
                            default);
                    }
                }
                else if (Bit.Get(NodeMap, mask))
                {
                    // var nodeIndex = Index(NodeMap, mask);
                    var nodeIndex = BitCount((int)NodeMap & (((int)mask) - 1));

                    var nodeToUpdate = Nodes[nodeIndex];
                    var (cd, newNode, ov) = nodeToUpdate.Update(env, change, hash, section.Next());
                    var (newNodes, _) = SetItem(Nodes, nodeIndex, newNode, env.Mutate);
                    return (cd, new Entries(EntryMap, NodeMap, Items, newNodes), ov);
                }
                else
                {
                    if (env.Type == UpdateType.SetItem)
                    {
                        // Key must already exist to set it
                        throw new ArgumentException($"Key doesn't exist in map: {change.Key}");
                    }
                    else if (env.Type == UpdateType.TrySetItem)
                    {
                        // Key doesn't exist, so there's nothing to set
                        return (0, this, default);
                    }

                    // var entryIndex = Index(EntryMap, mask);
                    var entryIndex = BitCount((int)EntryMap & (((int)mask) - 1));

                    // var entries = Bit.Set(EntryMap, mask, true);
                    var entries = EntryMap | mask;

                    var newItems = Insert(Items, entryIndex, change);
                    return (1, new Entries(entries, NodeMap, newItems, Nodes), default);
                }
            }

            public IEnumerator<(K, V)> GetEnumerator()
            {
                foreach (var item in Items)
                {
                    yield return item;
                }

                foreach (var node in Nodes)
                {
                    foreach (var item in node)
                    {
                        yield return item;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }

        /// <summary>
        /// Contains items that share the same hash but have different keys
        /// </summary>
        internal class Collision : Node
        {
            public readonly (K Key, V Value)[] Items;
            public readonly uint Hash;

            public Tag Type => Tag.Collision;

            public Collision((K Key, V Value)[] items, uint hash)
            {
                Items = items;
                Hash = hash;
            }

            public (bool Found, K Key, V Value) Read(K key, uint hash, Sec section)
            {
                foreach (var kv in Items)
                {
                    if (default(EqK).Equals(kv.Key, key))
                    {
                        return (true, kv.Key, kv.Value);
                    }
                }
                return default;
            }

            public (int CountDelta, Node Node, V Old) Remove(K key, uint hash, Sec section)
            {
                var len = Items.Length;
                if (len == 0) return (0, this, default);
                else if (len == 1) return (-1, EmptyNode.Default, Items[0].Value);
                else if (len == 2)
                {
                    var ((_, n, _), ov) = default(EqK).Equals(Items[0].Key, key)
                        ? (EmptyNode.Default.Update((UpdateType.Add, false), Items[1], hash, default), Items[0].Value)
                        : (EmptyNode.Default.Update((UpdateType.Add, false), Items[0], hash, default), Items[1].Value);

                    return (-1, n, ov);
                }
                else
                {
                    V oldValue = default;
                    IEnumerable<(K, V)> Yield((K Key, V Value)[] items, K ikey)
                    {
                        foreach (var item in items)
                        {
                            if (default(EqK).Equals(item.Key, ikey))
                            {
                                oldValue = item.Value;
                            }
                            else
                            {
                                yield return item;
                            }
                        }
                    }

                    var nitems = Yield(Items, key).ToArray();

                    return (nitems.Length - Items.Length, new Collision(nitems, hash), oldValue);
                }
            }

            public (int CountDelta, Node Node, V Old) Update((UpdateType Type, bool Mutate) env, (K Key, V Value) change, uint hash, Sec section)
            {
                var index = -1;
                for (var i = 0; i < Items.Length; i++)
                {
                    if (default(EqK).Equals(Items[i].Key, change.Key))
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                {
                    if (env.Type == UpdateType.Add)
                    {
                        // Key already exists - so it's an error to add again
                        throw new ArgumentException($"Key already exists in map: {change.Key}");
                    }
                    else if (env.Type == UpdateType.TryAdd)
                    {
                        // Already added, so we don't continue to try
                        return (0, this, default);
                    }

                    var (newArr, ov) = SetItem(Items, index, change, false);
                    return (0, new Collision(newArr, hash), ov.Value);
                }
                else
                {
                    if (env.Type == UpdateType.SetItem)
                    {
                        // Key must already exist to set it
                        throw new ArgumentException($"Key doesn't exist in map: {change.Key}");
                    }
                    else if (env.Type == UpdateType.TrySetItem)
                    {
                        // Key doesn't exist, so there's nothing to set
                        return (0, this, default);
                    }

                    var nitems = new (K, V)[Items.Length + 1];
                    System.Array.Copy(Items, nitems, Items.Length);
                    nitems[Items.Length] = change;
                    return (1, new Collision(nitems, hash), default);
                }
            }

            public IEnumerator<(K, V)> GetEnumerator() =>
                Items.AsEnumerable().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                Items.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Empty node
        /// </summary>
        internal class EmptyNode : Node
        {
            public static readonly EmptyNode Default = new EmptyNode();

            public Tag Type => Tag.Empty;

            public (bool Found, K Key, V Value) Read(K key, uint hash, Sec section) =>
                default;

            public (int CountDelta, Node Node, V Old) Remove(K key, uint hash, Sec section) =>
                (0, this, default);

            public (int CountDelta, Node Node, V Old) Update((UpdateType Type, bool Mutate) env, (K Key, V Value) change, uint hash, Sec section)
            {
                if (env.Type == UpdateType.SetItem)
                {
                    // Key must already exist to set it
                    throw new ArgumentException($"Key doesn't exist in map: {change.Key}");
                }
                else if (env.Type == UpdateType.TrySetItem)
                {
                    // Key doesn't exist, so there's nothing to set
                    return (0, this, default);
                }

                var dataMap = Mask(Bit.Get(hash, section));
                return (1, new Entries(dataMap, 0, new[] { change }, Array.Empty<Node>()), default);
            }

            public IEnumerator<(K, V)> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                yield break;
            }
        }

        /// <summary>
        /// Merges two key-value pairs into a single Node
        /// </summary>
        static Node Merge((K, V) pair1, (K, V) pair2, uint pair1Hash, uint pair2Hash, Sec section)
        {
            if (section.Offset >= 25)
            {
                return new Collision(new[] { pair1, pair2 }, pair1Hash);
            }
            else
            {
                var nextLevel = section.Next();
                var pair1Index = Bit.Get(pair1Hash, nextLevel);
                var pair2Index = Bit.Get(pair2Hash, nextLevel);
                if (pair1Index == pair2Index)
                {
                    var node = Merge(pair1, pair2, pair1Hash, pair2Hash, nextLevel);
                    var nodeMap = Mask(pair1Index);
                    return new Entries(0, nodeMap, new (K, V)[0], new[] { node });
                }
                else
                {
                    var dataMap = Mask(pair1Index);
                    dataMap = Bit.Set(dataMap, Mask(pair2Index), true);
                    return new Entries(dataMap, 0, pair1Index < pair2Index
                        ? new[] { pair1, pair2 }
                        : new[] { pair2, pair1 }, Array.Empty<Node>());
                }
            }
        }

        public IEnumerable<(K Key, V Value)> AsEnumerable() =>
            Root;

        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            Root.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Root.GetEnumerator();

        /// <summary>
        /// Counts the number of 1-bits in bitmap
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BitCount(int bits)
        {
            var c2 = bits - ((bits >> 1) & 0x55555555);
            var c4 = (c2 & 0x33333333) + ((c2 >> 2) & 0x33333333);
            var c8 = (c4 + (c4 >> 4)) & 0x0f0f0f0f;
            return (c8 * 0x01010101) >> 24;
        }

        /// <summary>
        /// Finds the number of set bits below the bit at `location`
        /// This function is used to find where in the array of entries or nodes 
        /// the item should be inserted
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Index(uint bits, int location) =>
            BitCount((int)bits & (location - 1));

        /// <summary>
        /// Finds the number of 1-bits below the bit at `location`
        /// This function is used to find where in the array of entries or nodes 
        /// the item should be inserted
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Index(uint bitmap, uint location) =>
            BitCount((int)bitmap & (((int)location) - 1));

        /// <summary>
        /// Returns the value used to index into the bit vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Mask(int index) =>
            (uint)(1 << index);

        /// <summary>
        /// Sets the item at index. If mutate is true it sets the 
        /// value without copying the array, otherwise the operation is pure
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (A[] Items, A Old) SetItem<A>(A[] items, int index, A value, bool mutate)
        {
            if (mutate)
            {
                var old = items[index]; 
                items[index] = value;
                return (items, old);
            }
            else
            {
                var old = items[index]; 
                var nitems = new A[items.Length];
                System.Array.Copy(items, nitems, items.Length);
                nitems[index] = value;
                return (nitems, old);
            }
        }

        /// <summary>
        /// Clones part of an existing array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static A[] Clone<A>(A[] items, int count)
        {
            var nitems = new A[count];
            System.Array.Copy(items, nitems, count);
            return nitems;
        }

        /// <summary>
        /// Clones an existing array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static A[] Clone<A>(A[] items)
        {
            var len = items.Length;
            var nitems = new A[len];
            System.Array.Copy(items, nitems, len);
            return nitems;
        }

        /// <summary>
        /// Inserts a new item in the array (immutably)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static A[] Insert<A>(A[] array, int index, A value)
        {
            var narray = new A[array.Length + 1];
            System.Array.Copy(array, 0, narray, 0, index);
            System.Array.Copy(array, index, narray, index + 1, array.Length - index);
            narray[index] = value;
            return narray;
        }

        /// <summary>
        /// Returns a new array with the item at index removed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static A[] RemoveAt<A>(A[] array, int index)
        {
            if (array.Length == 0)
            {
                return array;
            }

            var narray = new A[array.Length - 1];
            if (index > 0)
            {
                System.Array.Copy(array, 0, narray, 0, index);
            }
            if (index + 1 < array.Length)
            {
                System.Array.Copy(array, index + 1, narray, index, array.Length - index - 1);
            }
            return narray;
        }

        public override string ToString() =>
            count < 50
                ? $"[{ String.Join(", ", AsEnumerable().Select(TupleToString)) }]"
                : $"[{ String.Join(", ", AsEnumerable().Select(TupleToString).Take(50)) } ... ]";

        string TupleToString((K Key, V Value) tuple) =>
            $"({tuple.Key}, {tuple.Value})";

        public IEnumerable<K> Keys =>
            AsEnumerable().Select(kv => kv.Key);

        public IEnumerable<V> Values =>
            AsEnumerable().Select(kv => kv.Value);

        public bool TryGetValue(K key, out V value)
        {
            var ov = Find(key);
            if (ov.IsSome)
            {
                value = (V)ov;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() =>
            AsEnumerable().Select(kv => new KeyValuePair<K, V>(kv.Key, kv.Value))
                          .GetEnumerator();
    }

    internal struct Sec
    {
        public const int Mask = 31;
        public readonly int Offset;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sec(int offset) =>
            Offset = offset;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sec Next() =>
            new Sec(Offset + 5);
    }

    internal static class Bit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Set(uint value, int bit, bool flag) =>
            flag
                ? value | (uint)bit
                : value & (~(uint)bit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Set(uint value, uint bit, bool flag) =>
            flag
                ? value | bit
                : value & (~bit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Get(uint value, int bit) =>
            (value & (uint)bit) == (uint)bit;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Get(uint value, uint bit) =>
            (value & bit) == bit;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get(uint data, Sec section) =>
            (int)((data & (uint)(Sec.Mask << section.Offset)) >> section.Offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Set(uint data, Sec section, int value)
        {
            value <<= section.Offset;
            int offsetMask = (0xFFFF & (int)Sec.Mask) << section.Offset;
            return (data & ~(uint)offsetMask) | ((uint)value & (uint)offsetMask);
        }
    }
}
