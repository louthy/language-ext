using System;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct MHashMap<K, V> :
    Foldable<HashMap<K, V>, V>,
    Monoid<HashMap<K, V>>,
    Eq<HashMap<K, V>>
{
    [Pure]
    public static HashMap<K, V> Append(HashMap<K, V> x, HashMap<K, V> y) =>
        x + y;

    [Pure]
    public static Func<Unit, int> Count(HashMap<K, V> fa) => _ =>
        fa.Count;

    [Pure]
    public static HashMap<K, V> Subtract(HashMap<K, V> x, HashMap<K, V> y) =>
        x - y;

    [Pure]
    public static HashMap<K, V> Empty() =>
        HashMap<K, V>.Empty;

    [Pure]
    public static bool Equals(HashMap<K, V> x, HashMap<K, V> y) =>
        x == y;

    [Pure]
    public static Func<Unit, S> Fold<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) => _ =>
        fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) => _ =>
        fa.Values.Reverse().FoldBack(state, f);

    [Pure]
    public static int GetHashCode(HashMap<K, V> x) =>
        x.GetHashCode();
}
