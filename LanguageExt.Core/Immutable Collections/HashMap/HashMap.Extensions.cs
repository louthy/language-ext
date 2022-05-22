#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

public static class HashMapExtensions
{
    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K, V> ToHashMap<K, V>(this IEnumerable<(K, V)> items) =>
        LanguageExt.HashMap.createRange(items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K, V> ToHashMap<K, V>(this IEnumerable<Tuple<K, V>> items) =>
        LanguageExt.HashMap.createRange(items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K, V> ToHashMap<K, V>(this IEnumerable<KeyValuePair<K, V>> items) =>
        LanguageExt.HashMap.createRange(items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K1, HashMap<K2, V>> ToHashMap<K1, K2, V>(this IEnumerable<(K1, K2, V)> items) =>
        items.Fold(HashMap<K1, HashMap<K2, V>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K1, HashMap<K2, V>> ToHashMap<K1, K2, V>(this IEnumerable<Tuple<K1, K2, V>> items) =>
        items.Fold(HashMap<K1, HashMap<K2, V>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K1, HashMap<K2, HashMap<K3, V>>> ToHashMap<K1, K2, K3, V>(this IEnumerable<(K1, K2, K3, V)> items) =>
        items.Fold(HashMap<K1, HashMap<K2, HashMap<K3, V>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K1, HashMap<K2, HashMap<K3, V>>> ToHashMap<K1, K2, K3, V>(this IEnumerable<Tuple<K1, K2, K3, V>> items) =>
        items.Fold(HashMap<K1, HashMap<K2, HashMap<K3, V>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K1, HashMap<K2, HashMap<K3, HashMap<K4, V>>>> ToHashMap<K1, K2, K3, K4, V>(this IEnumerable<(K1, K2, K3, K4, V)> items) =>
        items.Fold(HashMap<K1, HashMap<K2, HashMap<K3, HashMap<K4, V>>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K1, HashMap<K2, HashMap<K3, HashMap<K4, V>>>> ToHashMap<K1, K2, K3, K4, V>(this IEnumerable<Tuple<K1, K2, K3, K4, V>> items) =>
        items.Fold(HashMap<K1, HashMap<K2, HashMap<K3, HashMap<K4, V>>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));

    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<K, V>(this HashMap<K, V> self) =>
        self.Count;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sum<K>(this HashMap<K, int> self) =>
        self.Values.Sum();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Find<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey) =>
        self.Find(outerKey, b => b.Find(innerKey), () => None);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Find<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey) =>
        self.Find(aKey, b => b.Find(bKey, c => c.Find(cKey), () => None), () => None);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Find<A, B, T, R>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey, Func<T, R> Some, Func<R> None) =>
        self.Find(outerKey, b => b.Find(innerKey, Some, None), None);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Find<A, B, C, T, R>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey, Some, None),
                None),
            None);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Find<A, B, C, D, T, R>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey,
                    d => d.Find(dKey, Some, None),
                    None),
                None),
            None);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<A, HashMap<B, T>> AddOrUpdate<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, Some, None),
            () => Prelude.HashMap(Tuple(innerKey, None()))
        );

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<A, HashMap<B, T>> AddOrUpdate<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey, T value) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, _ => value, value),
            () => Prelude.HashMap(Tuple(innerKey, value))
        );

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> AddOrUpdate<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, _ => value, value),
            () => Prelude.HashMap(Tuple(cKey, value))
        );

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> AddOrUpdate<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, Some, None),
            () => Prelude.HashMap(Tuple(cKey, None()))
        );

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, _ => value, value),
            () => Prelude.HashMap(Tuple(dKey, value))
        );

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, Some, None),
            () => Prelude.HashMap(Tuple(dKey, None()))
        );

    [Pure]
    public static HashMap<A, HashMap<B, T>> Remove<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey)
    {
        var b = self.Find(outerKey);
        if (b.IsSome)
        {
            var bv = b.Value.Remove(innerKey);
            if (bv.Count() == 0)
            {
                return self.Remove(outerKey);
            }
            else
            {
                return self.SetItem(outerKey, bv);
            }
        }
        else
        {
            return self;
        }
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> Remove<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey)
    {
        var b = self.Find(aKey);
        if (b.IsSome)
        {
            var c = b.Value.Find(bKey);
            if (c.IsSome)
            {
                var cv = c.Value.Remove(cKey);
                if (cv.Count() == 0)
                {
                    var bv = b.Value.Remove(bKey);
                    if (b.Value.Count() == 0)
                    {
                        return self.Remove(aKey);
                    }
                    else
                    {
                        return self.SetItem(aKey, bv);
                    }
                }
                else
                {
                    return self.SetItem(aKey, b.Value.SetItem(bKey, cv));
                }
            }
            else
            {
                return self;
            }
        }
        else
        {
            return self;
        }
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> MapRemoveT<A, B, T, V>(this HashMap<A, HashMap<B, T>> self, Func<HashMap<B, T>, HashMap<B, V>> map)
    {
        return self.Map((ka, va) => map(va))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> MapRemoveT<A, B, C, T, V>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<HashMap<C, T>, HashMap<C, V>> map)
    {
        return self.Map((ka, va) => va.Map((kb, vb) => map(vb))
                                      .Filter((kb, vb) => vb.Count > 0))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> MapRemoveT<A, B, C, D, T, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<HashMap<D, T>, HashMap<D, V>> map)
    {
        return self.Map((ka, va) => va.Map((kb, vb) => vb.Map((kc, vc) => map(vc))
                                                         .Filter((kc, vc) => vc.Count > 0))
                                      .Filter((kb, vb) => vb.Count > 0))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> MapT<A, B, T, V>(this HashMap<A, HashMap<B, T>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.Map(map));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> MapT<A, B, C, T, V>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.MapT(map));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> MapT<A, B, C, D, T, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.MapT(map));
    }

    [Pure]
    public static HashMap<A, HashMap<B, T>> FilterT<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.Filter(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> FilterT<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.FilterT(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> FilterT<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.FilterT(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, T>> FilterRemoveT<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> FilterRemoveT<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> FilterRemoveT<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static bool Exists<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool Exists<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool Exists<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool ForAll<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static bool ForAll<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static bool ForAll<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> SetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;
        return map.SetItem(aKey, av.SetItem(bKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> SetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> SetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> SetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;
        return map.SetItem(aKey, av.SetItem(bKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> SetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> SetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> TrySetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;
        return map.SetItem(aKey, av.TrySetItem(bKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> TrySetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> TrySetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> TrySetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;
        return map.SetItem(aKey, av.TrySetItem(bKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> TrySetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> TrySetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, Some));
    }

    [Pure]
    public static S FoldT<A, B, S, V>(this HashMap<A, HashMap<B, V>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.Fold(s, folder));
    }

    [Pure]
    public static S FoldT<A, B, C, S, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.FoldT(s, folder));
    }

    [Pure]
    public static S FoldT<A, B, C, D, S, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.FoldT(s, folder));
    }
}
