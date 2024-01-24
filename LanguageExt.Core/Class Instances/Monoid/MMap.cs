using System;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct MMap<K, V> :
    Foldable<Map<K, V>, V>,
    Monoid<Map<K, V>>,
    Eq<Map<K, V>>
{
    [Pure]
    public static Map<K, V> Append(Map<K, V> x, Map<K, V> y) =>
        x + y;

    [Pure]
    public static Func<Unit, int> Count(Map<K, V> fa) => _ =>
        fa.Count;

    [Pure]
    public static Map<K, V> Subtract(Map<K, V> x, Map<K, V> y) =>
        x - y;

    [Pure]
    public static Map<K, V> Empty() =>
        Map<K, V>.Empty;

    [Pure]
    public static bool Equals(Map<K, V> x, Map<K, V> y) =>
        x == y;

    [Pure]
    public static Func<Unit, S> Fold<S>(Map<K, V> fa, S state, Func<S, V, S> f) => _ =>
        fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Map<K, V> fa, S state, Func<S, V, S> f) => _ =>
        fa.Values.Reverse().FoldBack(state, f);

    [Pure]
    public static int GetHashCode(Map<K, V> x) =>
        x.GetHashCode();
}
