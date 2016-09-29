using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static class MapExtensions
    {
        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<K, U> Map<K, V, U>(this Map<K, V> self, Func<V, U> mapper) =>
            new Map<K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<K, U> Map<K, V, U>(this Map<K, V> self, Func<K, V, U> mapper) =>
            new Map<K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public static int Count<K, V>(this Map<K, V> self) =>
            self.Count;

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, U> Select<K, V, U>(this Map<K, V> self, Func<V, U> mapper) =>
            new Map<K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, U> Select<K, V, U>(this Map<K, V> self, Func<K, V, U> mapper) =>
            new Map<K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> Where<K, V>(this Map<K, V> self, Func<V, bool> pred) =>
            new Map<K, V>(MapModule.Filter(self.Value.Root, pred), self.Value.Rev);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> Where<K, V>(this Map<K, V> self, Func<K, V, bool> pred) =>
            self.SetRoot(MapModule.Filter(self.Value.Root, pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<K, V> Filter<K, V>(this Map<K, V> self, Func<V, bool> pred) =>
            self.SetRoot(MapModule.Filter(self.Value.Root, pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<K, V> Filter<K, V>(this Map<K, V> self, Func<K, V, bool> pred) =>
            self.SetRoot(MapModule.Filter(self.Value.Root, pred));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<K, V>(this Map<K, V> self, Func<K, V, bool> pred) =>
            MapModule.ForAll(self.Value.Root, pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<K, V>(this Map<K, V> self, Func<Tuple<K, V>, bool> pred) =>
            MapModule.ForAll(self.Value.Root, (k, v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<K, V>(this Map<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.ForAll(self.Value.Root, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<K, V>(this Map<K, V> self, Func<V, bool> pred) =>
            MapModule.ForAll(self.Value.Root, (k, v) => pred(v));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<K, V>(this Map<K, V> self, Func<K, V, bool> pred) =>
            MapModule.Exists(self.Value.Root, pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<K, V>(this Map<K, V> self, Func<Tuple<K, V>, bool> pred) =>
            MapModule.Exists(self.Value.Root, (k, v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<K, V>(this Map<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.Exists(self.Value.Root, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<K, V>(this Map<K, V> self, Func<V, bool> pred) =>
            MapModule.Exists(self.Value.Root, (_, v) => pred(v));

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit Iter<K, V>(this Map<K, V> self, Action<K, V> action)
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
        public static Unit Iter<K, V>(this Map<K, V> self, Action<V> action)
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
        public static Unit Iter<K, V>(this Map<K, V> self, Action<Tuple<K, V>> action)
        {
            foreach (var item in self)
            {
                action(new Tuple<K, V>(item.Key, item.Value));
            }
            return unit;
        }

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit Iter<K, V>(this Map<K, V> self, Action<KeyValuePair<K, V>> action)
        {
            foreach (var item in self)
            {
                action(new KeyValuePair<K, V>(item.Key, item.Value));
            }
            return unit;
        }

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public static Map<K, V> Choose<K, V>(this Map<K, V> self, Func<K, V, Option<V>> selector) =>
            self.SetRoot(MapModule.Choose(self.Value.Root, selector));

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public static Map<K, V> Choose<K, V>(this Map<K, V> self, Func<V, Option<V>> selector) =>
            self.SetRoot(MapModule.Choose(self.Value.Root, selector));

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S Fold<K, V, S>(this Map<K, V> self, S state, Func<S, K, V, S> folder) =>
            MapModule.Fold(self.Value.Root, state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S Fold<K, V, S>(this Map<K, V> self, S state, Func<S, V, S> folder) =>
            MapModule.Fold(self.Value.Root, state, folder);

        [Pure]
        public static Map<K, U> Bind<K, T, U>(this Map<K, T> self, Func<T, Map<K, U>> binder) =>
            failwith<Map<K, U>>("Map<K,V> doesn't support Bind.");

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, U> SelectMany<K, T, U>(this Map<K, T> self, Func<T, Map<K, U>> binder) =>
            failwith<Map<K, U>>("Map<K,V> doesn't support Bind or SelectMany.");

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> SelectMany<K, T, U, V>(this Map<K, T> self, Func<T, Map<K, U>> binder, Func<T, U, V> project) =>
            failwith<Map<K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

        [Pure]
        public static int Sum<K>(this Map<K, int> self) =>
            self.Values.Sum();

        // 
        // Map<A<Map<B,C>>
        // 

        [Pure]
        public static Option<C> Find<A, B, C>(this Map<A, Map<B, C>> self, A outerKey, B innerKey) =>
            self.Find(outerKey, b => b.Find(innerKey), () => None);

        [Pure]
        public static Option<D> Find<A, B, C, D>(this Map<A, Map<B, Map<C, D>>> self, A aKey, B bKey, C cKey) =>
            self.Find(aKey, b => b.Find(bKey, c => c.Find(cKey), () => None), () => None);

        [Pure]
        public static D Find<A, B, C, D>(this Map<A, Map<B, C>> self, A outerKey, B innerKey, Func<C, D> Some, Func<D> None) =>
            self.Find(outerKey, b => b.Find(innerKey, Some, None), None);

        [Pure]
        public static E Find<A, B, C, D, E>(this Map<A, Map<B, Map<C, D>>> self, A aKey, B bKey, C cKey, Func<D, E> Some, Func<E> None) =>
            self.Find(aKey,
                b => b.Find(bKey,
                    c => c.Find(cKey, Some, None),
                    None),
                None);

        [Pure]
        public static F Find<A, B, C, D, E, F>(this Map<A, Map<B, Map<C, Map<D, E>>>> self, A aKey, B bKey, C cKey, D dKey, Func<E, F> Some, Func<F> None) =>
            self.Find(aKey,
                b => b.Find(bKey,
                    c => c.Find(cKey,
                        d => d.Find(dKey, Some, None),
                        None),
                    None),
                None);

        [Pure]
        public static Map<A, Map<B, C>> AddOrUpdate<A, B, C>(this Map<A, Map<B, C>> self, A outerKey, B innerKey, Func<C, C> Some, Func<C> None) =>
            self.AddOrUpdate(
                outerKey,
                b => b.AddOrUpdate(innerKey, Some, None),
                () => Prelude.Map(Tuple(innerKey, None()))
            );

        [Pure]
        public static Map<A, Map<B, C>> AddOrUpdate<A, B, C>(this Map<A, Map<B, C>> self, A outerKey, B innerKey, C value) =>
            self.AddOrUpdate(
                outerKey,
                b => b.AddOrUpdate(innerKey, _ => value, value),
                () => Prelude.Map(Tuple(innerKey, value))
            );

        [Pure]
        public static Map<A, Map<B, Map<C, D>>> AddOrUpdate<A, B, C, D>(this Map<A, Map<B, Map<C, D>>> self, A aKey, B bKey, C cKey, D value) =>
            self.AddOrUpdate(
                aKey,
                bKey,
                c => c.AddOrUpdate(cKey, _ => value, value),
                () => Prelude.Map(Tuple(cKey, value))
            );

        [Pure]
        public static Map<A, Map<B, Map<C, D>>> AddOrUpdate<A, B, C, D>(this Map<A, Map<B, Map<C, D>>> self, A aKey, B bKey, C cKey, Func<D, D> Some, Func<D> None) =>
            self.AddOrUpdate(
                aKey,
                bKey,
                c => c.AddOrUpdate(cKey, Some, None),
                () => Prelude.Map(Tuple(cKey, None()))
            );

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, E>>>> AddOrUpdate<A, B, C, D, E>(this Map<A, Map<B, Map<C, Map<D, E>>>> self, A aKey, B bKey, C cKey, D dKey, E value) =>
            self.AddOrUpdate(
                aKey,
                bKey,
                cKey,
                d => d.AddOrUpdate(dKey, _ => value, value),
                () => Prelude.Map(Tuple(dKey, value))
            );

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, E>>>> AddOrUpdate<A, B, C, D, E>(this Map<A, Map<B, Map<C, Map<D, E>>>> self, A aKey, B bKey, C cKey, D dKey, Func<E, E> Some, Func<E> None) =>
            self.AddOrUpdate(
                aKey,
                bKey,
                cKey,
                d => d.AddOrUpdate(dKey, Some, None),
                () => Prelude.Map(Tuple(dKey, None()))
            );

        [Pure]
        public static Map<A, Map<B, C>> Remove<A, B, C>(this Map<A, Map<B, C>> self, A outerKey, B innerKey)
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
        public static Map<A, Map<B, Map<C, D>>> Remove<A, B, C, D>(this Map<A, Map<B, Map<C, D>>> self, A aKey, B bKey, C cKey)
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
        public static Map<A, Map<B, Map<C, Map<D, E>>>> Remove<A, B, C, D, E>(this Map<A, Map<B, Map<C, Map<D, E>>>> self, A aKey, B bKey, C cKey, D dKey)
        {
            var res = self.Find(aKey, bKey, cKey);

            if (res.IsSome && res.CountT() > 1)
            {
                return self.SetItemT(aKey, bKey, cKey, res.IfNoneUnsafe(null).Remove(dKey));
            }
            else
            {
                if (res.IsSome)
                {
                    if (res.Map(d => d.ContainsKey(dKey)).IfNone(false))
                    {
                        return Remove(self, aKey, bKey, cKey);
                    }
                    else
                    {
                        return self;
                    }
                }
                else
                {
                    return Remove(self, aKey, bKey, cKey);
                }
            }
        }

        [Pure]
        public static Map<A, Map<B, V>> MapRemoveT<A, B, T, V>(this Map<A, Map<B, T>> self, Func<Map<B, T>, Map<B, V>> map)
        {
            return self.Map((ka, va) => map(va))
                       .Filter((ka, va) => va.Count > 0);
        }

        [Pure]
        public static Map<A, Map<B, Map<C, V>>> MapRemoveT<A, B, C, T, V>(this Map<A, Map<B, Map<C, T>>> self, Func<Map<C, T>, Map<C, V>> map)
        {
            return self.Map((ka, va) => va.Map((kb, vb) => map(vb))
                                          .Filter((kb, vb) => vb.Count > 0))
                       .Filter((ka, va) => va.Count > 0);
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, V>>>> MapRemoveT<A, B, C, D, T, V>(this Map<A, Map<B, Map<C, Map<D, T>>>> self, Func<Map<D, T>, Map<D, V>> map)
        {
            return self.Map((ka, va) => va.Map((kb, vb) => vb.Map((kc, vc) => map(vc))
                                                             .Filter((kc, vc) => vc.Count > 0))
                                          .Filter((kb, vb) => vb.Count > 0))
                       .Filter((ka, va) => va.Count > 0);
        }

        [Pure]
        public static Map<A, Map<B, V>> MapT<A, B, T, V>(this Map<A, Map<B, T>> self, Func<T, V> map)
        {
            return self.Map((ka, va) => va.Map(map));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, V>>> MapT<A, B, C, T, V>(this Map<A, Map<B, Map<C, T>>> self, Func<T, V> map)
        {
            return self.Map((ka, va) => va.MapT(map));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, V>>>> MapT<A, B, C, D, T, V>(this Map<A, Map<B, Map<C, Map<D, T>>>> self, Func<T, V> map)
        {
            return self.Map((ka, va) => va.MapT(map));
        }

        [Pure]
        public static Map<A, Map<B, T>> FilterT<A, B, T>(this Map<A, Map<B, T>> self, Func<T, bool> pred)
        {
            return self.Map((ka, va) => va.Filter(pred));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, T>>> FilterT<A, B, C, T>(this Map<A, Map<B, Map<C, T>>> self, Func<T, bool> pred)
        {
            return self.Map((ka, va) => va.FilterT(pred));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, T>>>> FilterT<A, B, C, D, T>(this Map<A, Map<B, Map<C, Map<D, T>>>> self, Func<T, bool> pred)
        {
            return self.Map((ka, va) => va.FilterT(pred));
        }

        [Pure]
        public static Map<A, Map<B, T>> FilterRemoveT<A, B, T>(this Map<A, Map<B, T>> self, Func<T, bool> pred)
        {
            return self.MapRemoveT(v => v.Filter(pred));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, T>>> FilterRemoveT<A, B, C, T>(this Map<A, Map<B, Map<C, T>>> self, Func<T, bool> pred)
        {
            return self.MapRemoveT(v => v.Filter(pred));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, T>>>> FilterRemoveT<A, B, C, D, T>(this Map<A, Map<B, Map<C, Map<D, T>>>> self, Func<T, bool> pred)
        {
            return self.MapRemoveT(v => v.Filter(pred));
        }

        [Pure]
        public static bool Exists<A, B, T>(this Map<A, Map<B, T>> self, Func<T, bool> pred)
        {
            return self.Exists((k, v) => v.Exists(pred));
        }

        [Pure]
        public static bool Exists<A, B, C, T>(this Map<A, Map<B, Map<C, T>>> self, Func<T, bool> pred)
        {
            return self.Exists((k, v) => v.Exists(pred));
        }

        [Pure]
        public static bool Exists<A, B, C, D, T>(this Map<A, Map<B, Map<C, Map<D, T>>>> self, Func<T, bool> pred)
        {
            return self.Exists((k, v) => v.Exists(pred));
        }

        [Pure]
        public static bool ForAll<A, B, T>(this Map<A, Map<B, T>> self, Func<T, bool> pred)
        {
            return self.ForAll((k, v) => v.ForAll(pred));
        }

        [Pure]
        public static bool ForAll<A, B, C, T>(this Map<A, Map<B, Map<C, T>>> self, Func<T, bool> pred)
        {
            return self.ForAll((k, v) => v.ForAll(pred));
        }

        [Pure]
        public static bool ForAll<A, B, C, D, T>(this Map<A, Map<B, Map<C, Map<D, T>>>> self, Func<T, bool> pred)
        {
            return self.ForAll((k, v) => v.ForAll(pred));
        }

        [Pure]
        public static Map<A, Map<B, V>> SetItemT<A, B, V>(this Map<A, Map<B, V>> map, A aKey, B bKey, V value)
        {
            var a = map.Find(aKey);
            if (a.IsNone) throw new ArgumentException("Key not found in Map");
            var av = a.Value;
            return map.SetItem(aKey, av.SetItem(bKey, value));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, V>>> SetItemT<A, B, C, V>(this Map<A, Map<B, Map<C, V>>> map, A aKey, B bKey, C cKey, V value)
        {
            var a = map.Find(aKey);
            if (a.IsNone) throw new ArgumentException("Key not found in Map");
            var av = a.Value;

            return map.SetItem(aKey, av.SetItemT(bKey, cKey, value));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, V>>>> SetItemT<A, B, C, D, V>(this Map<A, Map<B, Map<C, Map<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
        {
            var a = map.Find(aKey);
            if (a.IsNone) throw new ArgumentException("Key not found in Map");
            var av = a.Value;

            return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, value));
        }

        [Pure]
        public static Map<A, Map<B, V>> SetItemT<A, B, V>(this Map<A, Map<B, V>> map, A aKey, B bKey, Func<V, V> Some)
        {
            var a = map.Find(aKey);
            if (a.IsNone) throw new ArgumentException("Key not found in Map");
            var av = a.Value;
            return map.SetItem(aKey, av.SetItem(bKey, Some));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, V>>> SetItemT<A, B, C, V>(this Map<A, Map<B, Map<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
        {
            var a = map.Find(aKey);
            if (a.IsNone) throw new ArgumentException("Key not found in Map");
            var av = a.Value;

            return map.SetItem(aKey, av.SetItemT(bKey, cKey, Some));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, V>>>> SetItemT<A, B, C, D, V>(this Map<A, Map<B, Map<C, Map<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
        {
            var a = map.Find(aKey);
            if (a.IsNone) throw new ArgumentException("Key not found in Map");
            var av = a.Value;

            return map.SetItem(aKey, av.SetItemT(bKey, cKey, dKey, Some));
        }

        [Pure]
        public static Map<A, Map<B, V>> TrySetItemT<A, B, V>(this Map<A, Map<B, V>> map, A aKey, B bKey, V value)
        {
            var a = map.Find(aKey);
            if (a.IsNone) return map;
            var av = a.Value;
            return map.SetItem(aKey, av.TrySetItem(bKey, value));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, V>>> TrySetItemT<A, B, C, V>(this Map<A, Map<B, Map<C, V>>> map, A aKey, B bKey, C cKey, V value)
        {
            var a = map.Find(aKey);
            if (a.IsNone) return map;
            var av = a.Value;

            return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, value));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, V>>>> TrySetItemT<A, B, C, D, V>(this Map<A, Map<B, Map<C, Map<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, V value)
        {
            var a = map.Find(aKey);
            if (a.IsNone) return map;
            var av = a.Value;

            return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, value));
        }

        [Pure]
        public static Map<A, Map<B, V>> TrySetItemT<A, B, V>(this Map<A, Map<B, V>> map, A aKey, B bKey, Func<V, V> Some)
        {
            var a = map.Find(aKey);
            if (a.IsNone) return map;
            var av = a.Value;
            return map.SetItem(aKey, av.TrySetItem(bKey, Some));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, V>>> TrySetItemT<A, B, C, V>(this Map<A, Map<B, Map<C, V>>> map, A aKey, B bKey, C cKey, Func<V, V> Some)
        {
            var a = map.Find(aKey);
            if (a.IsNone) return map;
            var av = a.Value;

            return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, Some));
        }

        [Pure]
        public static Map<A, Map<B, Map<C, Map<D, V>>>> TrySetItemT<A, B, C, D, V>(this Map<A, Map<B, Map<C, Map<D, V>>>> map, A aKey, B bKey, C cKey, D dKey, Func<V, V> Some)
        {
            var a = map.Find(aKey);
            if (a.IsNone) return map;
            var av = a.Value;

            return map.SetItem(aKey, av.TrySetItemT(bKey, cKey, dKey, Some));
        }

        [Pure]
        public static S FoldT<A, B, S, V>(this Map<A, Map<B, V>> map, S state, Func<S, V, S> folder)
        {
            return map.Fold(state, (s, x) => x.Fold(s, folder));
        }

        [Pure]
        public static S FoldT<A, B, C, S, V>(this Map<A, Map<B, Map<C, V>>> map, S state, Func<S, V, S> folder)
        {
            return map.Fold(state, (s, x) => x.FoldT(s, folder));
        }

        [Pure]
        public static S FoldT<A, B, C, D, S, V>(this Map<A, Map<B, Map<C, Map<D, V>>>> map, S state, Func<S, V, S> folder)
        {
            return map.Fold(state, (s, x) => x.FoldT(s, folder));
        }
    }
}