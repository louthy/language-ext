#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static class MapOrdExtensions
{
    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    public static Map<OrdK, K, U> Map<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<V, U> mapper)
        where OrdK : Ord<K> =>
        new (self.AsEnumerable().Select(kv => (kv.Key, mapper(kv.Value))));

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    public static Map<OrdK, K, U> Map<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<K, V, U> mapper) where OrdK : Ord<K> =>
        new (self.AsEnumerable().Select(kv => (kv.Key, mapper(kv.Key, kv.Value))));

    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    public static int Count<OrdK, K, V>(this Map<OrdK, K, V> self) where OrdK : Ord<K> =>
        self.Count;

    [Pure]
    public static int Sum<OrdK, K>(this Map<OrdK, K, int> self) where OrdK : Ord<K> =>
        self.Values.Sum();
}
