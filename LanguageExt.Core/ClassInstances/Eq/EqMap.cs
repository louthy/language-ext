using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct EqMap<EqK, K, V> : Eq<Map<K, V>>
        where EqK : struct, Eq<K>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if `x` and `y` are equal</returns>
        [Pure]
        public bool Equals(Map<K, V> x, Map<K, V> y) =>
            x.Equals<EqK>(y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of `x`</returns>
        [Pure]
        public int GetHashCode(Map<K, V> x) =>
            x.GetHashCode();
    }

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
            default(EqMap<EqDefault<K>, K, V>).Equals(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of `x`</returns>
        [Pure]
        public int GetHashCode(Map<K, V> x) =>
            default(EqMap<EqDefault<K>, K, V>).GetHashCode(x);
    }
}
