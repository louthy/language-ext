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

namespace LanguageExt
{
    /// <summary>
    /// Implementation of the CHAMP trie hash map data structure (Compressed Hash Array Map Trie)
    /// https://michael.steindorfer.name/publications/phd-thesis-efficient-immutable-collections.pdf
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
        const short PartitionMaxValue = 31;

        internal enum UpdateType
        {
            Add,
            TryAdd,
            AddOrUpdate,
            SetItem,
            TrySetItem
        }

        public static readonly TrieMap<EqK, K, V> Empty = new TrieMap<EqK, K, V>(EmptyNode.Default, 0);

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
                var hash = new BitVector32(default(EqK).GetHashCode(item.Key));
                var section = BitVector32.CreateSection(PartitionMaxValue);
                var (countDelta, newRoot) = Root.Update(type, true, item, hash, section);
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
        /// Try to add an item to the map.  If it already exists, do
        /// nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TryAdd(K key, V value) =>
            Update(key, value, UpdateType.TryAdd, false);

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
        public TrieMap<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, Func<V> None)
        {
            var value = Find(key);
            return value.IsSome
                ? AddOrUpdate(key, Some((V)value))
                : AddOrUpdate(key, None());
        }

        /// <summary>
        /// Add an item to the map, if it exists update the value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> AddOrUpdate(K key, Func<V, V> Some, V None)
        {
            var value = Find(key);
            return value.IsSome
                ? AddOrUpdate(key, Some((V)value))
                : AddOrUpdate(key, None);
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
        public TrieMap<EqK, K, V> AddRange(IEnumerable<KeyValuePair<K, V>> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.Add(item.Key, item.Value);
            }
            return self;
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
        public TrieMap<EqK, K, V> RemoveRange(IEnumerable<K> items)
        {
            var self = this;
            foreach (var item in items)
            {
                self = self.Remove(item);
            }
            return self;
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
        public TrieMap<EqK, K, V> SetItem(K key, Func<V, V> Some)
        {
            var value = Find(key).Map(Some).IfNone(() => throw new ArgumentException($"Key doesn't exist in map: {key}"));
            return SetItem(key, value);
        }

        /// <summary>
        /// Try to set an item that already exists in the map.  If none
        /// exists, do nothing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> TrySetItem(K key, V value) =>
            Update(key, value, UpdateType.TrySetItem, false);

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
        /// Update an item in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Update(K key, V value) =>
            Update(key, value, UpdateType.Add, false);

        /// <summary>
        /// Remove an item from the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Remove(K key)
        {
            var hashVector = new BitVector32(default(EqK).GetHashCode(key));
            var section = BitVector32.CreateSection(PartitionMaxValue);
            var (countDelta, newRoot) = Root.Remove(key, hashVector, section);
            return ReferenceEquals(newRoot, Root)
                ? this
                : new TrieMap<EqK, K, V>(newRoot, count + countDelta);
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public V this[K key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var value = Find(key);
                if (value.IsNone) throw new ArgumentException($"Key doesn't exist in map: {key}");
                return (V)value;
            }
        }

        /// <summary>
        /// Create an empty map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Clear() =>
            Empty;

        /// <summary>
        /// Get the hash code of the items in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            hash == 0
                ? (hash = hash(AsEnumerable()))
                : hash;

        /// <summary>
        /// Returns the whether the `key` exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(K key) =>
            Find(key).IsSome;

        /// <summary>
        /// Returns the whether the `key` exists in the map
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(K key, V Value) =>
            Find(key).Map(v => ReferenceEquals(v, Value) || (v?.Equals(Value) ?? false)).IfNone(false);

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
            var hash = new BitVector32(default(EqK).GetHashCode(key));
            var section = BitVector32.CreateSection(PartitionMaxValue);
            return Root.GetValue(key, hash, section);
        }

        /// <summary>
        /// Returns the value associated with `key` then match the result
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Find<R>(K key, Func<V, R> Some, Func<R> None) =>
            Find(key, Some, None);

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
        public (TrieMap<EqK, K, V> Map, V Value) FindOrAdd(K key, V value) =>
            Find(key, Some: v => (this, v), None: () => (Add(key, value), value));

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
        public (TrieMap<EqK, K, V> Map, Option<V> Value) FindOrMaybeAdd(K key, Option<V> value) =>
            Find(key, Some: v => (this, v), None: () =>
                value.IsSome
                    ? (Add(key, (V)value), value)
                    : (this, value));

        /// <summary>
        /// Returns the value associated with `key`.  Or None, if no key exists
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<V> FindSeq(K key) =>
            Find(key).ToSeq();

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
        public TrieMap<EqK, K, U> Map<U>(Func<K, V, U> f) =>
            new TrieMap<EqK, K, U>(AsEnumerable().Select(kv => (kv.Key, f(kv.Key, kv.Value))), false);

        /// <summary>
        /// Filter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Filter(Func<V, bool> f) =>
            new TrieMap<EqK, K, V>(AsEnumerable().Filter(kv => f(kv.Value)), false);

        /// <summary>
        /// Filter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Filter(Func<K, V, bool> f) =>
            new TrieMap<EqK, K, V>(AsEnumerable().Filter(kv => f(kv.Key, kv.Value)), false);

        /// <summary>
        /// Associative union
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrieMap<EqK, K, V> Append(TrieMap<EqK, K, V> rhs) =>
            TryAddRange(rhs.AsEnumerable());

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
            var iterA = GetEnumerator();
            var iterB = rhs.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                if (!default(EqK).Equals(iterA.Current.Key, iterB.Current.Key)) return false;
            }
            iterA = GetEnumerator();
            iterB = rhs.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                if (!default(EqV).Equals(iterA.Current.Value, iterB.Current.Value)) return false;
            }
            return true;
        }

        /// <summary>
        /// Update an item in the map - can mutate if needed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TrieMap<EqK, K, V> Update(K key, V value, UpdateType type, bool inplace)
        {
            var hash = new BitVector32(default(EqK).GetHashCode(key));
            var section = BitVector32.CreateSection(PartitionMaxValue);
            var (countDelta, newRoot) = Root.Update(type, inplace, (key, value), hash, section);
            return ReferenceEquals(newRoot, Root)
                ? this
                : new TrieMap<EqK, K, V>(newRoot, count + countDelta);
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
            Option<V> GetValue(K key, BitVector32 hash, BitVector32.Section section);
            (int CountDelta, Node Node) Update(UpdateType type, bool inplace, (K Key, V Value) change, BitVector32 hash, BitVector32.Section section);
            (int CountDelta, Node Node) Remove(K key, BitVector32 hash, BitVector32.Section section);
        }

        /// <summary>
        /// Contains items and sub-nodes
        /// </summary>
        internal class Entries : Node
        {
            public readonly BitVector32 EntryMap;
            public readonly BitVector32 NodeMap;
            public readonly (K Key, V Value)[] Items;
            public readonly Node[] Nodes;

            public Entries(BitVector32 entryMap, BitVector32 nodeMap, (K, V)[] items, Node[] nodes)
            {
                EntryMap = entryMap;
                NodeMap = nodeMap;
                Items = items;
                Nodes = nodes;
            }

            public void Deconstruct(out BitVector32 entryMap, out BitVector32 nodeMap, out (K, V)[] items, out Node[] nodes)
            {
                entryMap = EntryMap;
                nodeMap = NodeMap;
                items = Items;
                nodes = Nodes;
            }

            public (int CountDelta, Node Node) Remove(K key, BitVector32 hash, BitVector32.Section section)
            {
                var hashIndex = hash[section];
                var mask = Mask(hashIndex);

                // If key belongs to an entry
                if (EntryMap[mask])
                {
                    var ind = Index(EntryMap, mask);
                    if (default(EqK).Equals(Items[ind].Key, key))
                    {
                        var newMap = new BitVector32(EntryMap);
                        newMap[mask] = false;
                        var newItems = RemoveAt(Items, ind);
                        return (-1, new Entries(newMap, NodeMap, newItems, Nodes));
                    }
                    else
                    {
                        return (0, this);
                    }
                }
                //If key lies in a subnode
                else if (NodeMap[mask])
                {
                    var ind = Index(NodeMap, mask);
                    var (cd, subNode) = Nodes[ind].Remove(key, hash, BitVector32.CreateSection(PartitionMaxValue, section));
                    switch (subNode)
                    {
                        case Entries bitmapNode:
                            var (subItemMap, subNodeMap, subItems, subNodes) = bitmapNode;
                            if (subItems.Length == 1 && subNodes.Length == 0)
                            {
                                // If the node only has one subnode, make that subnode the new node
                                if (Items.Length == 0 && Nodes.Length == 1)
                                {
                                    return (cd, new Entries(subItemMap, subNodeMap, subItems, subNodes));
                                }
                                else
                                {
                                    var indexToInsert = Index(EntryMap, mask);
                                    var newNodeMap = new BitVector32(NodeMap);
                                    var newEntryMap = new BitVector32(EntryMap);
                                    newNodeMap[mask] = false;
                                    newEntryMap[mask] = true;
                                    var newEntries = Insert(Items, indexToInsert, subItems[0]);
                                    var newNodes = RemoveAt(Nodes, ind);
                                    return (cd, new Entries(newEntryMap, newNodeMap, newEntries, newNodes));
                                }
                            }
                            else
                            {
                                var nodeCopy = new Node[Nodes.Length];
                                System.Array.Copy(Nodes, nodeCopy, Nodes.Length);
                                nodeCopy[ind] = subNode;
                                return (cd, new Entries(EntryMap, NodeMap, Items, nodeCopy));
                            }

                        case Collision collNode:
                            var nodeCopy2 = new Node[Nodes.Length];
                            System.Array.Copy(Nodes, nodeCopy2, Nodes.Length);
                            nodeCopy2[ind] = subNode;
                            return (cd, new Entries(EntryMap, NodeMap, Items, nodeCopy2));

                        default:
                            return (cd, this);
                    }
                }
                else
                {
                    return (0, this);
                }
            }

            public Option<V> GetValue(K key, BitVector32 hash, BitVector32.Section section)
            {
                var hashIndex = hash[section];
                var mask = Mask(hashIndex);
                if (EntryMap[mask])
                {
                    var entryIndex = Index(EntryMap, mask);
                    if (default(EqK).Equals(Items[entryIndex].Key, key))
                    {
                        return Items[entryIndex].Value;
                    }
                    else
                    {
                        return None;
                    }
                }
                else if (NodeMap[mask])
                {
                    var subNode = Nodes[Index(NodeMap, mask)];
                    return subNode.GetValue(key, hash, BitVector32.CreateSection(PartitionMaxValue, section));
                }
                else
                {
                    return None;
                }
            }

            public (int CountDelta, Node Node) Update(UpdateType type, bool inplace, (K Key, V Value) change, BitVector32 hash, BitVector32.Section section)
            {
                var hashIndex = hash[section];
                var mask = Mask(hashIndex);
                if (EntryMap[mask])
                {
                    var entryIndex = Index(EntryMap, mask);
                    if (default(EqK).Equals(Items[entryIndex].Key, change.Key))
                    {
                        if (type == UpdateType.Add)
                        {
                            // Key already exists - so it's an error to add again
                            throw new ArgumentException($"Key already exists in map: {change.Key}");
                        }
                        else if (type == UpdateType.TryAdd)
                        {
                            // Already added, so we don't continue to try
                            return (0, this);
                        }

                        var newItems = Set(Items, entryIndex, change, inplace);
                        return (0, new Entries(EntryMap, NodeMap, newItems, Nodes));
                    }
                    else
                    {
                        if (type == UpdateType.SetItem)
                        {
                            // Key must already exist to set it
                            throw new ArgumentException($"Key already exists in map: {change.Key}");
                        }
                        else if (type == UpdateType.TrySetItem)
                        {
                            // Key doesn't exist, so there's nothing to set
                            return (0, this);
                        }

                        // Add
                        var currentEntry = Items[entryIndex];
                        var currentHash = new BitVector32(default(EqK).GetHashCode(currentEntry.Key));
                        var node = Merge(change, currentEntry, hash, currentHash, section);
                        var newItems = Items.Filter(elem => !default(EqK).Equals(elem.Key, currentEntry.Key)).ToArray();
                        var newEntryMap = new BitVector32(EntryMap);
                        newEntryMap[mask] = false;
                        var newNodeMap = new BitVector32(NodeMap);
                        newNodeMap[mask] = true;
                        var nodeIndex = Index(NodeMap, mask);
                        var newNodes = Insert(Nodes, nodeIndex, node);
                        return (1, new Entries(newEntryMap, newNodeMap, newItems, newNodes));
                    }
                }
                else if (NodeMap[mask])
                {
                    var nodeIndex = Index(NodeMap, mask);
                    var nodeToUpdate = Nodes[nodeIndex];
                    var (cd, newNode) = nodeToUpdate.Update(type, inplace, change, hash, BitVector32.CreateSection(PartitionMaxValue, section));
                    var newNodes = Set(Nodes, nodeIndex, newNode, inplace);
                    return (cd, new Entries(EntryMap, NodeMap, Items, newNodes));
                }
                else
                {
                    if (type == UpdateType.SetItem)
                    {
                        // Key must already exist to set it
                        throw new ArgumentException($"Key doesn't exist in map: {change.Key}");
                    }
                    else if (type == UpdateType.TrySetItem)
                    {
                        // Key doesn't exist, so there's nothing to set
                        return (0, this);
                    }

                    var entryIndex = Index(EntryMap, mask);
                    var entries = new BitVector32(EntryMap);
                    entries[mask] = true;
                    var newItems = Insert(Items, entryIndex, change);
                    return (1, new Entries(entries, NodeMap, newItems, Nodes));
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
            public readonly BitVector32 Hash;

            public Collision((K Key, V Value)[] items, BitVector32 hash)
            {
                Items = items;
                Hash = hash;
            }

            public Option<V> GetValue(K key, BitVector32 hash, BitVector32.Section section)
            {
                foreach (var kv in Items)
                {
                    if (default(EqK).Equals(kv.Key, key))
                    {
                        return Some(kv.Value);
                    }
                }
                return None;
            }

            public (int CountDelta, Node Node) Remove(K key, BitVector32 hash, BitVector32.Section section)
            {
                var len = Items.Length;
                if (len == 0) return (0, this);
                else if (len == 1) return (-1, EmptyNode.Default);
                else if (len == 2)
                {
                    var (_, n) = default(EqK).Equals(Items[0].Key, key)
                        ? EmptyNode.Default.Update(UpdateType.Add, false, Items[1], hash, BitVector32.CreateSection(PartitionMaxValue))
                        : EmptyNode.Default.Update(UpdateType.Add, false, Items[0], hash, BitVector32.CreateSection(PartitionMaxValue));

                    return (-1, n);
                }
                else
                {
                    IEnumerable<(K, V)> Yield((K Key, V Value)[] items, K ikey)
                    {
                        foreach (var item in items)
                        {
                            if (!default(EqK).Equals(item.Key, ikey))
                            {
                                yield return item;
                            }
                        }
                    }

                    var nitems = Yield(Items, key).ToArray();

                    return (nitems.Length - Items.Length, new Collision(nitems, hash));
                }
            }

            public (int CountDelta, Node Node) Update(UpdateType type, bool inplace, (K Key, V Value) change, BitVector32 hash, BitVector32.Section section)
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
                    if (type == UpdateType.Add)
                    {
                        // Key already exists - so it's an error to add again
                        throw new ArgumentException($"Key already exists in map: {change.Key}");
                    }
                    else if (type == UpdateType.TryAdd)
                    {
                        // Already added, so we don't continue to try
                        return (0, this);
                    }

                    var newArr = Set(Items, index, change, false);
                    return (0, new Collision(newArr, hash));
                }
                else
                {
                    if (type == UpdateType.SetItem)
                    {
                        // Key must already exist to set it
                        throw new ArgumentException($"Key doesn't exist in map: {change.Key}");
                    }
                    else if (type == UpdateType.TrySetItem)
                    {
                        // Key doesn't exist, so there's nothing to set
                        return (0, this);
                    }

                    var nitems = new (K, V)[Items.Length + 1];
                    System.Array.Copy(Items, nitems, Items.Length);
                    nitems[Items.Length] = change;
                    return (1, new Collision(nitems, hash));
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

            public Option<V> GetValue(K key, BitVector32 hash, BitVector32.Section section) =>
                None;

            public (int CountDelta, Node Node) Remove(K key, BitVector32 hash, BitVector32.Section section) =>
                (0, this);

            public (int CountDelta, Node Node) Update(UpdateType type, bool inplace, (K Key, V Value) change, BitVector32 hash, BitVector32.Section section)
            {
                if (type == UpdateType.SetItem)
                {
                    // Key must already exist to set it
                    throw new ArgumentException($"Key doesn't exist in map: {change.Key}");
                }
                else if (type == UpdateType.TrySetItem)
                {
                    // Key doesn't exist, so there's nothing to set
                    return (0, this);
                }

                var dataMap = new BitVector32(Mask(hash[section]));
                var items = new[] { change };
                var nodes = new Node[0];
                var nodeMap = new BitVector32(0);
                return (1, new Entries(dataMap, nodeMap, items, nodes));
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
        /// Merges two key-value pairs into a Node
        /// </summary>
        static Node Merge((K, V) pair1, (K, V) pair2, BitVector32 pair1Hash, BitVector32 pair2Hash, BitVector32.Section section)
        {
            if (section.Offset >= 25)
            {
                return new Collision(new[] { pair1, pair2 }, pair1Hash);
            }
            else
            {
                var nextLevel = BitVector32.CreateSection(PartitionMaxValue, section);
                var pair1Index = pair1Hash[nextLevel];
                var pair2Index = pair2Hash[nextLevel];
                if (pair1Index == pair2Index)
                {
                    var node = Merge(pair1, pair2, pair1Hash, pair2Hash, nextLevel);
                    var nodeMap = new BitVector32(Mask(pair1Index));
                    return new Entries(new BitVector32(0), nodeMap, new (K, V)[0], new[] { node });
                }
                else
                {
                    var dataMap = new BitVector32(Mask(pair1Index));
                    dataMap[Mask(pair2Index)] = true;
                    return new Entries(dataMap, new BitVector32(0), pair1Index < pair2Index
                        ? new[] { pair1, pair2 }
                        : new[] { pair2, pair1 }, new Node[0]);
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
        static int BitCount(int bitmap)
        {
            var count2 = bitmap - ((bitmap >> 1) & 0x55555555);
            var count4 = (count2 & 0x33333333) + ((count2 >> 2) & 0x33333333);
            var count8 = (count4 + (count4 >> 4)) & 0x0f0f0f0f;
            return (count8 * 0x01010101) >> 24;
        }

        /// <summary>
        /// Finds the number of 1-bits below the bit at "pos"
        /// Since the array in the ChampHashMap is compressed (does not have room for unfilled values)
        /// This function is used to find where in the array of entries or nodes the item should be inserted
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Index(BitVector32 bitmap, int pos) =>
            BitCount(bitmap.Data & (pos - 1));

        /// <summary>
        /// Returns the value used to index into the BitVector.  
        /// The BitVector must be accessed by powers of 2, so to get the nth bit
        /// the mask must be 2^n
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Mask(int index) =>
            1 << index;

        /// <summary>
        /// Sets the value at index "index" to "value". If inplace is true it sets the 
        /// value without copying the array. Otherwise it returns a new copy
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static A[] Set<A>(A[] items, int index, A value, bool inplace)
        {
            if (inplace)
            {
                items[index] = value;
                return items;
            }
            else
            {
                var nitems = new A[items.Length];
                System.Array.Copy(items, nitems, items.Length);
                nitems[index] = value;
                return nitems;
            }
        }

        /// <summary>
        /// Inserts a new element in an array at index "index"
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
}
