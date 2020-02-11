using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct HashableMap<K, V> : Hashable<Map<K, V>>
    {
        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of `x`</returns>
        [Pure]
        public int GetHashCode(Map<K, V> x) =>
            x.GetHashCode();
    }

    public struct HashableMap<OrdK, K, V> : Hashable<Map<OrdK, K, V>>
        where OrdK : struct, Ord<K>
    {
        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of `x`</returns>
        [Pure]
        public int GetHashCode(Map<OrdK, K, V> x) =>
            x.GetHashCode();
    }
}
