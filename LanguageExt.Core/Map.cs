using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.List;

namespace LanguageExt
{
    public static partial class Map
    {
        public static IImmutableDictionary<K, V> add<K, V>(IImmutableDictionary<K, V> map, K key, V value) =>
            map.Add(key, value);

        public static IImmutableDictionary<K, V> add<K, V>(IImmutableDictionary<K, V> map, params Tuple<K, V>[] keyValues) =>
            keyValues.Count() == 0
                ? map
                : keyValues.Count() == 1
                    ? map.Add(keyValues[0].Item1, keyValues[0].Item2)
                    : addRange(map, keyValues);

        public static IImmutableDictionary<K, V> addRange<K, V>(IImmutableDictionary<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
            addRange(map, keyValues.Select(kv => new KeyValuePair<K, V>(kv.Item1, kv.Item2)));

        public static IImmutableDictionary<K, V> addRange<K, V>(IImmutableDictionary<K, V> map, IEnumerable<KeyValuePair<K, V>> keyValues) =>
            map.AddRange(keyValues);

        public static IImmutableDictionary<K, V> remove<K, V>(IImmutableDictionary<K, V> map, K key, V value) =>
            map.Remove(key);

        public static bool containsKey<K, V>(IImmutableDictionary<K, V> map, K key) =>
            map.ContainsKey(key);

        public static bool contains<K, V>(IImmutableDictionary<K, V> map, KeyValuePair<K, V> kv) =>
            map.Contains(kv);

        public static bool contains<K, V>(IImmutableDictionary<K, V> map, Tuple<K, V> kv) =>
            map.Contains(new KeyValuePair<K, V>(kv.Item1, kv.Item2));

        public static IImmutableDictionary<K, V> setItem<K, V>(IImmutableDictionary<K, V> map, K key, V value) =>
            map.SetItem(key, value);

        public static Option<V> find<K, V>(IImmutableDictionary<K, V> map, K key) =>
            containsKey(map, key)
                ? Some(map[key])
                : None;

        public static Unit iter<K, V>(IImmutableDictionary<K, V> map, Action<V> action) =>
            iter<K>(map.Keys, key => action(map[key]));

        public static Unit iter<K, V>(IImmutableDictionary<K, V> map, Action<K, V> action) =>
            iter<K>(map.Keys, key => action(key, map[key]));

        public static bool forall<K, V>(IImmutableDictionary<K, V> map, Func<K, V, bool> pred) =>
            map.All(kv => pred(kv.Key, kv.Value));

        public static bool forall<K, V>(IImmutableDictionary<K, V> map, Func<Tuple<K, V>, bool> pred) =>
            map.All(kv => pred(tuple(kv.Key, kv.Value)));

        public static bool forall<K, V>(IImmutableDictionary<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
            map.All(kv => pred(kv));

        public static IImmutableDictionary<K, U> map<K, T, U>(IImmutableDictionary<K, T> map, Func<T, U> f) =>
            map.Select(kv => new KeyValuePair<K, U>(kv.Key, f(kv.Value))).ToImmutableDictionary();

        public static IImmutableDictionary<K, U> map<K, T, U>(IImmutableDictionary<K, T> map, Func<K, T, U> f) =>
            map.Select(kv => new KeyValuePair<K, U>(kv.Key, f(kv.Key, kv.Value))).ToImmutableDictionary();

        public static IImmutableDictionary<K, T> filter<K, T>(IImmutableDictionary<K, T> map, Func<T, bool> predicate) =>
            map.Where(kv => predicate(kv.Value)).ToImmutableDictionary();

        public static IImmutableDictionary<K, T> choose<K, T>(IImmutableDictionary<K, T> map, Func<T, Option<T>> selector) =>
            Map.map(filter(Map.map(map, selector), t => t.IsSome), t => t.Value);

        public static IImmutableDictionary<K, T> choose<K, T>(IImmutableDictionary<K, T> map, Func<K, T, Option<T>> selector) =>
            Map.map(filter(Map.map(map, selector), t => t.IsSome), t => t.Value);

        public static int length<K, T>(IImmutableDictionary<K, T> map) =>
            map.Count;

        public static S fold<S, K, V>(IImmutableDictionary<K, V> map, S state, Func<K, V, S, S> folder)
        {
            iter(map, (key, value) => { state = folder(key, value, state); });
            return state;
        }

        public static bool exists<K, V>(IImmutableDictionary<K, V> map, Func<K, V, bool> pred)
        {
            foreach (var key in map.Keys)
            {
                if ( pred(key, map[key]) )
                    return true;
            }
            return false;
        }

        public static bool exists<K, V>(IImmutableDictionary<K, V> map, Func<Tuple<K, V>, bool> pred)
        {
            foreach (var key in map.Keys)
            {
                if (pred(Tuple.Create(key, map[key])))
                    return true;
            }
            return false;
        }

        public static bool exists<K, V>(IImmutableDictionary<K, V> map, Func<KeyValuePair<K, V>, bool> pred)
        {
            foreach (var keyValue in map)
            {
                if (pred(keyValue))
                    return true;
            }
            return false;
        }
    }
}

public static class __MapExt
{
    public static IImmutableDictionary<K, V> Add<K, V>(this IImmutableDictionary<K, V> map, params Tuple<K, V>[] keyValues) =>
        LanguageExt.Map.add(map, keyValues);

    public static IImmutableDictionary<K, V> AddRange<K, V>(this IImmutableDictionary<K, V> map, IEnumerable<Tuple<K, V>> keyValues) =>
        LanguageExt.Map.addRange(map, keyValues);

    public static bool Contains<K, V>(this IImmutableDictionary<K, V> map, Tuple<K, V> kv) =>
        LanguageExt.Map.contains(map, kv);

    public static Option<V> Find<K, V>(this IImmutableDictionary<K, V> map, K key) =>
        LanguageExt.Map.find(map, key);

    public static Unit Iter<K, V>(this IImmutableDictionary<K, V> map, Action<V> action) =>
        LanguageExt.Map.iter(map, action);

    public static Unit Iter<K, V>(this IImmutableDictionary<K, V> map, Action<K, V> action) =>
        LanguageExt.Map.iter(map, action);

    public static bool ForAll<K, V>(this IImmutableDictionary<K, V> map, Func<K, V, bool> pred) =>
        LanguageExt.Map.forall(map, pred);

    public static bool ForAll<K, V>(this IImmutableDictionary<K, V> map, Func<Tuple<K, V>, bool> pred) =>
        LanguageExt.Map.forall(map, pred);

    public static bool ForAll<K, V>(this IImmutableDictionary<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
        LanguageExt.Map.forall(map, pred);

    public static IImmutableDictionary<K, U> Map<K, T, U>(this IImmutableDictionary<K, T> map, Func<T, U> f) =>
        LanguageExt.Map.map(map, f);

    public static IImmutableDictionary<K, U> Mapi<K, T, U>(this IImmutableDictionary<K, T> map, Func<K, T, U> f) =>
        LanguageExt.Map.map(map, f);

    public static IImmutableDictionary<K, T> Filter<K, T>(this IImmutableDictionary<K, T> map, Func<T, bool> predicate) =>
        LanguageExt.Map.filter(map, predicate);

    public static IImmutableDictionary<K, T> Choose<K, T>(this IImmutableDictionary<K, T> map, Func<T, Option<T>> selector) =>
        LanguageExt.Map.choose(map, selector);

    public static IImmutableDictionary<K, T> Choose<K, T>(this IImmutableDictionary<K, T> map, Func<K, T, Option<T>> selector) =>
        LanguageExt.Map.choose(map, selector);

    public static int Length<K, T>(this IImmutableDictionary<K, T> map) =>
        LanguageExt.Map.length(map);

    public static S Fold<S, K, V>(this IImmutableDictionary<K, V> map, S state, Func<K, V, S, S> folder) =>
        LanguageExt.Map.fold(map, state, folder);

    public static bool Exists<K, V>(this IImmutableDictionary<K, V> map, Func<K, V, bool> pred) =>
        LanguageExt.Map.exists(map, pred);

    public static bool Exists<K, V>(this IImmutableDictionary<K, V> map, Func<Tuple<K, V>, bool> pred) =>
        LanguageExt.Map.exists(map,pred);

    public static bool Exists<K, V>(this IImmutableDictionary<K, V> map, Func<KeyValuePair<K, V>, bool> pred) =>
        LanguageExt.Map.exists(map, pred);
}

