#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class MapExtensions
{
    public static Map<Key, V> As<Key, V>(this K<Map<Key>, V> ma) =>
        (Map<Key, V>)ma;
    
    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<K, V> ToMap<K, V>(this IEnumerable<(K, V)> items) =>
        LanguageExt.Map.createRange(items);

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<K, V> ToMap<K, V>(this IEnumerable<Tuple<K, V>> items) =>
        LanguageExt.Map.createRange(items);

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<K, V> ToMap<K, V>(this IEnumerable<KeyValuePair<K, V>> items) =>
        LanguageExt.Map.createRange(items);

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<(K1, K2), V> ToMap<K1, K2, V>(this IEnumerable<(K1, K2, V)> items) =>
        new(items.Select(x => ((x.Item1, x.Item2), x.Item3)));

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<(K1, K2, K3), V> ToMap<K1, K2, K3, V>(this IEnumerable<(K1, K2, K3, V)> items) =>
        new(items.Select(x => ((x.Item1, x.Item2, x.Item3), x.Item4)));

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<(K1, K2, K3, K4), V> ToMap<K1, K2, K3, K4, V>(this IEnumerable<(K1, K2, K3, K4, V)> items) =>
        new(items.Select(x => ((x.Item1, x.Item2, x.Item3, x.Item4), x.Item5)));
 
    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    public static Map<K, U> Map<K, V, U>(this Map<K, V> self, Func<V, U> mapper) =>
        new (self.AsIterable().Select(kv => (kv.Key, mapper( kv.Value))));

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    public static Map<K, U> Map<K, V, U>(this Map<K, V> self, Func<K, V, U> mapper) =>
        new (self.AsIterable().Select(kv => (kv.Key, mapper(kv.Key, kv.Value))));
}
