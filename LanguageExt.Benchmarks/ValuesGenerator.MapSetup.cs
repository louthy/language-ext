using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    internal partial class ValuesGenerator
    {
        public static ImmutableDictionary<T, T> SysColImmutableDictionarySetup<T>(Dictionary<T, T> values)
        {
            var immutableMap = ImmutableDictionary.Create<T, T>();
            foreach (var kvp in values)
            {
                immutableMap = immutableMap.Add(kvp.Key, kvp.Value);
            }

            return immutableMap;
        }

        public static ImmutableSortedDictionary<T, T> SysColImmutableSortedDictionarySetup<T>(Dictionary<T, T> values)
        {
            var immutableMap = ImmutableSortedDictionary.Create<T, T>();
            foreach (var kvp in values)
            {
                immutableMap = immutableMap.Add(kvp.Key, kvp.Value);
            }

            return immutableMap;
        }

        public static Dictionary<T, T> SysColDictionarySetup<T>(Dictionary<T, T> values)
        {
            var dictionary = new Dictionary<T, T>();
            foreach (var kvp in values)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            return dictionary;
        }

        public static HashMap<TEq, T, T> LangExtHashMapSetup<T, TEq>(Dictionary<T, T> values)
            where TEq : struct, Eq<T>
        {
            var hashMap = HashMap<TEq, T, T>();
            foreach (var kvp in values)
            {
                hashMap = hashMap.Add(kvp.Key, kvp.Value);
            }

            return hashMap;
        }

        public static Map<TOrd, T, T> LangExtMapSetup<T, TOrd>(Dictionary<T, T> values)
            where TOrd : struct, Ord<T>
        {
            var hashMap = Map<TOrd, T, T>();
            foreach (var kvp in values)
            {
                hashMap = hashMap.Add(kvp.Key, kvp.Value);
            }

            return hashMap;
        }
    }
}
