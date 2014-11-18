using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Prelude;
using LanguageExt.List;

namespace LanguageExt
{
    public static partial class Map
    {
        public static IImmutableDictionary<K, V> add<K, V>(this IImmutableDictionary<K, V> self, K key, V value) =>
            self.Add(key, value);

        public static IImmutableDictionary<K, V> add<K, V>(this IImmutableDictionary<K, V> self, Tuple<K,V> kv) =>
            self.Add(kv.Item1, kv.Item2);

        public static Unit remove<K, V>(this IImmutableDictionary<K, V> self, K key, V value) =>
            ignore( () => self.Remove(key) );

        public static bool contains<K, V>(this IImmutableDictionary<K, V> self, K key) =>
            self.ContainsKey(key);

        public static bool contains<K, V>(this IImmutableDictionary<K, V> self, KeyValuePair<K,V> kv) =>
            self.Contains(kv);

        public static bool contains<K, V>(this IImmutableDictionary<K, V> self, Tuple<K, V> kv) =>
            self.Contains(new KeyValuePair<K, V>(kv.Item1, kv.Item2));

        public static IImmutableDictionary<K, V> set<K, V>(this IImmutableDictionary<K, V> self, K key, V value) =>
            self.SetItem(key, value);

        public static Option<V> find<K, V>(this IImmutableDictionary<K, V> self, K key) =>
            contains(self, key)
                ? Some(self[key])
                : None;

        public static Unit each<K, V>(this IImmutableDictionary<K, V> self, Action<K, V> action) =>
            ignore(() => each<K>(self.Keys, key => action(key, self[key])));

        public static IImmutableDictionary<K, U> map<K, T, U>(this IImmutableDictionary<K, T> m, Func<T, U> f) =>
            m.Select(kv => new KeyValuePair<K, U>(kv.Key, f(kv.Value))).ToImmutableDictionary();

        public static IImmutableDictionary<K, U> map<K, T, U>(this IImmutableDictionary<K, T> m, Func<K, T, U> f) =>
            m.Select(kv => new KeyValuePair<K, U>(kv.Key, f(kv.Key, kv.Value))).ToImmutableDictionary();

        public static IImmutableDictionary<K,T> filter<K, T>(this IImmutableDictionary<K,T> m, Func<T, bool> predicate) =>
            m.Where(kv => predicate(kv.Value)).ToImmutableDictionary();
    }
}
