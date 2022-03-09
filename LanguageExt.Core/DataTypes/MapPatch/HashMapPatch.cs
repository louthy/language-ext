using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Represents change to a Hash-Map
    /// </summary>
    /// <remarks>
    /// This is primarily used by the `Change` events on the `AtomHashMap` types,
    /// and the `Changes` property of `TrackingHashMap`. 
    /// </remarks>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public sealed class HashMapPatch<K, V>
    {
        readonly TrieMap<EqDefault<K>, K, V> prev;
        readonly TrieMap<EqDefault<K>, K, V> curr;
        readonly TrieMap<EqDefault<K>, K, Change<V>> changes;
        readonly K key;
        readonly Change<V> change;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal HashMapPatch(
            TrieMap<EqDefault<K>, K, V> prev,
            TrieMap<EqDefault<K>, K, V> curr,
            TrieMap<EqDefault<K>, K, Change<V>> changes)
        {
            this.prev = prev;
            this.curr = curr;
            this.changes = changes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal HashMapPatch(
            TrieMap<EqDefault<K>, K, V> prev,
            TrieMap<EqDefault<K>, K, V> curr,
            K key,
            Change<V> change)
        {
            this.prev = prev;
            this.curr = curr;
            this.key = key;
            this.change = change;
        }

        public HashMap<K, V> From
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(prev);
        }

        public HashMap<K, V> To
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(curr);
        }

        public HashMap<K, Change<V>> Changes
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => isnull(changes)
                ? HashMap<K, Change<V>>.Empty.Add(key, change)
                : new HashMap<K, Change<V>>(changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => 
            Changes.ToString();
    }
  
    /// <summary>
    /// Represents change to a Hash-Map
    /// </summary>
    /// <remarks>
    /// This is primarily used by the `Change` events on the `AtomHashMap` types,
    /// and the `Changes` property of `TrackingHashMap`. 
    /// </remarks>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public sealed class HashMapPatch<EqK, K, V>
        where EqK : struct, Eq<K>
    {
        readonly TrieMap<EqK, K, V> prev;
        readonly TrieMap<EqK, K, V> curr;
        readonly TrieMap<EqK, K, Change<V>> changes;
        readonly K key;
        readonly Change<V> change;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal HashMapPatch(
            TrieMap<EqK, K, V> prev,
            TrieMap<EqK, K, V> curr,
            TrieMap<EqK, K, Change<V>> changes)
        {
            this.prev = prev;
            this.curr = curr;
            this.changes = changes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal HashMapPatch(
            TrieMap<EqK, K, V> prev,
            TrieMap<EqK, K, V> curr,
            K key,
            Change<V> change)
        {
            this.prev = prev;
            this.curr = curr;
            this.key = key;
            this.change = change;
        }
 
        public HashMap<EqK, K, V> From
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new (prev);
        }

        public HashMap<EqK, K, V> To
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new (curr);
        }

        public HashMap<EqK, K, Change<V>> Changes
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => isnull(changes)
                ? HashMap<EqK, K, Change<V>>.Empty.Add(key, change) 
                : new HashMap<EqK, K, Change<V>>(changes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => 
            Changes.ToString();
    }
}
