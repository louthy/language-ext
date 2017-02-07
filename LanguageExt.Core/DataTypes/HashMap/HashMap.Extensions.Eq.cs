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

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, U> Select<EqK, K, V, U>(this HashMap<EqK, K, V> self, Func<V, U> mapper) where EqK : struct, Eq<K> =>
        self.Map(mapper);

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, U> Select<EqK, K, V, U>(this HashMap<EqK, K, V> self, Func<K, V, U> mapper) where EqK : struct, Eq<K> =>
        self.Map(mapper);

    /// <summary>
    /// Atomically filter out items that return false when a predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>New map with items filtered</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, V> Where<EqK, K, V>(this HashMap<EqK, K, V> self, Func<V, bool> pred) where EqK : struct, Eq<K> =>
        self.Filter(pred);

    /// <summary>
    /// Atomically filter out items that return false when a predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>New map with items filtered</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<EqK, K, V> Where<EqK, K, V>(this HashMap<EqK, K, V> self, Func<K, V, bool> pred) where EqK : struct, Eq<K> =>
        self.Filter(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<EqK, K, V>(this HashMap<EqK, K, V> self, Func<K, V, bool> pred) where EqK : struct, Eq<K>
    {
        foreach(var item in self.AsEnumerable())
        {
            if (!pred(item.Key, item.Value)) return false;
        }
        return true;
    }

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<EqK, K, V>(this HashMap<EqK, K, V> self, Func<Tuple<K, V>, bool> pred) where EqK : struct, Eq<K> =>
        self.AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<EqK, K, V>(this HashMap<EqK, K, V> self, Func<(K Key, V Value), bool> pred) where EqK : struct, Eq<K> =>
        self.AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if *all* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<EqK, K, V>(this HashMap<EqK, K, V> self, Func<KeyValuePair<K, V>, bool> pred) where EqK : struct, Eq<K> =>
        self.AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<EqK, K, V>(this HashMap<EqK, K, V> self, Func<V, bool> pred) where EqK : struct, Eq<K> =>
        self.Values.ForAll(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    public static bool Exists<EqK, K, V>(this HashMap<EqK, K, V> self, Func<K, V, bool> pred) where EqK : struct, Eq<K>
    {
        foreach (var item in self.AsEnumerable())
        {
            if (pred(item.Key, item.Value)) return true;
        }
        return false;
    }

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<EqK, K, V>(this HashMap<EqK, K, V> self, Func<Tuple<K, V>, bool> pred) where EqK : struct, Eq<K> =>
        self.AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<EqK, K, V>(this HashMap<EqK, K, V> self, Func<(K Key, V Value), bool> pred) where EqK : struct, Eq<K> =>
        self.AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<EqK, K, V>(this HashMap<EqK, K, V> self, Func<KeyValuePair<K, V>, bool> pred) where EqK : struct, Eq<K> =>
        self.AsEnumerable().Map(kv => new KeyValuePair<K,V>(kv.Key, kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<EqK, K, V>(this HashMap<EqK, K, V> self, Func<V, bool> pred) where EqK : struct, Eq<K> =>
        self.Values.Exists(pred);

    /// <summary>
    /// Atomically iterate through all key/value pairs in the map (in order) and execute an
    /// action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<EqK, K, V>(this HashMap<EqK, K, V> self, Action<K, V> action) where EqK : struct, Eq<K>
    {
        foreach (var item in self)
        {
            action(item.Key, item.Value);
        }
        return unit;
    }

    /// <summary>
    /// Atomically iterate through all values in the map (in order) and execute an
    /// action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<EqK, K, V>(this HashMap<EqK, K, V> self, Action<V> action) where EqK : struct, Eq<K>
    {
        foreach (var item in self)
        {
            action(item.Value);
        }
        return unit;
    }

    /// <summary>
    /// Atomically iterate through all key/value pairs (as tuples) in the map (in order) 
    /// and execute an action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<EqK, K, V>(this HashMap<EqK, K, V> self, Action<Tuple<K, V>> action) where EqK : struct, Eq<K>
    {
        foreach (var item in self)
        {
            action(new Tuple<K, V>(item.Key, item.Value));
        }
        return unit;
    }

    /// <summary>
    /// Atomically iterate through all key/value pairs (as tuples) in the map (in order) 
    /// and execute an action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<EqK, K, V>(this HashMap<EqK, K, V> self, Action<(K Key, V Value)> action) where EqK : struct, Eq<K>
    {
        foreach (var item in self)
        {
            action((item.Key, item.Value));
        }
        return unit;
    }

    /// <summary>
    /// Atomically iterate through all key/value pairs in the map (in order) and execute an
    /// action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<EqK, K, V>(this HashMap<EqK, K, V> self, Action<KeyValuePair<K, V>> action) where EqK : struct, Eq<K>
    {
        foreach (var item in self)
        {
            action(new KeyValuePair<K, V>(item.Key, item.Value));
        }
        return unit;
    }

    /// <summary>
    /// Atomically folds all items in the map (in order) using the folder function provided.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<EqK, K, V, S>(this HashMap<EqK, K, V> self, S state, Func<S, K, V, S> folder) where EqK : struct, Eq<K> =>
        self.AsEnumerable().Fold(state, (s,x) => folder(s, x.Key,x.Value));

    /// <summary>
    /// Atomically folds all items in the map (in order) using the folder function provided.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<EqK, K, V, S>(this HashMap<EqK, K, V> self, S state, Func<S, V, S> folder) where EqK : struct, Eq<K> =>
        self.Values.Fold(state, folder);

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