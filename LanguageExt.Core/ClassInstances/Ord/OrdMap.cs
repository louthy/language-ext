using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdMap<OrdK, K, V> : Ord<Map<OrdK, K, V>>
        where OrdK : struct, Ord<K>
    {
        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if `x` greater than `y` : `1`
        /// if `x` less than `y`    : `-1`
        /// if `x` equals `y`       : `0`
        /// </returns>
        [Pure]
        public int Compare(Map<OrdK, K, V> x, Map<OrdK, K, V> y) =>
            x.CompareTo(y);

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
            x.GetHashCode();
    }

    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdMap<K, V> : Ord<Map<K, V>>
    {
        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if `x` greater than `y` : `1`
        /// if `x` less than `y`    : `-1`
        /// if `x` equals `y`       : `0`
        /// </returns>
        [Pure]
        public int Compare(Map<K, V> x, Map<K, V> y) =>
            x.CompareTo(y);

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
            x.GetHashCode();
    }
}
