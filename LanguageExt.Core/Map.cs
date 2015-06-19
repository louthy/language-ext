using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Map
    {
        public static Map<K, V> create<K, V>() where K : IComparable<K> =>
            new Empty<K, V>();

        public static Map<K, V> add<K, V>(Map<K, V> map, K key, V value) where K : IComparable<K> =>
            map.Add(key, value);

        public static Map<K, V> add<K, V>(Map<K, V> map, params Tuple<K, V>[] keyValues) where K : IComparable<K> =>
            keyValues.Count() == 0
                ? map
                : keyValues.Count() == 1
                    ? map.Add(keyValues[0].Item1, keyValues[0].Item2)
                    : addRange(map, keyValues);

        public static Map<K, V> addRange<K, V>(Map<K, V> map, IEnumerable<Tuple<K, V>> keyValues) where K : IComparable<K> =>
            map.AddRange(keyValues);

        public static Map<K, V> addRange<K, V>(Map<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) where K : IComparable<K> =>
            addRange(map, keyValues.Select(t => tuple(t.Key,t.Value)));

        public static Map<K, V> remove<K, V>(Map<K, V> map, K key) where K : IComparable<K> =>
            map.Remove(key);

        public static bool containsKey<K, V>(Map<K, V> map, K key) where K : IComparable<K> =>
            map.ContainsKey(key);

        public static bool contains<K, V>(Map<K, V> map, KeyValuePair<K, V> kv) where K : IComparable<K> =>
            map.Contains(tuple(kv.Key,kv.Value));

        public static bool contains<K, V>(Map<K, V> map, Tuple<K, V> kv) where K : IComparable<K> =>
            map.Contains(kv);

        public static Map<K, V> setItem<K, V>(Map<K, V> map, K key, V value) where K : IComparable<K> =>
            map.SetItem(key, value);

        public static Option<V> find<K, V>(Map<K, V> map, K key) where K : IComparable<K> =>
            map.Find(key);

        public static Unit iter<K, V>(Map<K, V> map, Action<V> action) where K : IComparable<K> =>
            map.Iter(action);

        public static Unit iter<K, V>(Map<K, V> map, Action<K, V> action) where K : IComparable<K> =>
            map.Iter(action);

        public static bool forall<K, V>(Map<K, V> map, Func<K, V, bool> pred) where K : IComparable<K> =>
            map.ForAll(kv => pred(kv.Item1, kv.Item2));

        public static bool forall<K, V>(Map<K, V> map, Func<Tuple<K, V>, bool> pred) where K : IComparable<K> =>
            map.ForAll(kv => pred(tuple(kv.Item1, kv.Item2)));

        public static bool forall<K, V>(Map<K, V> map, Func<KeyValuePair<K, V>, bool> pred) where K : IComparable<K> =>
            map.ForAll(kv => pred(new KeyValuePair<K, V>(kv.Item1,kv.Item2)));

        public static Map<K, U> map<K, T, U>(Map<K, T> map, Func<T, U> f) where K : IComparable<K> =>
            map.Select(f);

        public static Map<K, U> map<K, T, U>(Map<K, T> map, Func<K, T, U> f) where K : IComparable<K> =>
            map.Select(f);

        public static Map<K, T> filter<K, T>(Map<K, T> map, Func<T, bool> predicate) where K : IComparable<K> =>
            map.Filter(predicate);

        public static Map<K, T> choose<K, T>(Map<K, T> map, Func<T, Option<T>> selector) where K : IComparable<K> =>
            map.Choose(selector);

        public static Map<K, T> choose<K, T>(Map<K, T> map, Func<K, T, Option<T>> selector) where K : IComparable<K> =>
            map.Choose(selector);

        public static int length<K, T>(Map<K, T> map) where K : IComparable<K> =>
            map.Count;

        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, K, V, S> folder) where K : IComparable<K> =>
            map.Fold(state, folder);

        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, K, S> folder) where K : IComparable<K> =>
            map.Fold(state, folder);

        public static S fold<S, K, V>(Map<K, V> map, S state, Func<S, V, S> folder) where K : IComparable<K> =>
            map.Fold(state, folder);

        public static bool exists<K, V>(Map<K, V> map, Func<K, V, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);

        public static bool exists<K, V>(Map<K, V> map, Func<Tuple<K, V>, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);

        public static bool exists<K, V>(Map<K, V> map, Func<KeyValuePair<K, V>, bool> pred) where K : IComparable<K> =>
            map.Exists(pred);
    }
}

