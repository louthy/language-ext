#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class HashMapExtensions
{
    public static HashMap<Key, V> As<Key, V>(this K<HashMap<Key>, V> ma) =>
        (HashMap<Key, V>)ma;
    
    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<K, V> ToHashMap<K, V>(this IEnumerable<(K, V)> items) =>
        new(items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<(K1, K2), V> ToHashMap<K1, K2, V>(this IEnumerable<(K1, K2, V)> items) =>
        new (items.Select(x => ((x.Item1, x.Item2), x.Item3)));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<(K1, K2, K3), V> ToHashMap<K1, K2, K3, V>(this IEnumerable<(K1, K2, K3, V)> items) =>
        new (items.Select(x => ((x.Item1, x.Item2, x.Item3), x.Item4)));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<(K1, K2, K3, K4), V> ToHashMap<K1, K2, K3, K4, V>(this IEnumerable<(K1, K2, K3, K4, V)> items) =>
        new (items.Select(x => ((x.Item1, x.Item2, x.Item3, x.Item4), x.Item5)));
        
    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public static IQueryable<(K Key, V Value)> AsQueryable<K, V>(this HashMap<K, V> source) =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        source.Value.AsQueryable();
}
