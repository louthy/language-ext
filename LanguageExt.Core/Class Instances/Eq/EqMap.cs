using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct EqMap<K, V> : Eq<Map<K, V>>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if `x` and `y` are equal</returns>
        [Pure]
        public bool Equals(Map<K, V> x, Map<K, V> y) =>
            x.Equals(y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of `x`</returns>
        [Pure]
        public int GetHashCode(Map<K, V> x) =>
            default(HashableMap<K, V>).GetHashCode(x);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public Task<bool> EqualsAsync(Map<K, V> x, Map<K, V> y) =>
            Equals(x, y).AsTask();
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(Map<K, V> x) =>
            GetHashCode(x).AsTask();
    }

    public struct EqMap<OrdK, K, V> : Eq<Map<OrdK, K, V>>
        where OrdK : struct, Ord<K>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if `x` and `y` are equal</returns>
        [Pure]
        public bool Equals(Map<OrdK, K, V> x, Map<OrdK, K, V> y) =>
            x.Equals(y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of `x`</returns>
        [Pure]
        public int GetHashCode(Map<OrdK, K, V> x) =>
            default(HashableMap<OrdK, K, V>).GetHashCode(x);
  
        [Pure]
        public Task<bool> EqualsAsync(Map<OrdK, K, V> x, Map<OrdK, K, V> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Map<OrdK, K, V> x) => 
            GetHashCode(x).AsTask();      
    }
}
