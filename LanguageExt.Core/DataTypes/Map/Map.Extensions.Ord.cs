using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class MapOrdExtensions
    {
        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<OrdK, K, U> Map<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<V, U> mapper)
            where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<OrdK, K, U> Map<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<K, V, U> mapper) where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public static int Count<OrdK, K, V>(this Map<OrdK, K, V> self) where OrdK : struct, Ord<K> =>
            self.Count;

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<OrdK, K, U> Select<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<V, U> mapper) where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<OrdK, K, U> Select<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<K, V, U> mapper) where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<OrdK, K, V> Where<OrdK, K, V>(this Map<OrdK, K, V> self, Func<V, bool> pred) where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, V>(MapModule.Filter(self.Value.Root, pred), self.Value.Rev);

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<OrdK, K, V> Where<OrdK, K, V>(this Map<OrdK, K, V> self, Func<K, V, bool> pred) where OrdK : struct, Ord<K> =>
            self.SetRoot(MapModule.Filter(self.Value.Root, pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<OrdK, K, V> Filter<OrdK, K, V>(this Map<OrdK, K, V> self, Func<V, bool> pred) where OrdK : struct, Ord<K> =>
            self.SetRoot(MapModule.Filter(self.Value.Root, pred));

        /// <summary>
        /// Atomically filter out items that return false when a predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>New map with items filtered</returns>
        [Pure]
        public static Map<OrdK, K, V> Filter<OrdK, K, V>(this Map<OrdK, K, V> self, Func<K, V, bool> pred) where OrdK : struct, Ord<K> =>
            self.SetRoot(MapModule.Filter(self.Value.Root, pred));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<OrdK, K, V>(this Map<OrdK, K, V> self, Func<K, V, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.ForAll(self.Value.Root, pred);

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<OrdK, K, V>(this Map<OrdK, K, V> self, Func<Tuple<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.ForAll(self.Value.Root, (k, v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<OrdK, K, V>(this Map<OrdK, K, V> self, Func<(K Key, V Value), bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.ForAll(self.Value.Root, (k, v) => pred((k, v)));

        /// <summary>
        /// Return true if *all* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<OrdK, K, V>(this Map<OrdK, K, V> self, Func<KeyValuePair<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.ForAll(self.Value.Root, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Return true if all items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool ForAll<OrdK, K, V>(this Map<OrdK, K, V> self, Func<V, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.ForAll(self.Value.Root, (k, v) => pred(v));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<OrdK, K, V>(this Map<OrdK, K, V> self, Func<K, V, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.Exists(self.Value.Root, pred);

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<OrdK, K, V>(this Map<OrdK, K, V> self, Func<Tuple<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.Exists(self.Value.Root, (k, v) => pred(new Tuple<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<OrdK, K, V>(this Map<OrdK, K, V> self, Func<(K, V), bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.Exists(self.Value.Root, (k, v) => pred((k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<OrdK, K, V>(this Map<OrdK, K, V> self, Func<KeyValuePair<K, V>, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.Exists(self.Value.Root, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        /// <summary>
        /// Return true if *any* items in the map return true when the predicate is applied
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the map return true when the predicate is applied</returns>
        [Pure]
        public static bool Exists<OrdK, K, V>(this Map<OrdK, K, V> self, Func<V, bool> pred) where OrdK : struct, Ord<K> =>
            MapModule.Exists(self.Value.Root, (_, v) => pred(v));

        /// <summary>
        /// Atomically iterate through all key/value pairs in the map (in order) and execute an
        /// action on each
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Unit</returns>
        public static Unit Iter<OrdK, K, V>(this Map<OrdK, K, V> self, Action<K, V> action) where OrdK : struct, Ord<K>
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
        public static Unit Iter<OrdK, K, V>(this Map<OrdK, K, V> self, Action<V> action) where OrdK : struct, Ord<K>
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
        public static Unit Iter<OrdK, K, V>(this Map<OrdK, K, V> self, Action<Tuple<K, V>> action) where OrdK : struct, Ord<K>
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
        public static Unit Iter<OrdK, K, V>(this Map<OrdK, K, V> self, Action<(K, V)> action) where OrdK : struct, Ord<K>
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
        public static Unit Iter<OrdK, K, V>(this Map<OrdK, K, V> self, Action<KeyValuePair<K, V>> action) where OrdK : struct, Ord<K>
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
        public static Map<OrdK, K, U> Choose<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<K, V, Option<U>> selector) where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Choose(self.Value.Root, selector), self.Value.Rev);

        /// <summary>
        /// Equivalent to map and filter but the filtering is done based on whether the returned
        /// Option is Some or None.  If the item is None then it's filtered out, if not the the
        /// mapped Some value is used.
        /// </summary>
        /// <param name="selector">Predicate</param>
        /// <returns>Filtered map</returns>
        [Pure]
        public static Map<OrdK, K, U> Choose<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<V, Option<U>> selector) where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Choose(self.Value.Root, selector), self.Value.Rev);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S Fold<OrdK, K, V, S>(this Map<OrdK, K, V> self, S state, Func<S, K, V, S> folder) where OrdK : struct, Ord<K> =>
            MapModule.Fold(self.Value.Root, state, folder);

        /// <summary>
        /// Atomically folds all items in the map (in order) using the folder function provided.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S Fold<OrdK, K, V, S>(this Map<OrdK, K, V> self, S state, Func<S, V, S> folder) where OrdK : struct, Ord<K> =>
            MapModule.Fold(self.Value.Root, state, folder);

        [Pure]
        public static Map<OrdK, K, U> Bind<OrdK, K, T, U>(this Map<OrdK, K, T> self, Func<T, Map<OrdK, K, U>> binder) where OrdK : struct, Ord<K> =>
            failwith<Map<OrdK, K, U>>("Map<K,V> doesn't support Bind.");

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<OrdK, K, U> SelectMany<OrdK, K, T, U>(this Map<OrdK, K, T> self, Func<T, Map<OrdK, K, U>> binder) where OrdK : struct, Ord<K> =>
            failwith<Map<OrdK, K, U>>("Map<K,V> doesn't support Bind or SelectMany.");

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<OrdK, K, V> SelectMany<OrdK, K, T, U, V>(this Map<OrdK, K, T> self, Func<T, Map<OrdK, K, U>> binder, Func<T, U, V> project) where OrdK : struct, Ord<K> =>
            failwith<Map<OrdK, K, V>>("Map<K,V> doesn't support Bind or SelectMany.");

        [Pure]
        public static int Sum<OrdK, K>(this Map<OrdK, K, int> self) where OrdK : struct, Ord<K> =>
            self.Values.Sum();
   }
}