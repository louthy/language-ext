#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt;

public static partial class HashMapExtensions
{
    [Obsolete("Use AsHashMap instead")]
    public static HashMap<K, V> ToHashMap<K, V>(this IEnumerable<(K, V)> items) =>
        new(items);

    [Obsolete("Use AsHashMap instead")]
    public static HashMap<(K1, K2), V> ToHashMap<K1, K2, V>(this IEnumerable<(K1, K2, V)> items) =>
        new (items.Select(x => ((x.Item1, x.Item2), x.Item3)));

    [Obsolete("Use AsHashMap instead")]
    public static HashMap<(K1, K2, K3), V> ToHashMap<K1, K2, K3, V>(this IEnumerable<(K1, K2, K3, V)> items) =>
        new (items.Select(x => ((x.Item1, x.Item2, x.Item3), x.Item4)));

    [Obsolete("Use AsHashMap instead")]
    public static HashMap<(K1, K2, K3, K4), V> ToHashMap<K1, K2, K3, K4, V>(this IEnumerable<(K1, K2, K3, K4, V)> items) =>
        new (items.Select(x => ((x.Item1, x.Item2, x.Item3, x.Item4), x.Item5)));
}
