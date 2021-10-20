using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static class MapExtensions
    {
        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> ToMap<K, V>(this IEnumerable<(K, V)> items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> ToMap<K, V>(this IEnumerable<Tuple<K, V>> items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> ToMap<K, V>(this IEnumerable<KeyValuePair<K, V>> items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K1, Map<K2, V>> ToMap<K1, K2, V>(this IEnumerable<(K1, K2, V)> items) =>
            items.Fold(Map<K1, Map<K2, V>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3));

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K1, Map<K2, V>> ToMap<K1, K2, V>(this IEnumerable<Tuple<K1, K2, V>> items) =>
            items.Fold(Map<K1, Map<K2, V>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3));

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K1, Map<K2, Map<K3, V>>> ToMap<K1, K2, K3, V>(this IEnumerable<(K1, K2, K3, V)> items) =>
            items.Fold(Map<K1, Map<K2, Map<K3, V>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4));

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K1, Map<K2, Map<K3, V>>> ToMap<K1, K2, K3, V>(this IEnumerable<Tuple<K1, K2, K3, V>> items) =>
            items.Fold(Map<K1, Map<K2, Map<K3, V>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4));

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K1, Map<K2, Map<K3, Map<K4, V>>>> ToMap<K1, K2, K3, K4, V>(this IEnumerable<(K1, K2, K3, K4, V)> items) =>
            items.Fold(Map<K1, Map<K2, Map<K3, Map<K4, V>>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K1, Map<K2, Map<K3, Map<K4, V>>>> ToMap<K1, K2, K3, K4, V>(this IEnumerable<Tuple<K1, K2, K3, K4, V>> items) =>
            items.Fold(Map<K1, Map<K2, Map<K3, Map<K4, V>>>>(), (s, x) => s.AddOrUpdate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));


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
        public static S FoldT<A, B, S, V>(this Map<A, Map<B, V>> map, S state, Func<S, V, S> folder) =>
            map.Fold(state, (s, x) => x.Fold(s, folder));

        [Pure]
        public static S FoldT<A, B, C, S, V>(this Map<A, Map<B, Map<C, V>>> map, S state, Func<S, V, S> folder) =>
            map.Fold(state, (s, x) => x.FoldT(s, folder));

        [Pure]
        public static S FoldT<A, B, C, D, S, V>(this Map<A, Map<B, Map<C, Map<D, V>>>> map, S state, Func<S, V, S> folder) =>
            map.Fold(state, (s, x) => x.FoldT(s, folder));

        [Pure]
        public static int CountT<A, B, V>(this Map<A, Map<B, V>> map) =>
            map.Fold(0, (s, x) => s+  x.Count);

        [Pure]
        public static int CountT<A, B, C, V>(this Map<A, Map<B, Map<C, V>>> map) =>
            map.Fold(0, (s, x) => s + x.CountT());

        [Pure]
        public static int CountT<A, B, C, D, V>(this Map<A, Map<B, Map<C, Map<D, V>>>> map) =>
            map.Fold(0, (s, x) => s + x.CountT());
    }
}
