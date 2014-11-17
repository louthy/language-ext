using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;

public static class __TryExt
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
}
