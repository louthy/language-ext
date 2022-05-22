#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;

public static partial class TrackingHashMapExtensions
{
    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K, V> ToTrackingHashMap<K, V>(this IEnumerable<(K, V)> items) =>
        LanguageExt.TrackingHashMap.createRange(items);

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K, V> ToTrackingHashMap<K, V>(this IEnumerable<Tuple<K, V>> items) =>
        LanguageExt.TrackingHashMap.createRange(items);

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K, V> ToTrackingHashMap<K, V>(this IEnumerable<KeyValuePair<K, V>> items) =>
        LanguageExt.TrackingHashMap.createRange(items);

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K1, TrackingHashMap<K2, V>> ToTrackingHashMap<K1, K2, V>(this IEnumerable<(K1, K2, V)> items) =>
        items.Fold(TrackingHashMap<K1, TrackingHashMap<K2, V>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3));

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K1, TrackingHashMap<K2, V>> ToTrackingHashMap<K1, K2, V>(this IEnumerable<Tuple<K1, K2, V>> items) =>
        items.Fold(TrackingHashMap<K1, TrackingHashMap<K2, V>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3));

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, V>>> ToTrackingHashMap<K1, K2, K3, V>(this IEnumerable<(K1, K2, K3, V)> items) =>
        items.Fold(TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, V>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4));

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, V>>> ToTrackingHashMap<K1, K2, K3, V>(this IEnumerable<Tuple<K1, K2, K3, V>> items) =>
        items.Fold(TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, V>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4));

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, TrackingHashMap<K4, V>>>> ToTrackingHashMap<K1, K2, K3, K4, V>(this IEnumerable<(K1, K2, K3, K4, V)> items) =>
        items.Fold(TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, TrackingHashMap<K4, V>>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, TrackingHashMap<K4, V>>>> ToTrackingHashMap<K1, K2, K3, K4, V>(this IEnumerable<Tuple<K1, K2, K3, K4, V>> items) =>
        items.Fold(TrackingHashMap<K1, TrackingHashMap<K2, TrackingHashMap<K3, TrackingHashMap<K4, V>>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));

    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    public static int Count<K, V>(this TrackingHashMap<K, V> self) =>
        self.Count;

    [Pure]
    public static int Sum<K>(this TrackingHashMap<K, int> self) =>
        self.Values.Sum();

    [Pure]
    public static Option<T> Find<A, B, T>(this TrackingHashMap<A, TrackingHashMap<B, T>> self, A outerKey, B innerKey) =>
        self.Find(outerKey, b => b.Find(innerKey), () => None);

    [Pure]
    public static Option<T> Find<A, B, C, T>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> self, A aKey, B bKey, C cKey) =>
        self.Find(aKey, b => b.Find(bKey, c => c.Find(cKey), () => None), () => None);

    [Pure]
    public static R Find<A, B, T, R>(this TrackingHashMap<A, TrackingHashMap<B, T>> self, A outerKey, B innerKey, Func<T, R> Some, Func<R> None) =>
        self.Find(outerKey, b => b.Find(innerKey, Some, None), None);

    [Pure]
    public static R Find<A, B, C, T, R>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey, Some, None),
                None),
            None);

    [Pure]
    public static R Find<A, B, C, D, T, R>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, TrackingHashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey,
                    d => d.Find(dKey, Some, None),
                    None),
                None),
            None);

    [Pure]
    public static TrackingHashMap<A, TrackingHashMap<B, T>> AddOrUpdate<A, B, T>(this TrackingHashMap<A, TrackingHashMap<B, T>> self, A outerKey, B innerKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, Some, None),
            () => Prelude.TrackingHashMap(Tuple(innerKey, None()))
        );

    [Pure]
    public static TrackingHashMap<A, TrackingHashMap<B, T>> AddOrUpdate<A, B, T>(this TrackingHashMap<A, TrackingHashMap<B, T>> self, A outerKey, B innerKey, T value) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, _ => value, value),
            () => Prelude.TrackingHashMap(Tuple(innerKey, value))
        );

    [Pure]
    public static TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> AddOrUpdate<A, B, C, T>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> self, A aKey, B bKey, C cKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, _ => value, value),
            () => Prelude.TrackingHashMap(Tuple(cKey, value))
        );

    [Pure]
    public static TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> AddOrUpdate<A, B, C, T>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, Some, None),
            () => Prelude.TrackingHashMap(Tuple(cKey, None()))
        );

    [Pure]
    public static TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, TrackingHashMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, TrackingHashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, _ => value, value),
            () => Prelude.TrackingHashMap(Tuple(dKey, value))
        );

    [Pure]
    public static TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, TrackingHashMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, TrackingHashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, Some, None),
            () => Prelude.TrackingHashMap(Tuple(dKey, None()))
        );

    [Pure]
    public static TrackingHashMap<A, TrackingHashMap<B, T>> Remove<A, B, T>(this TrackingHashMap<A, TrackingHashMap<B, T>> self, A outerKey, B innerKey)
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
    public static TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> Remove<A, B, C, T>(this TrackingHashMap<A, TrackingHashMap<B, TrackingHashMap<C, T>>> self, A aKey, B bKey, C cKey)
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
}
