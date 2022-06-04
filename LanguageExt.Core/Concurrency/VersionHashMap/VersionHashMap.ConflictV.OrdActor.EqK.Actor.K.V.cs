#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// <para>
    /// Versioned hash-map.  Each value has a vector-clock attached to it which understands the causality of updates
    /// from multiple actors.  Actors are just unique keys that represent individual contributors.  They could be client
    /// connections, nodes in a network, users, or whatever is appropriate to discriminate between commits.
    /// </para>
    /// <para>
    /// Deleted items are not removed from the hash-map, they are merely marked as deleted.  This allows conflicts between
    /// writes and deletes to be resolved.
    /// </para> 
    /// <para>
    /// Run `RemoveDeletedItemsOlderThan` to clean up items that have been deleted and are now just hanging around.  Use
    /// a big enough delay that it won't conflict with other commits (this could be seconds, minutes, or longer:
    /// depending on the expected latency of writes to the hash-map).
    /// </para>
    /// </summary>
    public class VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V> :
        IEnumerable<(K Key, V Value)>,
        IEquatable<VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V>>
        where OrdActor  : struct, Ord<Actor>
        where EqK       : struct, Eq<K>
        where ConflictV : struct, Conflict<V>
    {
        internal volatile TrieMap<EqK, K, VersionVector<ConflictV, OrdActor, TLong, Actor, long, V>> Items;

        /// <summary>
        /// Empty version map
        /// </summary>
        public static VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V>(TrieMap<EqK, K, VersionVector<ConflictV, OrdActor, TLong, Actor, long, V>>.Empty);
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">Trie map</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        VersionHashMap(TrieMap<EqK, K, VersionVector<ConflictV, OrdActor, TLong, Actor, long, V>> items) =>
            this.Items = items;

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Version - this may be in a state of never existing, but won't ever fail</returns>
        [Pure]
        public Version<Actor, K, V> this[K key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FindVersion(key);
        }

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
        /// Atomically swap a key in the map.  Allows for multiple operations on the hash-map in an entirely
        /// transactional and atomic way.
        /// </summary>
        /// <param name="swap">Swap function, maps the current state of the value associated with the key to a new state</param>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit SwapKey(K key, Func<Version<Actor, K, V>, Version<Actor, K, V>> swap)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems   = Items;
                var okey     = oitems.Find(key)
                                     .Map(v => v.ToVersion(key))
                                     .IfNone(() => VersionNeverExistedVector<ConflictV, OrdActor, Actor, K, V>.New(key));
                
                var nversion = swap(okey);

                var nitems = oitems.AddOrMaybeUpdate(key,
                                                     exists => exists.Put(nversion.ToVector<ConflictV, OrdActor, Actor, K, V>()),
                                                     #nullable disable
                                                     () => Optional(nversion.ToVector<ConflictV, OrdActor, Actor, K, V>()));
                                                      #nullable enable
                
                if(ReferenceEquals(oitems, nitems))
                {
                    // no change
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }
                
        /// <summary>
        /// Atomically updates a new item in the map.  If the key already exists, then the vector clocks, within the version
        /// vector, are compared to ascertain if the proposed version was caused-by, causes, or conflicts with the current
        /// version:
        ///
        ///     * If the proposed version was caused-by the current version, then it is ignored.
        ///     * If the proposed version causes the current version, then it is accepted and updated as the latest version
        ///     * If the proposed version is in conflict with the current version, then values from both versions are
        ///       passed to ConflictV.Append(v1, v2) for value resolution, and the pairwise-maximum of the two vector-clocks
        ///       is taken to be the new 'time.
        /// 
        /// </summary>
        /// <param name="version">Version to update</param>
        public Unit Update(Version<Actor, K, V> nversion)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.AddOrMaybeUpdate(nversion.Key,
                                                     exists => exists.Put(nversion.ToVector<ConflictV, OrdActor, Actor, K, V>()),
                                                     #nullable disable
                                                     () => Optional(nversion.ToVector<ConflictV, OrdActor, Actor, K, V>()));
                                                      #nullable enable
                
                if(ReferenceEquals(oitems, nitems))
                {
                    // no change
                    return default;
                }
                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Remove items that are older than the specified time-stamp
        /// </summary>
        /// <param name="cutoff">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveOlderThan(DateTime cutoff) =>
            RemoveOlderThan(cutoff.ToUniversalTime().Ticks);

        /// <summary>
        /// Remove items that are older than the specified time-stamp
        /// </summary>
        /// <param name="timeStamp">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveOlderThan(long timeStamp) =>
            FilterInPlace((ts, _) => ts > timeStamp);

        /// <summary>
        /// Remove deleted items that are older than the specified time-stamp
        /// </summary>
        /// <param name="cutoff">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveDeletedItemsOlderThan(DateTime cutoff) =>
            RemoveDeletedItemsOlderThan(cutoff.ToUniversalTime().Ticks);

        /// <summary>
        /// Remove deleted items that are older than the specified time-stamp
        /// </summary>
        /// <param name="timeStamp">Cut off time-stamp</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit RemoveDeletedItemsOlderThan(long timeStamp) =>
            FilterInPlace((ts, v) => v.IsSome || ts > timeStamp);
        
        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<V> Find(K value) =>
            Items.Find(value).Bind(static v => v.Value);

        /// <summary>
        /// Retrieve a value from the map by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Version<Actor, K, V> FindVersion(K key) =>
            Items.Find(key).Match(
                Some: v => v.ToVersion(key),
                None: () => VersionNeverExistedVector<ConflictV, OrdActor, Actor, K, V>.New(key));

        /// <summary>
        /// Enumerable of keys
        /// </summary>
        [Pure]
        public IEnumerable<K> Keys
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsEnumerable().Map(static x => x.Key);
        }

        /// <summary>
        /// Enumerable of value
        /// </summary>
        [Pure]
        public IEnumerable<V> Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsEnumerable().Map(static x => x.Value);
        }

        /// <summary>
        /// Convert to a HashMap
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashMap<K, V> ToHashMap() =>
            Items.AsEnumerable().Choose(static x => x.Value.Value.Map(v => (x.Key, v))).ToHashMap();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<(K Key, V Value)> GetEnumerator() =>
            Items.AsEnumerable().Choose(static x => x.Value.Value.Map(v => (x.Key, v))).GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            Items.GetEnumerator();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<(K Key, V Value)> ToSeq() =>
            toSeq(AsEnumerable());

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// The ellipsis is used for collections over 50 items
        /// To get a formatted string with all the items, use `ToFullString`
        /// or `ToFullArrayString`.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() =>
            CollectionFormat.ToShortArrayString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), Count);

        /// <summary>
        /// Format the collection as `(key: value), (key: value), (key: value), ...`
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToFullString(string separator = ", ") =>
            CollectionFormat.ToFullString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), separator);

        /// <summary>
        /// Format the collection as `[(key: value), (key: value), (key: value), ...]`
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToFullArrayString(string separator = ", ") =>
            CollectionFormat.ToFullArrayString(AsEnumerable().Map(kv => $"({kv.Key}: {kv.Value})"), separator);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(K Key, V Value)> AsEnumerable() =>
            Items.AsEnumerable().Choose(static x => x.Value.Value.Map(v => (x.Key, v)));

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            ToHashMap().IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSubsetOf(IEnumerable<K> other) =>
            Items.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            ToHashMap().IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProperSupersetOf(IEnumerable<K> other) =>
            Items.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubsetOf(IEnumerable<(K Key, V Value)> other) =>
            ToHashMap().IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubsetOf(IEnumerable<K> other) =>
            Items.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSubsetOf(HashMap<K, V> other) =>
            ToHashMap().IsSubsetOf(other.Value);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSupersetOf(IEnumerable<(K Key, V Value)> other) =>
            ToHashMap().IsSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                var nitems = this.Items.Intersect(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(IEnumerable<(K Key, V Value)> other) =>
            ToHashMap().Overlaps(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(IEnumerable<K> other) =>
            ToHashMap().Overlaps(other);

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
                var nitems = this.Items.Except(srhs);
                if (ReferenceEquals(Interlocked.CompareExchange(ref this.Items, nitems, oitems), oitems))
                {
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) =>
            obj is VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V> hm && Equals(hm);

        /// <summary>
        /// Equality of keys and values with `EqDefault<V>` used for values
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V>? other) =>
            other is not null && Items.Equals(other.Items);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            Items.GetHashCode();

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V> Where(Func<long, Option<V>, bool> pred) =>
            new VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V>(Items.Filter(v => pred(v.TimeStamp, v.Value)));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V> Where(Func<K, long, Option<V>, bool> pred) =>
            new VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V>(Items.Filter((k, v) => pred(k, v.TimeStamp, v.Value)));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V> Filter(Func<long, Option<V>, bool> pred) =>
            new VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V>(Items.Filter(v => pred(v.TimeStamp, v.Value)));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V> Filter(Func<K, long, Option<V>, bool> pred) =>
            new VersionHashMap<ConflictV, OrdActor, EqK, Actor, K, V>(Items.Filter((k, v) => pred(k, v.TimeStamp, v.Value)));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        public Unit FilterInPlace(Func<long, Option<V>, bool> pred)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.Filter(v => pred(v.TimeStamp, v.Value));

                if (ReferenceEquals(oitems, nitems))
                {
                    // no change
                    return default;
                }

                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
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
        public Unit FilterInPlace(Func<K, long, Option<V>, bool> pred)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = Items;
                var nitems = oitems.Filter((k, v) => pred(k, v.TimeStamp, v.Value));

                if (ReferenceEquals(oitems, nitems))
                {
                    // no change
                    return default;
                }

                if (ReferenceEquals(Interlocked.CompareExchange(ref Items, nitems, oitems), oitems))
                {
                    return default;
                }
                else
                {
                    sw.SpinOnce();
                }
            }
        }

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<(K Key, V Value), bool> pred) =>
            AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<KeyValuePair<K, V>, bool> pred) =>
            AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).ForAll(pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<V, bool> pred) =>
            AsEnumerable().Map(static x => x.Value).ForAll(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(Func<(K Key, V Value), bool> pred) =>
            AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).Exists(pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(Func<V, bool> pred) =>
            AsEnumerable().Map(static x => x.Value).Exists(pred);

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S Fold<S>(S state, Func<S, V, S> folder) =>
            AsEnumerable().Map(static x => x.Value).Fold(state, folder);
    }
}
#nullable disable
