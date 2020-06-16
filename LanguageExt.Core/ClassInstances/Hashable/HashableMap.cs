using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

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
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(Map<K, V> x) =>
            GetHashCode(x).AsTask();
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
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(Map<OrdK, K, V> x) =>
            GetHashCode(x).AsTask();
    }
}
