#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class HashMapExtensions
{
    public static HashMap<EqKey, Key, V> As<EqKey, Key, V>(this K<HashMapEq<EqKey, Key>, V> ma)
        where EqKey : Eq<Key> =>
        (HashMap<EqKey, Key, V>)ma;
    
    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<EqK, K, V> ToHashMap<EqK, K, V>(this IEnumerable<(K, V)> items)
        where EqK : Eq<K> =>
        new(items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<EqK, (K1, K2), V> ToHashMap<EqK, K1, K2, V>(this IEnumerable<(K1, K2, V)> items)
        where EqK : Eq<(K1, K2)> =>
        new (items.Select(x => ((x.Item1, x.Item2), x.Item3)));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<EqK, (K1, K2, K3), V> ToHashMap<EqK, K1, K2, K3, V>(this IEnumerable<(K1, K2, K3, V)> items)
        where EqK : Eq<(K1, K2, K3)> =>
        new (items.Select(x => ((x.Item1, x.Item2, x.Item3), x.Item4)));

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashMap<EqK, (K1, K2, K3, K4), V> ToHashMap<EqK, K1, K2, K3, K4, V>(this IEnumerable<(K1, K2, K3, K4, V)> items)
        where EqK : Eq<(K1, K2, K3, K4)> =>
        new (items.Select(x => ((x.Item1, x.Item2, x.Item3, x.Item4), x.Item5)));
        
    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public static IQueryable<(K Key, V Value)> AsQueryable<EqK, K, V>(this HashMap<EqK, K, V> source)
        where EqK : Eq<K> =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        Queryable.AsQueryable(source.Value.AsQueryable());

}
