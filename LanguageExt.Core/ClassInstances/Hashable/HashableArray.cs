using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array hash
    /// </summary>
    public struct HashableArray<HashA, A> : Hashable<A[]> where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A[] x) =>
            hash<HashA, A>(x);
    }

    /// <summary>
    /// Array hash
    /// </summary>
    public struct HashableArray<A> : Hashable<A[]>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A[] x) =>
            default(HashableArray<HashableDefault<A>, A>).GetHashCode(x);
    }
}
