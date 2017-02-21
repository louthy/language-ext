using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

public static class HashMapEqExtensions
{
    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    public static int Count<EqK, K, V>(this HashMap<EqK, K, V> self) where EqK : struct, Eq<K> =>
        self.Count;

    [Pure]
    public static HashMap<EqK, K, U> Bind<EqK, K, T, U>(this HashMap<EqK, K, T> self, Func<T, Map<K, U>> binder) where EqK : struct, Eq<K> =>
        failwith<HashMap<EqK, K, U>>("Map<EqK, K,V> doesn't support Bind.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, U> SelectMany<EqK, K, T, U>(this HashMap<EqK, K, T> self, Func<T, Map<K, U>> binder) where EqK : struct, Eq<K> =>
        failwith<HashMap<EqK, K, U>>("Map<EqK, K,V> doesn't support Bind or SelectMany.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, V> SelectMany<EqK, K, T, U, V>(this HashMap<K, T> self, Func<T, Map<K, U>> binder, Func<T, U, V> project) where EqK : struct, Eq<K> =>
        failwith<HashMap<EqK, K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    public static int Sum<EqK, K>(this HashMap<EqK, K, int> self) where EqK : struct, Eq<K> =>
        self.Values.Sum();

    [Pure]
    public static HashMap<EqK, K, U> Bind<EqK, K, T, U>(this HashMap<EqK, K, T> self, Func<T, HashMap<EqK, K, U>> binder) where EqK : struct, Eq<K> =>
        failwith<HashMap<EqK, K, U>>("HMap<K,V> doesn't support Bind.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, U> SelectMany<EqK, K, T, U>(this HashMap<EqK, K, T> self, Func<T, HashMap<EqK, K, U>> binder) where EqK : struct, Eq<K> =>
        failwith<HashMap<EqK, K, U>>("HMap<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, V> SelectMany<EqK, K, T, U, V>(this HashMap<EqK, K, T> self, Func<T, HashMap<EqK, K, U>> binder, Func<T, U, V> project) where EqK : struct, Eq<K> =>
        failwith<HashMap<EqK, K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

}