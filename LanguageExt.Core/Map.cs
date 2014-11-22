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

        public static IImmutableDictionary<K, V> remove<K, V>(this IImmutableDictionary<K, V> self, K key, V value) =>
            self.Remove(key);

        public static bool contains<K, V>(this IImmutableDictionary<K, V> self, K key) =>
            self.ContainsKey(key);

        public static bool contains<K, V>(this IImmutableDictionary<K, V> self, KeyValuePair<K,V> kv) =>
            self.Contains(kv);

        public static bool contains<K, V>(this IImmutableDictionary<K, V> self, Tuple<K, V> kv) =>
            self.Contains(new KeyValuePair<K, V>(kv.Item1, kv.Item2));

        public static IImmutableDictionary<K, V> setItem<K, V>(this IImmutableDictionary<K, V> self, K key, V value) =>
            self.SetItem(key, value);

        public static Option<V> find<K, V>(this IImmutableDictionary<K, V> self, K key) =>
            contains(self, key)
                ? Some(self[key])
                : None;

        public static Unit iter<K, V>(this IImmutableDictionary<K, V> self, Action<K, V> action) =>
            ignore(iter<K>(self.Keys, key => action(key, self[key])));

        public static bool forall<K, V>(this IImmutableDictionary<K, V> self, Func<K,V,bool> pred)
        {
            bool state = true;
            foreach (var item in self)
            {
                state = state && pred(item.Key, item.Value);
            }
            return state;
        }

        public static IImmutableDictionary<K, U> map<K, T, U>(this IImmutableDictionary<K, T> m, Func<T, U> f) =>
            m.Select(kv => new KeyValuePair<K, U>(kv.Key, f(kv.Value))).ToImmutableDictionary();

        public static IImmutableDictionary<K, U> mapi<K, T, U>(this IImmutableDictionary<K, T> m, Func<K, T, U> f) =>
            m.Select(kv => new KeyValuePair<K, U>(kv.Key, f(kv.Key, kv.Value))).ToImmutableDictionary();

        public static IImmutableDictionary<K,T> filter<K, T>(this IImmutableDictionary<K,T> m, Func<T, bool> predicate) =>
            m.Where(kv => predicate(kv.Value)).ToImmutableDictionary();

        public static int length<K, T>(this IImmutableDictionary<K, T> self) =>
           self.Count;
    }
}
