using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Immutable;

public static class __TryOutExt
{
    public static Option<V> TryGetValue<K, V>(this IDictionary<K, V> self, K key)
    {
        V value;
        return self.TryGetValue(key, out value)
            ? Some(value)
            : None;
    }
    public static Option<V> TryGetValue<K, V>(this IReadOnlyDictionary<K, V> self, K key)
    {
        V value;
        return self.TryGetValue(key, out value)
            ? Some(value)
            : None;
    }
    public static Option<V> TryGetValue<K, V>(this IImmutableDictionary<K, V> self, K key)
    {
        V value;
        return self.TryGetValue(key, out value)
            ? Some(value)
            : None;
    }
    public static Option<K> TryGetValue<K>(this IImmutableSet<K> self, K key)
    {
        K value;
        return self.TryGetValue(key, out value)
            ? Some(value)
            : None;
    }

}
