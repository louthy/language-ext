using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;

public static class HashMapExtensions
{
    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    public static int Count<K, V>(this HashMap<K, V> self) =>
        self.Count;

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<K, U> Select<K, V, U>(this HashMap<K, V> self, Func<V, U> mapper) =>
        self.Map(mapper);

    /// <summary>
    /// Atomically maps the map to a new map
    /// </summary>
    /// <returns>Mapped items in a new map</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<K, U> Select<K, V, U>(this HashMap<K, V> self, Func<K, V, U> mapper) =>
        self.Map(mapper);

    /// <summary>
    /// Atomically filter out items that return false when a predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>New map with items filtered</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<K, V> Where<K, V>(this HashMap<K, V> self, Func<V, bool> pred) =>
        self.Filter(pred);

    /// <summary>
    /// Atomically filter out items that return false when a predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>New map with items filtered</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<K, V> Where<K, V>(this HashMap<K, V> self, Func<K, V, bool> pred) =>
        self.Filter(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<K, V>(this HashMap<K, V> self, Func<K, V, bool> pred)
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
    public static bool ForAll<K, V>(this HashMap<K, V> self, Func<Tuple<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<K, V>(this HashMap<K, V> self, Func<(K Key, V Value), bool> pred) =>
        self.AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if *all* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<K, V>(this HashMap<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => new KeyValuePair<K, V>(kv.Key, kv.Value)).ForAll(pred);

    /// <summary>
    /// Return true if all items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool ForAll<K, V>(this HashMap<K, V> self, Func<V, bool> pred) =>
        self.Values.ForAll(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    public static bool Exists<K, V>(this HashMap<K, V> self, Func<K, V, bool> pred)
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
    public static bool Exists<K, V>(this HashMap<K, V> self, Func<Tuple<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => Tuple(kv.Key, kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<K, V>(this HashMap<K, V> self, Func<(K Key, V Value), bool> pred) =>
        self.AsEnumerable().Map(kv => (Key: kv.Key, Value: kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<K, V>(this HashMap<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
        self.AsEnumerable().Map(kv => new KeyValuePair<K,V>(kv.Key, kv.Value)).Exists(pred);

    /// <summary>
    /// Return true if *any* items in the map return true when the predicate is applied
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if all items in the map return true when the predicate is applied</returns>
    [Pure]
    public static bool Exists<K, V>(this HashMap<K, V> self, Func<V, bool> pred) =>
        self.Values.Exists(pred);

    /// <summary>
    /// Atomically iterate through all key/value pairs in the map (in order) and execute an
    /// action on each
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Unit</returns>
    public static Unit Iter<K, V>(this HashMap<K, V> self, Action<K, V> action)
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
    public static Unit Iter<K, V>(this HashMap<K, V> self, Action<V> action)
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
    public static Unit Iter<K, V>(this HashMap<K, V> self, Action<Tuple<K, V>> action)
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
    public static Unit Iter<K, V>(this HashMap<K, V> self, Action<(K Key, V Value)> action)
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
    public static Unit Iter<K, V>(this HashMap<K, V> self, Action<KeyValuePair<K, V>> action)
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
    public static S Fold<K, V, S>(this HashMap<K, V> self, S state, Func<S, K, V, S> folder) =>
        self.AsEnumerable().Fold(state, (s,x) => folder(s, x.Key,x.Value));

    /// <summary>
    /// Atomically folds all items in the map (in order) using the folder function provided.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<K, V, S>(this HashMap<K, V> self, S state, Func<S, V, S> folder) =>
        self.Values.Fold(state, folder);

    [Pure]
    public static Map<K, U> Bind<K, T, U>(this HashMap<K, T> self, Func<T, Map<K, U>> binder) =>
        failwith<Map<K, U>>("Map<K,V> doesn't support Bind.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Map<K, U> SelectMany<K, T, U>(this HashMap<K, T> self, Func<T, Map<K, U>> binder) =>
        failwith<Map<K, U>>("Map<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Map<K, V> SelectMany<K, T, U, V>(this HashMap<K, T> self, Func<T, Map<K, U>> binder, Func<T, U, V> project) =>
        failwith<Map<K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    public static int Sum<K>(this HashMap<K, int> self) =>
        self.Values.Sum();

    [Pure]
    public static Option<T> Find<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey) =>
        self.Find(outerKey, b => b.Find(innerKey), () => None);

    [Pure]
    public static Option<T> Find<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey) =>
        self.Find(aKey, b => b.Find(bKey, c => c.Find(cKey), () => None), () => None);

    [Pure]
    public static R Find<A, B, T, R>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey, Func<T, R> Some, Func<R> None) =>
        self.Find(outerKey, b => b.Find(innerKey, Some, None), None);

    [Pure]
    public static R Find<A, B, C, T, R>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey, Some, None),
                None),
            None);

    [Pure]
    public static R Find<A, B, C, D, T, R>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, R> Some, Func<R> None) =>
        self.Find(aKey,
            b => b.Find(bKey,
                c => c.Find(cKey,
                    d => d.Find(dKey, Some, None),
                    None),
                None),
            None);

    [Pure]
    public static HashMap<A, HashMap<B, T>> AddOrUpdate<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, Some, None),
            () => Prelude.HashMap(Tuple(innerKey, None()))
        );

    [Pure]
    public static HashMap<A, HashMap<B, T>> AddOrUpdate<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey, T value) =>
        self.AddOrUpdate(
            outerKey,
            b => b.AddOrUpdate(innerKey, _ => value, value),
            () => Prelude.HashMap(Tuple(innerKey, value))
        );

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> AddOrUpdate<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, _ => value, value),
            () => Prelude.HashMap(Tuple(cKey, value))
        );

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> AddOrUpdate<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            c => c.AddOrUpdate(cKey, Some, None),
            () => Prelude.HashMap(Tuple(cKey, None()))
        );

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, T value) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, _ => value, value),
            () => Prelude.HashMap(Tuple(dKey, value))
        );

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> AddOrUpdate<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, A aKey, B bKey, C cKey, D dKey, Func<T, T> Some, Func<T> None) =>
        self.AddOrUpdate(
            aKey,
            bKey,
            cKey,
            d => d.AddOrUpdate(dKey, Some, None),
            () => Prelude.HashMap(Tuple(dKey, None()))
        );

    [Pure]
    public static HashMap<A, HashMap<B, T>> Remove<A, B, T>(this HashMap<A, HashMap<B, T>> self, A outerKey, B innerKey)
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
    public static HashMap<A, HashMap<B, HashMap<C, T>>> Remove<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, A aKey, B bKey, C cKey)
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

    [Pure]
    public static HashMap<A, HashMap<B, V>> MapRemoveT<A, B, T, V>(this HashMap<A, HashMap<B, T>> self, Func<HashMap<B, T>, HashMap<B, V>> map)
    {
        return self.Map((ka, va) => map(va))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> MapRemoveT<A, B, C, T, V>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<HashMap<C, T>, HashMap<C, V>> map)
    {
        return self.Map((ka, va) => va.Map((kb, vb) => map(vb))
                                      .Filter((kb, vb) => vb.Count > 0))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> MapRemoveT<A, B, C, D, T, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<HashMap<D, T>, HashMap<D, V>> map)
    {
        return self.Map((ka, va) => va.Map((kb, vb) => vb.Map((kc, vc) => map(vc))
                                                         .Filter((kc, vc) => vc.Count > 0))
                                      .Filter((kb, vb) => vb.Count > 0))
                   .Filter((ka, va) => va.Count > 0);
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> MapT<A, B, T, V>(this HashMap<A, HashMap<B, T>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.Map(map));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> MapT<A, B, C, T, V>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.MapT(map));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> MapT<A, B, C, D, T, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, V> map)
    {
        return self.Map((ka, va) => va.MapT(map));
    }

    [Pure]
    public static HashMap<A, HashMap<B, T>> FilterT<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.Filter(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> FilterT<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.FilterT(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> FilterT<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.Map((ka, va) => va.FilterT(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, T>> FilterRemoveT<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, T>>> FilterRemoveT<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> FilterRemoveT<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.MapRemoveT(v => v.Filter(pred));
    }

    [Pure]
    public static bool Exists<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool Exists<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool Exists<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.Exists((k, v) => v.Exists(pred));
    }

    [Pure]
    public static bool ForAll<A, B, T>(this HashMap<A, HashMap<B, T>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static bool ForAll<A, B, C, T>(this HashMap<A, HashMap<B, HashMap<C, T>>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static bool ForAll<A, B, C, D, T>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, T>>>> self, Func<T, bool> pred)
    {
        return self.ForAll((k, v) => v.ForAll(pred));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> SetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;
        return map.SetItem(aKey, av.SetItem(bKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> SetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> SetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> SetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;
        return map.SetItem(aKey, av.SetItem(bKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> SetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> SetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) throw new ArgumentException("Key not found in Map");
        var av = a.Value;

        return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> TrySetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;
        return map.SetItem(aKey, av.TrySetItem(bKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> TrySetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> TrySetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, value));
    }

    [Pure]
    public static HashMap<A, HashMap<B, V>> TrySetItemT<A, B, V>(this HashMap<A, HashMap<B, V>> map, A aKey, B bKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;
        return map.SetItem(aKey, av.TrySetItem(bKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, V>>> TrySetItemT<A, B, C, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, Some));
    }

    [Pure]
    public static HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> TrySetItemT<A, B, C, D, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
    {
        var a = map.Find(aKey);
        if (a.IsNone) return map;
        var av = a.Value;

        return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, Some));
    }

    [Pure]
    public static S FoldT<A, B, S, V>(this HashMap<A, HashMap<B, V>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.Fold(s, folder));
    }

    [Pure]
    public static S FoldT<A, B, C, S, V>(this HashMap<A, HashMap<B, HashMap<C, V>>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.FoldT(s, folder));
    }

    [Pure]
    public static S FoldT<A, B, C, D, S, V>(this HashMap<A, HashMap<B, HashMap<C, HashMap<D, V>>>> map, S state, Func<S, V, S> folder)
    {
        return map.Fold(state, (s, x) => x.FoldT(s, folder));
    }

    [Pure]
    public static HashMap<K, U> Bind<K, T, U>(this HashMap<K, T> self, Func<T, HashMap<K, U>> binder) =>
        failwith<HashMap<K, U>>("HMap<K,V> doesn't support Bind.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<K, U> SelectMany<K, T, U>(this HashMap<K, T> self, Func<T, HashMap<K, U>> binder) =>
        failwith<HashMap<K, U>>("HMap<K,V> doesn't support Bind or SelectMany.");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashMap<K, V> SelectMany<K, T, U, V>(this HashMap<K, T> self, Func<T, HashMap<K, U>> binder, Func<T, U, V> project) =>
        failwith<HashMap<K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

}