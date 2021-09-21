using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static class OutExtensions
    {
        /// <summary>
        /// Get a value out of a dictionary as Some, otherwise None.
        /// </summary>
        /// <typeparam name="K">Key type</typeparam>
        /// <typeparam name="V">Value type</typeparam>
        /// <param name="self">Dictionary</param>
        /// <param name="Key">Key</param>
        /// <returns>OptionT filled Some(value) or None</returns>
        [Pure]
        public static Option<V> TryGetValue<K, V>(this IDictionary<K, V> self, K Key)
        {
            V value;
            return self.TryGetValue(Key, out value)
                ? Optional(value)
                : None;
        }

        /// <summary>
        /// Get a value out of a dictionary as Some, otherwise None.
        /// </summary>
        /// <typeparam name="K">Key type</typeparam>
        /// <typeparam name="V">Value type</typeparam>
        /// <param name="self">Dictionary</param>
        /// <param name="ReadOnlyKey">Key</param>
        /// <returns>OptionT filled Some(value) or None</returns>
        [Pure]
        public static Option<V> TryGetValue<K, V>(this IReadOnlyDictionary<K, V> self, K ReadOnlyKey)
        {
            V value;
            return self.TryGetValue(ReadOnlyKey, out value)
                ? Optional(value)
                : None;
        }
    }
}
